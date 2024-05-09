using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Products;
namespace Pexita.Services.Interfaces
{
    public interface IProductService
    {
        public bool AddProduct(ProductCreateVM product);
        public List<ProductInfoVM> GetProducts();
        public List<ProductInfoVM> GetProducts(int count);
        public ProductInfoVM GetProductByID(int id);
        public ProductInfoVM UpdateProductInfo(int id, ProductUpdateVM product);
        public bool DeleteProduct(int id);
        public ProductInfoVM ProductModelToInfoVM(ProductModel model);
        public bool AddCommentToProduct(int ProductID, CommentsModel comment);
        public bool UpdateProductRate(int ProductID, int rate);
        public bool IsProductAlready(string BrandName, string ProductTitle);
    }
}
