using AutoMapper;
using Pexita.Data;
using Pexita.Services;
using Moq;
using Moq.EntityFrameworkCore;
using Pexita.Data.Entities.Products;
using Microsoft.AspNetCore.Http;
using NunitTest.FakeServices;
using Pexita.Data.Entities.Tags;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Exceptions;
using Pexita.Services.Interfaces;
using Microsoft.CodeAnalysis;

namespace NunitTest.Product
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<AppDBContext> _mockDbContext;
        private FakeBrandService _fakeBrandService;
        private FakeTagsService _fakeTagsService;
        private FakePexitaTools _fakePexitaTools;
        private Mock<IMapper> _mockMapper;
        private ProductService _productService;
        private List<ProductModel> _capturedProducts;
        private ProductModel ExampleProduct;
        [SetUp]
        public void SetUp()
        {
            // Arrange
            _mockDbContext = new Mock<AppDBContext>();
            _fakeBrandService = new();
            _fakeTagsService = new FakeTagsService();
            _fakePexitaTools = new FakePexitaTools();
            _mockMapper = new Mock<IMapper>();
            _capturedProducts = new List<ProductModel>();

            _productService = new ProductService(
                _mockDbContext.Object,
                _fakeBrandService,
                _fakeTagsService,
                _fakePexitaTools,
                _mockMapper.Object
            );
            ExampleProduct = new ProductModel
            {
                ID = 1,
                Title = "Example Product",
                Description = "This is an example product.",
                Price = 29.99,
                Quantity = 100,
                IsAvailable = true,
                Colors = "Red, Blue",
                Rating = new List<ProductRating>
    {
        new ProductRating { Rating = 4 },
        new ProductRating { Rating = 5 },
        new ProductRating { Rating = 4 },
    },
                ProductPicsURL = "https://example.com/product-image.jpg",
                DateCreated = DateTime.UtcNow,
                BrandID = 1,
                Brand = new BrandModel
                {
                    ID = 1,
                    Name = "Example Brand",
                    Description = "This is an example brand.",
                    BrandPicURL = "https://example.com/brand-image.jpg",
                    Username = "example_user",
                    Email = "example@example.com",
                    DateCreated = DateTime.UtcNow
                },
                Comments = new List<CommentsModel>
    {
        new CommentsModel { Text = "Great product!", DateCreated = DateTime.UtcNow },
        new CommentsModel { Text = "Excellent quality!", DateCreated = DateTime.UtcNow },
    },
                Tags = new List<TagModel>
    {
        new TagModel { Title = "Electronics" },
        new TagModel { Title = "Gadgets" },
    },
                NewsLetters = new List<ProductNewsLetterModel>
    {
        new ProductNewsLetterModel {  },
        new ProductNewsLetterModel {  },
    },
                CartItems = new List<CartItems>
    {
        new CartItems { Count = 2},
        new CartItems { Count = 1},
    }
            };

            _ = _mockDbContext.Setup(x => x.Products).ReturnsDbSet(_capturedProducts);

            _ = _mockDbContext.Setup(context => context.Products.Add(It.IsAny<ProductModel>())).Callback<ProductModel>(_capturedProducts.Add);

            // Set up the IMapper mock to return the mapped ProductModel
            _ = _mockMapper.Setup(mapper => mapper.Map<ProductModel>(It.IsAny<ProductCreateVM>()))
                 .Returns<ProductCreateVM>(src =>
                 {
                     // Create a new ProductModel using the provided ProductCreateVM instance
                     var productModel = new ProductModel
                     {
                         // Map properties based on the defined mapping configuration
                         Title = src.Title,
                         Description = src.Description,
                         Price = src.Price,
                         Quantity = src.Quantity,
                         Brand = _fakeBrandService.GetBrandByName(src.Brand),
                         ProductPicsURL = _fakePexitaTools.SaveProductImages(src.ProductPics, $"{src.Brand}/{src.Title}").Result,
                         DateCreated = DateTime.UtcNow,
                         IsAvailable = true,
                         Tags = _fakePexitaTools.StringToTags(src.Tags),
                         Comments = new List<CommentsModel>(),
                         Rating = new List<ProductRating>()
                     };
                     return productModel;
                 });
            _mockMapper.Setup(mapper => mapper.Map<ProductInfoVM>(It.IsAny<ProductModel>())).Returns<ProductModel>(src =>
            {
                return new ProductInfoVM
                {
                    ID = src.ID,
                    Title = src.Title,
                    Description = src.Description,
                    Price = src.Price,
                    Quantity = src.Quantity,
                    Brand = _fakeBrandService.BrandModelToInfo(src.Brand), // Make sure the BrandInfoVM is correctly mapped using _brandService.BrandModelToInfo
                    Rate = _fakePexitaTools.GetRating(src.Rating.Select(x => x.Rating).ToList()), // Assuming GetRating method returns a double
                    Tags = _fakeTagsService.TagsToVM(src.Tags!), // Make sure TagsToVM method correctly maps the list of TagModel to List<TagInfoVM>
                    ProductPics = src.ProductPicsURL,
                    IsAvailable = src.IsAvailable,
                };
            });

            // Set up the mapping of ProductUpdateVM to ProductModel
            _mockMapper.Setup(mapper => mapper.Map(It.IsAny<ProductUpdateVM>(), It.IsAny<ProductModel>()))
                .Callback<ProductUpdateVM, ProductModel>((src, dest) =>
                {
                    // Perform the mapping manually for the properties that are not automatically mapped
                    dest.Title = src.Title;
                    dest.Description = src.Description;
                    dest.Price = src.Price;
                    dest.Quantity = src.Quantity;
                    dest.Brand = _fakeBrandService.GetBrandByName(src.Brand);
                    dest.ProductPicsURL = _fakePexitaTools.SaveProductImages(src.ProductPics, $"{src.Brand}/{src.Title}").Result;
                    dest.IsAvailable = src.IsAvailable;
                    dest.Tags = _fakePexitaTools.StringToTags(src.Tags);
                    // Add other properties that need manual mapping
                })
                .Returns(ExampleProduct); // Set the return value to exampleProduct
        }

        [TearDown]
        public void TearDown()
        {
            _capturedProducts.Clear();
        }

        [Test]
        public void AddProduct_ValidProduct_ReturnsTrue()
        {
            // Arrange
            var productCreateVM = new ProductCreateVM
            {
                Title = "Product 1",
                Description = "Description of Product 1",
                Price = 29.99,
                Quantity = 100,
                Brand = "Brand A",
                Rate = 4.5,
                ProductPics = new List<IFormFile>
                {
                    new FormFile(new MemoryStream(Array.Empty<byte>()), 0, 0, "example1", "example1.jpeg"),
                    new FormFile(new MemoryStream(Array.Empty<byte>()), 0, 0, "example2", "example2.jpeg"),
                },

                IsAvailable = true,
                Tags = "Electronics,Gadgets",
                Colors = "Red, Blue"
            };

            // Act
            bool result = _productService.AddProduct(productCreateVM);
            var addedproduct = _mockDbContext.Object.Products.First();

            // Assert

            Assert.That(result, Is.True); 

            Assert.That(addedproduct, Is.Not.Null);

            Assert.That(_capturedProducts, Has.Count.EqualTo(1)); 


            _mockDbContext.Verify(context => context.SaveChanges(), Times.Once);
        }

        [Test]
        public void AddProduct_NullProduct_ThrowsArgumentNullException()
        {
            // Arrange
            ProductCreateVM productCreateVM = null;

            // Act and Assert
            Assert.Throws<Exception>(() => _productService.AddProduct(productCreateVM));
        }

        [Test]
        public void GetProducts_WhenCalled_ReturnsListOfProducts()
        {
            // Arrange
            _capturedProducts.Add(ExampleProduct);

            // Act
            var products = _productService.GetProducts();

            // Assert
            Assert.That(products, Is.InstanceOf<List<ProductInfoVM>>());
            Assert.That(products, Is.Not.Empty);
            Assert.That(products[0].Brand, Is.Not.Null);
        }

        [Test]
        public void GetProducts_DatabaseEmpty_throwsNotfoundError()
        {
            // Arrange

            // Act and Assert 
            Assert.Throws<NotFoundException>(() => _productService.GetProducts());
        }

        [Test]
        public void UpdateProduct_GivenUpdatedObject_ReturnsUpdatedObject()
        {
            // Arrange
            var updatedProduct = new ProductUpdateVM
            {
                Title = "Updated Product",
                Description = "Updated product description.",
                Price = 39.99,
                Quantity = 50,
                Brand = "Updated Brand",
                ProductPics = new List<IFormFile>
                    {
                        new FormFile(new MemoryStream(Array.Empty<byte>()), 0, 0, "example3", "example3.jpeg"),
                        new FormFile(new MemoryStream(Array.Empty<byte>()), 0, 0, "example4", "example4.jpeg"),
                    },
                IsAvailable = false,
                Tags = "Electronics, Gadgets",
                Colors = "Green, Yellow"
                // Add other properties with updated values
            };

            var mockProductModel = new ProductModel
            {
                ID = 1,
                // Set other properties as needed
                Rating = new List<ProductRating>()
            };

            _mockDbContext.Setup(x => x.Products).ReturnsDbSet(new List<ProductModel> { mockProductModel }.AsQueryable());


            // Act
            var result = _productService.UpdateProductInfo(1, updatedProduct);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ID, Is.EqualTo(1));
                Assert.That(result.Title, Is.EqualTo("Updated Product"));
                Assert.That(result.Description, Is.EqualTo("Updated product description."));
            });
        }
    }
}
