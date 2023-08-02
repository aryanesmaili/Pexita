using AutoMapper;
using Pexita.Data;
using Pexita.Services.Interfaces;
using Pexita.Services;
using Pexita.Utility;
using Moq;
using Microsoft.EntityFrameworkCore;
using Pexita.Data.Entities.Products;

namespace NunitTest.Product
{
    [TestFixture]
    public class ProductServiceTests
    {
        // Mock for AppDBContext
        private Mock<AppDBContext> _mockDbContext;
        private ProductService _productService;

        [SetUp]
        public void Setup()
        {
            // Initialize the mock context and pass it to the ProductService
            _mockDbContext = new Mock<AppDBContext>();
            // Initialize other dependencies (you can use real implementations or more mocks for other services)
            var brandServiceMock = new Mock<IBrandService>();
            var tagsServiceMock = new Mock<ITagsService>();
            var pexitaToolsMock = new Mock<PexitaTools>();
            var mapperMock = new Mock<IMapper>();

            // Create ProductService instance with the mock context and other dependencies
            _productService = new ProductService(
                _mockDbContext.Object,
                brandServiceMock.Object,
                tagsServiceMock.Object,
                pexitaToolsMock.Object,
                mapperMock.Object
            );
        }

        [Test]
        public void AddProduct_GivenRightObject_ShouldAddToDB()
        {
            // Arrange


            // Act


            // Assert

        }
    }
}
