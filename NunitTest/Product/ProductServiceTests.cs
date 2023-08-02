using AutoMapper;
using Pexita.Data;
using Pexita.Services.Interfaces;
using Pexita.Services;
using Pexita.Utility;
using Moq;
using Moq.EntityFrameworkCore;
using Pexita.Data.Entities.Products;
using Microsoft.AspNetCore.Http;

namespace NunitTest.Product
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<AppDBContext> _mockDbContext;
        private Mock<IBrandService> _mockBrandService;
        private Mock<ITagsService> _mockTagsService;
        private Mock<PexitaTools> _mockPexitaTools;
        private Mock<IMapper> _mockMapper;
        private ProductService _productService;
        private List<ProductModel> _capturedProducts;

        [SetUp]
        public void SetUp()
        {
            // Arrange - Create mock instances
            _mockDbContext = new Mock<AppDBContext>();
            _mockBrandService = new Mock<IBrandService>();
            _mockTagsService = new Mock<ITagsService>();
            _mockPexitaTools = new Mock<PexitaTools>();
            _mockMapper = new Mock<IMapper>();
            _capturedProducts = new List<ProductModel>();

            _productService = new ProductService(
                _mockDbContext.Object,
                _mockBrandService.Object,
                _mockTagsService.Object,
                _mockPexitaTools.Object,
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
                    // You can add IFormFile instances here as per your test scenario
                    // For example: new FormFile(Stream.Null, 0, 0, "file1", "product1.jpg"),
                    //              new FormFile(Stream.Null, 0, 0, "file2", "product1_2.jpg"),
                },
                IsAvailable = true,
                Tags = "Electronics, Gadgets",
                Colors = "Red, Blue"
            };

            var mappedProductModel = new ProductModel
            {
                Title = "Test Product",
                Price = 10.99,
                // Add other properties as needed
            };

            // Set up the IMapper mock to return the mapped ProductModel
            _mockMapper.Setup(mapper => mapper.Map<ProductModel>(productCreateVM)).Returns(mappedProductModel);

            // Set up the DbSet mock to capture the added product
            var capturedProducts = new List<ProductModel>();
            _mockDbContext.Setup(context => context.Products.Add(It.IsAny<ProductModel>())).Callback<ProductModel>((product) =>
            {
                // Capture the added product
                capturedProducts.Add(product);
            });

            // Act
            bool result = _productService.AddProduct(productCreateVM);

            // Assert
            Assert.IsTrue(result); // Make sure the method returns true on successful addition
            Assert.AreEqual(1, capturedProducts.Count); // Verify that the product was added once

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
