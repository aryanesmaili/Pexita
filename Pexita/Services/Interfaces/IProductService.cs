using Pexita.Data.Entities.Products;
namespace Pexita.Services.Interfaces
{
    public interface IProductService
    {
        public Task<ProductModel> AddProduct(ProductCreateDTO product, string requestingUsername);
        public List<ProductModel> GetProducts();
        public List<ProductModel> GetProducts(int count);
        public Task<ProductModel> GetProductByID(int id);
        public Task<ProductModel> UpdateProductInfo(int id, ProductUpdateDTO product, string requestingUsername);
        public Task<ProductModel> PatchProductInfo(int id, ProductUpdateDTO product, string requestingUsername);
        public Task DeleteProduct(int id, string requestingUsername);
        public ProductModel ProductModelToInfoVM(ProductModel model);
        public Task AddCommentToProduct(ProductCommentDTO commentDTO, string requestingUsername);
        public Task UpdateProductRate(UpdateProductRateDTO rateDTO, string requestingUsername);
        public bool IsProductAlready(string BrandName, string ProductTitle);
    }
}

