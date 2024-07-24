using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Products;
namespace Pexita.Services.Interfaces
{
    public interface IProductService
    {
        public bool AddProduct(ProductCreateDTO product);
        public List<ProductInfoVM> GetProducts();
        public List<ProductInfoVM> GetProducts(int count);
        public Task<ProductInfoVM> GetProductByID(int id);
        public Task<ProductInfoVM> UpdateProductInfo(int id, ProductUpdateDTO product, string requestingUsername);
        public Task<ProductInfoVM> PatchProductInfo(int id, ProductUpdateDTO product, string requestingUsername);
        public Task<bool> DeleteProduct(int id, string requestingUsername);
        public ProductInfoVM ProductModelToInfoVM(ProductModel model);
        public Task<bool> AddCommentToProduct(ProductCommentDTO commentDTO, string requestingUsername);
        public Task<bool> UpdateProductRate(UpdateProductRateDTO rateDTO, string requestingUsername);
        public bool IsProductAlready(string BrandName, string ProductTitle);
    }
}

