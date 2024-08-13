using Pexita.Data.Entities.Products;
namespace Pexita.Services.Interfaces
{
    public interface IProductService
    {
        public Task<ProductInfoDTO> AddProduct(ProductCreateDTO product, string requestingUsername);
        public List<ProductInfoDTO> GetProducts();
        public List<ProductInfoDTO> GetProducts(int count);
        public Task<ProductInfoDTO> GetProductByID(int id);
        public Task<ProductInfoDTO> UpdateProductInfo(int id, ProductUpdateDTO product, string requestingUsername);
        public Task<ProductInfoDTO> PatchProductInfo(int id, ProductUpdateDTO product, string requestingUsername);
        public Task DeleteProduct(int id, string requestingUsername);
        public ProductInfoDTO ProductModelToInfoDTO(ProductModel model);
        public Task AddCommentToProduct(ProductCommentDTO commentDTO, string requestingUsername);
        public Task UpdateProductRate(UpdateProductRateDTO rateDTO, string requestingUsername);
        public bool IsProductAlready(string BrandName, string ProductTitle);
    }
}

