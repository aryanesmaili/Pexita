using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;

namespace Pexita.Data.Entities.Comments
{
    public class CommentsModel
    {
        public int ID { get; set; }
        public required string Text { get; set; }
        public required DateTime TimeCreated { get; set; }

        //Navigation Properties
        public  required int? UserID { get; set; }
        public  required UserModel? User { get; set; }
        public required int ProductID { get; set; }
        public required ProductModel Product { get; set; }
    }
}
