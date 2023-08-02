using AutoMapper;
using Pexita.Data;
using Pexita.Services.Interfaces;
using Pexita.Services;
using Pexita.Utility;
using Moq;
using Moq.EntityFrameworkCore;
using Pexita.Data.Entities.Products;
using Microsoft.AspNetCore.Http;
using NunitTest.FakeServices;
using Pexita.Data.Entities.Tags;
using System.Security.Policy;
using Pexita.Data.Entities.Comments;

namespace NunitTest.Product
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<AppDBContext> _mockDbContext;
        private FakeBrandService _fakeBrandService;
        private Mock<ITagsService> _mockTagsService;
        private FakePexitaTools _fakePexitaTools;
        private Mock<IMapper> _mockMapper;
        private ProductService _productService;
        private List<ProductModel> _capturedProducts;

        [SetUp]
        public void SetUp()
        {
            // Arrange - Create mock instances
            _mockDbContext = new Mock<AppDBContext>();
            _fakeBrandService = new();
            _mockTagsService = new Mock<ITagsService>();
            _fakePexitaTools = new FakePexitaTools();
            _mockMapper = new Mock<IMapper>();
            _capturedProducts = new List<ProductModel>();

            _productService = new ProductService(
                _mockDbContext.Object,
                _fakeBrandService,
                _mockTagsService.Object,
                _fakePexitaTools,
                _mockMapper.Object
            );

            _mockDbContext.Setup(x => x.Products).ReturnsDbSet(_capturedProducts);

            _ = _mockDbContext.Setup(context => context.Products.Add(It.IsAny<ProductModel>())).Callback<ProductModel>(_capturedProducts.Add);

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

            var mappedProductModel = _mockDbContext.Object.Products.First();

            // Set up the IMapper mock to return the mapped ProductModel
           _mockMapper.Setup(mapper => mapper.Map<ProductModel>(It.IsAny<ProductCreateVM>()))
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
                        DateAdded = DateTime.UtcNow,
                        IsAvailable = true,
                        Tags = _fakePexitaTools.StringToTags(src.Tags),
                        Comments = new List<CommentsModel>(),
                        Rating = new List<ProductRating>()
                    };
                        return productModel;
                    });


            // Act
            bool result = _productService.AddProduct(productCreateVM);

            // Assert
            Assert.IsTrue(result); // Make sure the method returns true on successful addition
            Assert.AreEqual(1, _capturedProducts.Count); // Verify that the product was added once

            // You can perform additional assertions on the capturedProducts list to check its properties
            // For example, if you have an Id property in ProductModel, you can check that it's not zero or negative.

            // Verify that SaveChanges was called on the context exactly once
            _mockDbContext.Verify(context => context.SaveChanges(), Times.Once);
        }

        [Test]
        public void AddProduct_NullProduct_ThrowsArgumentNullException()
        {
            // Arrange
            ProductCreateVM productCreateVM = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => _productService.AddProduct(productCreateVM));
        }
    }
}
