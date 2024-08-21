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
        private readonly EventDispatcher _eventDispatcher;

        public ProductService(AppDBContext Context, IBrandService brandService,
            ITagsService tagsService, IPexitaTools pexitaTools, IMapper Mapper, IUserService userService, EventDispatcher dispatcher)
        {
            _Context = Context;
            _brandService = brandService;
            _tagsService = tagsService;
            _pexitaTools = pexitaTools;
            _mapper = Mapper;
            _userService = userService;
            _eventDispatcher = dispatcher;
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

            BrandModel productBrand = await _pexitaTools.AuthorizeProductCreationAsync(product.BrandID, requestingUsername);
            // resolving some values asynchronously. the reason is that auto mapper is originally made for Entity => DTO mapping not the other way around.
            // so it doesn't support asynchronous functions. I have to resolve some values here if I want that to be done asynchronously.

            ProductModel NewProduct = _mapper.Map<ProductModel>(product);
            NewProduct.Brand = productBrand;

            _Context.Products.Add(NewProduct);
            await _Context.SaveChangesAsync();

            if (!productBrand.BrandNewsLetters.IsNullOrEmpty()) // we'll dispatch events only if the brand has subscribers.
            {
                BrandNewProductEvent Event = new() { Brand = NewProduct.Brand, BrandID = NewProduct.BrandID, Product = NewProduct, ProductID = NewProduct.ID };
                _eventDispatcher.Dispatch(Event);
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
            return _mapper.Map<ProductInfoDTO>(model);
        }
        /// <summary>
        /// Gets a product from database via its ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product info as DTO</returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<ProductInfoDTO> GetProductByID(int id)
        {
            var x = await _Context.Products
                .Include(p => p.Brand)
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.ID == id)
                ?? throw new NotFoundException($"Entity {id} not Found in Products.");
            var result = ProductModelToInfoDTO(x);
            return result;
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

            productModel = _mapper.Map(product, productModel);

            _Context.Update(productModel);
            await _Context.SaveChangesAsync();

            if (NotInStock && product.Quantity > 0 && productModel.NewsLetters?.Count > 0) // only releasing an event when it has subscribers
            {
                var eventMessage = new ProductAvailableEvent(id); // Product has changed its state to "In stock" so we're publishing an event.
                await _eventDispatcher.DispatchAsync(eventMessage);
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
        public async Task AddCommentToProduct(CommentsDTO commentDTO, string requestingUsername)
        {
            ProductModel product = await _pexitaTools.AuthorizeProductAccessAsync(commentDTO.ProductID, requestingUsername);
            CommentsModel commentsModel = _mapper.Map<CommentsModel>(commentDTO);
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
            ProductModel product = await _Context.Products.FindAsync(rateDTO.ProductID) ?? throw new NotFoundException($"Product {rateDTO.ProductID} not found.");

            ProductRating rating = new() { Rating = rateDTO.ProductRating, Product = product, ProductID = product.ID };

            _Context.Ratings.Add(rating);

            await _Context.SaveChangesAsync();
        }
        /// <summary>
        /// checking if a product already exists in a brand's collection.
        /// </summary>
        /// <param name="BrandName"></param>
        /// <param name="ProductTitle"></param>
        /// <returns></returns>
        public async Task<bool> IsProductAlready(int BrandID, string ProductTitle)
        {
            var brand = await _Context.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.ID == BrandID);

            if (brand == null)
                throw new NotFoundException($"Brand {BrandID} Does not Exist.");

            brand.Products ??= [];
            return brand.Products.Any(x => x.Title == ProductTitle);
        }
        /// <summary>
        /// Checks if a given ID is a valid product.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if it's a record False otherwise.</returns>
        public bool IsProduct(int id)
        {
            return _Context.Products.FirstOrDefault(x => x.ID == id) != null;
        }
        /// <summary>
        /// Checks if a given DTO is a true representative of sth in DB.
        /// </summary>
        /// <param name="product"></param>
        /// <returns>True if it's a record False otherwise.</returns>
        public bool IsProduct(ProductInfoDTO product)
        {
            return IsProduct(product.ID);
        }
    }
}
