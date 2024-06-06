using AutoMapper;
using FluentValidation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Comments;
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

        public ProductService(AppDBContext Context, IBrandService brandService,
            ITagsService tagsService, IPexitaTools pexitaTools, IMapper Mapper, IUserService userService)
        {
            _Context = Context;
            _brandService = brandService;
            _tagsService = tagsService;
            _pexitaTools = pexitaTools;
            _mapper = Mapper;
            _userService = userService;
        }

        public bool AddProduct(ProductCreateVM product)
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
                List<ProductModel> products = _Context.Products.Include(b => b.Brand).ToList();
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
            try
            {
                return ProductModelToInfoVM(await _Context.Products.SingleAsync(n => n.ID == id));
            }
            catch (InvalidOperationException)
            {
                throw new NotFoundException();
            }
        }

        public async Task<ProductInfoVM> UpdateProductInfo(int id, ProductUpdateVM product, string requestingUsername)
        {
            ProductModel productModel = await _Context.Products.SingleAsync(n => n.ID == id) ?? throw new NotFoundException();
            try
            {
                if (_userService.IsAdmin(requestingUsername))
                    _mapper.Map(product, productModel);

                try
                {
                    await _Context.SaveChangesAsync();
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
                UserModel user = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
                bool isAdmin = user.Role == "admin";
                if (!isAdmin || user.Username != requestingUsername)
                {
                    throw new NotAuthorizedException();
                }

                ProductModel Product = await _Context.Products.FirstOrDefaultAsync(product => product.ID == id) ?? throw new NotFoundException();

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
            try
            {
                UserModel user = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
                bool isAdmin = user.Role == "admin";
                if (!isAdmin || user.Username != requestingUsername)
                {
                    throw new NotAuthorizedException();
                }

                ProductModel Product = await _Context.Products.SingleAsync(product => product.ID == commentDTO.ProductID);
                Product.Comments!.Add(commentDTO.Comment);
                await _Context.SaveChangesAsync();
                return true;
            }

            catch (InvalidOperationException)
            {
                throw new NotFoundException();
            }

            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<bool> UpdateProductRate(UpdateProductRateDTO rateDTO, string requestingUsername)
        {
            try
            {
                UserModel user = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
                bool isAdmin = user.Role == "admin";
                if (!isAdmin || user.Username != requestingUsername)
                {
                    throw new NotAuthorizedException();
                }

                ProductModel product = await _Context.Products.SingleAsync(product => product.ID == rateDTO.ProductID);
                ProductRating rating = new() { Rating = rateDTO.ProductRating, Product = product, ProductID = product.ID };

                product.Rating.Add(rating);

                await _Context.SaveChangesAsync();
                return true;
            }

            catch (InvalidOperationException)
            {
                throw new NotFoundException();
            }
        }

        public bool IsProductAlready(string BrandName, string ProductTitle)
        {
            return _Context.Brands.Include(bp => bp.Products).AsNoTracking().FirstOrDefault(x => x.Name == BrandName)?
                        .Products!.FirstOrDefault(x => x.Title == ProductTitle) != null;
        }

    }
}
