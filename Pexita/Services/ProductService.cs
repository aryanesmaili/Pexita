using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Pexita.Data;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.Tags;
using Pexita.Exceptions;
using Pexita.Services.Interfaces;
using System.Linq;

namespace Pexita.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDBContext _Context;
        private readonly IBrandService _brandService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ITagsService _tagsService;
        public ProductService(AppDBContext Context, IBrandService brandService,
            IWebHostEnvironment webHostEnvironment, ITagsService tagsService)
        {
            _Context = Context;
            _brandService = brandService;
            _webHostEnvironment = webHostEnvironment;
            _tagsService = tagsService;
        }
        public bool AddProduct(ProductCreateVM product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                string identifier = $"{product.Brand}-{product.Title}";
                ProductModel NewProduct = new()
                {
                    Title = product.Title,
                    Description = product.Description,
                    Price = product.Price,
                    Brand = _brandService.GetBrandByName(product.Brand),
                    Quantity = product.Quantity,
                    Colors = product.Colors,
                    Rate = new List<double>(),
                    ProductPicsURL = SaveProductImages(product.ProductPics, identifier).Result,
                    DateAdded = DateTime.UtcNow,
                    IsAvailable = true,
                    Tags = StringToTags(product.Tags),
                    Comments = new List<CommentsModel>()
                };

                _Context.Products.Add(NewProduct);
                _Context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private async Task<string> SaveProductImages(List<IFormFile> files, string identifier)
        {
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, $"/Images/{identifier}");
            string[] allowedTypes = new[] { "image/jpeg", "image/png" };

            if (!Directory.Exists(imagePath))
                Directory.CreateDirectory(imagePath);

            for (int i = 0; i < files.Count; i++)
            {
                if (!allowedTypes.Contains(files[i].ContentType))
                    throw new FormatException();

                string uniqueFileName = $"{identifier}_{i:03}{Path.GetExtension(files[i].FileName)}";
                string filePath = Path.Combine(imagePath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await files[i].CopyToAsync(stream);
                }
            }
            return imagePath;
        }

        public List<ProductInfoVM> GetProducts()
        {
            try
            {
                List<ProductModel> products = _Context.Products.ToList();
                if (products.Count > 0)
                {

                    List<ProductInfoVM> productsVM = products.Select(ProductModelToInfoVM).ToList();
                    return productsVM;
                }
                else
                {
                    throw new NotFoundException();
                }
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
            ProductInfoVM product = new()
            {
                ID = model.ID,
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Quantity = model.Quantity,
                Brand = _brandService.BrandModelToInfo(model.Brand),
                Rate = GetRating(model.Rate!),
                DateCreated = model.DateAdded,
                Tags = _tagsService.TagsToVM(model.Tags!),
                ProductPics = model.ProductPicsURL,
                IsAvailable = model.IsAvailable,
            };
            return product;
        }
        private static double GetRating(List<double> Ratings) => Ratings.Average();

        private List<TagModel> StringToTags(string Tag)
        {
            if (Tag.Length == 0)
            {
                return new List<TagModel>();
            }
            var tags = Tag.Split(',');
            List<TagModel> res = new();

            foreach (string tag in tags)
            {
                if (_Context.Tags.FirstOrDefault(t => t.Title == tag) == null)
                    _Context.Tags.Add(new TagModel() { Title = tag });

                TagModel t = _Context.Tags.Single(t => t.Title == tag);
                t.TimesUsed++;
                res.Add(t);
            }
            return res;
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

        public ProductInfoVM UpdateProductInfo(int id, ProductCreateVM product)
        {
            ProductModel productModel = _Context.Products.FirstOrDefault(n => n.ID == id) ?? throw new NotFoundException();

            string identifier = $"{product.Brand}-{product.Title}";

            productModel.Title = product.Title;
            productModel.Description = product.Description;
            productModel.Price = product.Price;
            productModel.Brand = _brandService.GetBrandByName(product.Brand);

            productModel.ProductPicsURL = SaveProductImages(product.ProductPics, identifier).GetAwaiter().GetResult();
            productModel.IsAvailable = product.IsAvailable;

            try
            {
                _Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the error, rollback changes, etc.)
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
        public bool UpdateProductRate(int ProductID, double rate)
        {
            try
            {
                ProductModel product = _Context.Products.Single(product => product.ID == ProductID);

                product.Rate!.Add(rate);

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
