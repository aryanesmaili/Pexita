using AutoMapper;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Pexita.Data;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.Tags;
using Pexita.Exceptions;
using Pexita.Services.Interfaces;
using Pexita.Utility;
using System.Linq;

namespace Pexita.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDBContext _Context;
        private readonly IBrandService _brandService;
        private readonly ITagsService _tagsService;
        private readonly IPexitaTools _pexitaTools;
        private readonly IMapper _mapper;
        public ProductService(AppDBContext Context, IBrandService brandService,
            ITagsService tagsService, IPexitaTools pexitaTools, IMapper Mapper)
        {
            _Context = Context;
            _brandService = brandService;
            _tagsService = tagsService;
            _pexitaTools = pexitaTools;
            _mapper = Mapper;
        }
        // Add validation to check if the product already exists!
        public bool AddProduct(ProductCreateVM product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                ProductModel NewProduct = _mapper.Map<ProductModel>(product);

                _Context.Products.Add(NewProduct);
                _Context.SaveChanges();
                return true;
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

        public ProductInfoVM GetProductByID(int id)
        {
            try
            {
                return ProductModelToInfoVM(_Context.Products.Single(n => n.ID == id));
            }
            catch (InvalidOperationException)
            {
                throw new NotFoundException();
            }
        }

        public ProductInfoVM UpdateProductInfo(int id, ProductUpdateVM product)
        {
            ProductModel productModel = _Context.Products.FirstOrDefault(n => n.ID == id) ?? throw new NotFoundException();
            _mapper.Map(product, productModel);

            try
            {
                _Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving changes to the database.", ex);
            }
            return ProductModelToInfoVM(productModel);
        }

        public bool DeleteProduct(int id)
        {
            try
            {
                ProductModel Product = _Context.Products.FirstOrDefault(product => product.ID == id) ?? throw new NotFoundException();

                _Context.Products.Remove(Product);
                _Context.SaveChanges();
                return true;
            }

            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool AddCommentToProduct(int ProductID, CommentsModel comment)
        {
            try
            {
                ProductModel Product = _Context.Products.Single(product => product.ID == ProductID);
                Product.Comments!.Add(comment);
                _Context.SaveChanges();
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
        public bool UpdateProductRate(int ProductID, int rate)
        {
            try
            {
                ProductModel product = _Context.Products.Single(product => product.ID == ProductID);
                ProductRating rating = new() { Rating = rate, Product = product, ProductID = product.ID };

                product.Rating.Add(rating);

                _Context.SaveChanges();
                return true;
            }

            catch (InvalidOperationException)
            {
                throw new NotFoundException();
            }
        }
    }
}
