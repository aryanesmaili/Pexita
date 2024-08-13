using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;

namespace Pexita.Data.Entities.Newsletter
{
    public class ProductNewsLetterModel
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public required ProductModel Product { get; set; }
        public int UserID { get; set; }
        public required UserModel User { get; set; }
        public DateTime SubscribedAt { get; set; }
    }
    public class ProductNewsLetterDTO
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public required ProductInfoDTO Product { get; set; }
        public int UserID { get; set; }
        public required UserInfoDTO User { get; set; }
        public DateTime SubscribedAt { get; set; }
    }
}
