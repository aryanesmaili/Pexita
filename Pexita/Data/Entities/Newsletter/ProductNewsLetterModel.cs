using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;

namespace Pexita.Data.Entities.Newsletter
{
    public class ProductNewsLetterModel
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public ProductModel? Product { get; set; }
        public int UserID { get; set; }
        public UserModel User { get; set; }
    }
}
