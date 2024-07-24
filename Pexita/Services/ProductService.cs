using AutoMapper;
using FluentValidation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;
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

        public bool AddProduct(ProductCreateDTO product)
        {
            try
            {
                ProductModel NewProduct = _mapper.Map<ProductModel>(product);

                _Context.Products.Add(NewProduct);
                _Context.SaveChanges();
                return true;
            }
            catch (ValidationException e)
            {
                throw new ValidationException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public List<ProductInfoVM> GetProducts()
        {
            try
            {
                List<ProductModel> products = _Context.Products.Include(b => b.Brand).Include(c => c.Comments).Include(t => t.Tags).ToList();
                if (products.Count > 0)
                {
                    List<ProductInfoVM> productsVM = products.Select(ProductModelToInfoVM).ToList();
                    return productsVM;
                }
                else
                {
                    throw new NotFoundException("No Records found in Products Table");
                }
            }

            catch (NotFoundException e)
            {
                throw new NotFoundException(e.Message);
            }

            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<ProductInfoVM> GetProducts(int count)
        {
            if (count > _Context.Products.Count())
                throw new ArgumentOutOfRangeException(nameof(count));

            try
            {
                List<ProductModel> prods = _Context.Products.Take(count).ToList();

                if (prods.Count > 0)
                {
                    return prods.Select(ProductModelToInfoVM).ToList();
                }

                else
                {
                    throw new NotFoundException("No record was Found");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public ProductInfoVM ProductModelToInfoVM(ProductModel model)
        {
            return _mapper.Map<ProductInfoVM>(model);
        }

        public async Task<ProductInfoVM> GetProductByID(int id)
        {
            return ProductModelToInfoVM(await _Context.Products
                                                        .Include(p => p.Brand)
                                                        .Include(p => p.Comments)
                                                        .Include(p => p.Tags)
                                                        .FirstOrDefaultAsync(p => p.ID == id)
                                                        ?? throw new NotFoundException($"Entity {id} not Found in Products."));
        }

        public async Task<ProductInfoVM> UpdateProductInfo(int id, ProductUpdateDTO product, string requestingUsername)
        {
            try
            {
                ProductModel productModel = await _pexitaTools.AuthorizeProductRequest(id, requestingUsername);
                bool NotInStock = productModel.Quantity == 0;
                _mapper.Map(product, productModel);

                try
                {
                    await _Context.SaveChangesAsync();
                    if(NotInStock && product.Quantity > 0)
                    {
                        var eventMessage = new ProductAvailableEvent(id);
                        _eventdispatcher.Dispatch(eventMessage);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while saving changes to the database.", ex);
                }
                return ProductModelToInfoVM(productModel);
            }
            catch (ValidationException e)
            {
                throw new ValidationException(e.Message);
            }
        }

        public async Task<bool> DeleteProduct(int id, string requestingUsername)
        {
            try
            {
                ProductModel Product = await _pexitaTools.AuthorizeProductRequest(id, requestingUsername);

                _Context.Products.Remove(Product);
                await _Context.SaveChangesAsync();
                return true;
            }

            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> AddCommentToProduct(ProductCommentDTO commentDTO, string requestingUsername)
        {
            ProductModel product = await _pexitaTools.AuthorizeProductRequest(commentDTO.ProductID, requestingUsername);

            product.Comments.Add(commentDTO.Comment);
            await _Context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductRate(UpdateProductRateDTO rateDTO, string requestingUsername)
        {
            ProductModel product = await _pexitaTools.AuthorizeProductRequest(rateDTO.ProductID, requestingUsername);

            ProductRating rating = new() { Rating = rateDTO.ProductRating, Product = product, ProductID = product.ID };

            product.Rating.Add(rating);

            await _Context.SaveChangesAsync();
            return true;

        }

        public bool IsProductAlready(string BrandName, string ProductTitle)
        {
            return _Context.Brands.Include(bp => bp.Products).AsNoTracking().FirstOrDefault(x => x.Name == BrandName)?
                        .Products!.FirstOrDefault(x => x.Title == ProductTitle) != null;
        }

        public async Task<ProductInfoVM> PatchProductInfo(int id, ProductUpdateDTO productDTO, string requestingUsername)
        {
            ProductModel product = await _pexitaTools.AuthorizeProductRequest(id, requestingUsername);
            bool NotInStock = product.Quantity == 0;
            _mapper.Map(productDTO, product);

            try
            {
                await _Context.SaveChangesAsync();

                if (NotInStock && productDTO.Quantity > 0)
                {
                    var eventMessage = new ProductAvailableEvent(product.ID);
                    _eventdispatcher.Dispatch(eventMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving changes to the database.", ex);
            }
            return ProductModelToInfoVM(product);
        }
    }
}
