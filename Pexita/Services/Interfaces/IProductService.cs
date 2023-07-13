using Pexita.Data.Entities.Products;
namespace Pexita.Services.Interfaces
{
    public interface IProductService
    {
        public bool AddProduct(ProductVM product);
        public List<ProductModel> GetAllProducts();
        public ProductModel GetProductByID(int id);
        public ProductModel UpdateProductInfo(int id, ProductVM product);
        public bool DeleteProduct(int id);
    }
}
