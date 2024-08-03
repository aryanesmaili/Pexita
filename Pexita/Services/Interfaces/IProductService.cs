using Pexita.Data.Entities.Products;
namespace Pexita.Services.Interfaces
{
    public interface IProductService
    {
        public Task<ProductInfoVM> AddProduct(ProductCreateDTO product, string requestingUsername);
        public List<ProductInfoVM> GetProducts();
        public List<ProductInfoVM> GetProducts(int count);
        public Task<ProductInfoVM> GetProductByID(int id);
        public Task<ProductInfoVM> UpdateProductInfo(int id, ProductUpdateDTO product, string requestingUsername);
        public Task<ProductInfoVM> PatchProductInfo(int id, ProductUpdateDTO product, string requestingUsername);
        public Task DeleteProduct(int id, string requestingUsername);
        public ProductInfoVM ProductModelToInfoVM(ProductModel model);
        public Task AddCommentToProduct(ProductCommentDTO commentDTO, string requestingUsername);
        public Task UpdateProductRate(UpdateProductRateDTO rateDTO, string requestingUsername);
        public bool IsProductAlready(string BrandName, string ProductTitle);
    }
}

