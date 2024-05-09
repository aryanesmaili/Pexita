using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;

namespace Pexita.Data.Entities.Comments
{
    public class CommentsModel
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }

        //Navigation Properties
        public int? UserID { get; set; }
        public UserModel? User { get; set; }
        public int ProductID { get; set; }
        public ProductModel Product { get; set; }
    }
}
