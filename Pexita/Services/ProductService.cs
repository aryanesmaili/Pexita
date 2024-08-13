using AutoMapper;
using FluentValidation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pexita.Data;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Events;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.Tags;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDBContext _Context;
        private readonly IBrandService _brandService;
        private readonly ITagsService _tagsService;
        private readonly IPexitaTools _pexitaTools;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly EventDispatcher _eventdispatcher;

        public ProductService(AppDBContext Context, IBrandService brandService,
            ITagsService tagsService, IPexitaTools pexitaTools, IMapper Mapper, IUserService userService, EventDispatcher dispatcher)
        {
            _Context = Context;
            _brandService = brandService;
            _tagsService = tagsService;
            _pexitaTools = pexitaTools;
            _mapper = Mapper;
            _userService = userService;
            _eventdispatcher = dispatcher;
        }

        /// <summary>
        /// Adds a new product to Product Table.
        /// </summary>
        /// <param name="product"> the product to be added to the Table</param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<ProductInfoDTO> AddProduct(ProductCreateDTO product, string requestingUsername)
        {
            await _pexitaTools.AuthorizeProductCreationAsync(product.Brand, requestingUsername);

            // resolving some values asynchronously. the reason is that auto mapper is originally made for Entity => DTO mapping not the other way around.
            // so it doesn't support asynchronous functions. I have to resolve some values here if I want that to be done asynchronously.
            BrandModel productBrand = await _brandService.GetBrandByName(product.Brand);
            string ProductPicsURL = await _pexitaTools.SaveEntityImages(product.ProductPics, $"{product.Brand}/{product.Title}", false);
            var Tags = await _pexitaTools.StringToTags(product.Tags);

            ProductModel NewProduct = _mapper.Map<ProductModel>(product);
            NewProduct.Brand = productBrand;
            NewProduct.ProductPicsURL = ProductPicsURL;
            foreach (var tag in Tags)
            {
                tag.Products.Add(NewProduct);
            }
            NewProduct.Tags = Tags;

            _Context.Products.Add(NewProduct);
            await _Context.SaveChangesAsync();

            if (!productBrand.BrandNewsLetters.IsNullOrEmpty()) // we'll dispatch events only if the brand has subscribers.
            {
                BrandNewProductEvent Event = new() { Brand = NewProduct.Brand, BrandID = NewProduct.BrandID, Product = NewProduct, ProductID = NewProduct.ID };
                _eventdispatcher.Dispatch(Event);
            }
            return ProductModelToInfoDTO(NewProduct);
        }
        /// <summary>
        /// Get the list of products along with their brands, comments and tags.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public List<ProductInfoDTO> GetProducts()
        {
            List<ProductModel> products = _Context.Products.Include(b => b.Brand).Include(c => c.Comments).Include(t => t.Tags).ToList();
            if (products.Count > 0)
            {
                List<ProductInfoDTO> productsDTO = products.Select(ProductModelToInfoDTO).ToList();
                return productsDTO;
            }
            else
            {
                throw new NotFoundException("No Records found in Products Table");
            }
        }

        /// <summary>
        /// get a set amount of products from database.
        /// </summary>
        /// <param name="count">Count of how many products you want to get.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public List<ProductInfoDTO> GetProducts(int count)
        {
            if (count > _Context.Products.Count())
                throw new ArgumentOutOfRangeException(nameof(count));

            List<ProductModel> prods = _Context.Products.Take(count).ToList();

            if (prods.Count > 0)
            {
                return prods.Select(ProductModelToInfoDTO).ToList();
            }

            else
            {
                throw new NotFoundException("No record was Found");
            }

        }
        /// <summary>
        /// Converts a database table to a DTO to be sent to frontend.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ProductInfoDTO ProductModelToInfoDTO(ProductModel model)
        {
            BrandInfoDTO brand = _brandService.BrandModelToInfo(model.Brand);
            double rate = _pexitaTools.GetRating(model.Rating.Select(x => x.Rating).ToList());
            List<TagInfoDTO> tags = _tagsService.TagsToDTO(model.Tags ?? []);

            var res = _mapper.Map<ProductInfoDTO>(model);
            return res;
        }
        /// <summary>
        /// Gets a product from database via its ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product info as DTO</returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<ProductInfoDTO> GetProductByID(int id)
        {
            return ProductModelToInfoDTO(await _Context.Products
                                                        .Include(p => p.Brand)
                                                        .Include(p => p.Comments)
                                                        .Include(p => p.Tags)
                                                        .FirstOrDefaultAsync(p => p.ID == id)
                                                        ?? throw new NotFoundException($"Entity {id} not Found in Products."));
        }
        /// <summary>
        /// Completely Edits a database table based on the object given (PUT). publishes a <see cref="ProductAvailableEvent"/> if a product's state changes.
        /// </summary>
        /// <param name="id">id of the db record to be edited.</param>
        /// <param name="product">new product info.</param>
        /// <param name="requestingUsername">the user requesting the edit.</param>
        /// <returns></returns>
        public async Task<ProductInfoDTO> UpdateProductInfo(int id, ProductUpdateDTO product, string requestingUsername)
        {

            ProductModel productModel = await _pexitaTools.AuthorizeProductAccessAsync(id, requestingUsername);
            bool NotInStock = productModel.Quantity == 0;

            ProductModel newProductState = _mapper.Map(product, productModel);

            if (product.ProductPics.Count > 0)
            {
                newProductState.ProductPicsURL = await _pexitaTools.SaveEntityImages(product.ProductPics, $"{product.Brand}/{product.Title}", true);
            }
            var tgs = await _pexitaTools.StringToTags(product.Tags);
            foreach (var t in tgs)
            {
                if (!t.Products.Contains(newProductState))
                {
                    t.Products.Add(newProductState);
                }
            }
            newProductState.Tags = tgs;

            await _Context.SaveChangesAsync();
            if (NotInStock && product.Quantity > 0 && newProductState.NewsLetters?.Count > 0) // only releasing an event when it has subscribers
            {
                var eventMessage = new ProductAvailableEvent(id); // Product has changed its state to "In stock" so we're publishing an event.
                await _eventdispatcher.DispatchAsync(eventMessage);
            }
            return ProductModelToInfoDTO(productModel);
        }
        /// <summary>
        /// Deletes a record from database after checking if the requester has authorization to do so.
        /// </summary>
        /// <param name="id">id of the product to be deleted</param>
        /// <param name="requestingUsername">the requester's username.used in authenticating request.</param>
        /// <returns></returns>
        public async Task DeleteProduct(int id, string requestingUsername)
        {
            ProductModel Product = await _pexitaTools.AuthorizeProductAccessAsync(id, requestingUsername);

            _Context.Products.Remove(Product);
            await _Context.SaveChangesAsync();
        }
        /// <summary>
        /// Adds the given comment to the product.
        /// </summary>
        /// <param name="commentDTO">the comment to be added.</param>
        /// <param name="requestingUsername">the requester's username.used in authenticating request.</param>
        /// <returns></returns>
        public async Task AddCommentToProduct(ProductCommentDTO commentDTO, string requestingUsername)
        {
            ProductModel product = await _pexitaTools.AuthorizeProductAccessAsync(commentDTO.ProductID, requestingUsername);
            CommentsModel commentsModel = _mapper.Map<CommentsModel>(commentDTO); // TODO: add configuration 
            product.Comments?.Add(commentsModel);
            await _Context.SaveChangesAsync();
        }
        /// <summary>
        /// updates a record's rating.
        /// </summary>
        /// <param name="rateDTO"></param>
        /// <param name="requestingUsername"></param>
        /// <returns></returns>
        public async Task UpdateProductRate(UpdateProductRateDTO rateDTO, string requestingUsername)
        {
            ProductModel product = await _pexitaTools.AuthorizeProductAccessAsync(rateDTO.ProductID, requestingUsername);

            ProductRating rating = new() { Rating = rateDTO.ProductRating, Product = product, ProductID = product.ID };

            product.Rating.Add(rating);

            await _Context.SaveChangesAsync();
        }
        /// <summary>
        /// checking if a product already exists in a brand's collection.
        /// </summary>
        /// <param name="BrandName"></param>
        /// <param name="ProductTitle"></param>
        /// <returns></returns>
        public bool IsProductAlready(string BrandName, string ProductTitle)
        {
            return _Context.Brands.Include(bp => bp.Products).AsNoTracking().FirstOrDefault(x => x.Name == BrandName)?
                        .Products!.FirstOrDefault(x => x.Title == ProductTitle) != null;
        }
        /// <summary>
        /// partially edits a product's record in database.
        /// </summary>
        /// <param name="id">ID of the product to be edited</param>
        /// <param name="productDTO">new product's info.</param>
        /// <param name="requestingUsername">the user requesting the edit.used in authenticating the request</param>
        /// <returns></returns>
        public async Task<ProductInfoDTO> PatchProductInfo(int id, ProductUpdateDTO productDTO, string requestingUsername)
        {
            ProductModel product = await _pexitaTools.AuthorizeProductAccessAsync(id, requestingUsername);
            bool NotInStock = product.Quantity == 0;
            _mapper.Map(productDTO, product);
            await _Context.SaveChangesAsync();

            if (NotInStock && productDTO.Quantity > 0 && product.NewsLetters?.Count > 0)
            {
                var eventMessage = new ProductAvailableEvent(product.ID);
                await _eventdispatcher.DispatchAsync(eventMessage);
            }

            return ProductModelToInfoDTO(product);
        }
    }
}
