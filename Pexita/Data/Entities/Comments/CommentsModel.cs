using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;

namespace Pexita.Data.Entities.Comments
{
    public class CommentsModel
    {
        public int ID { get; set; }
        public required string Text { get; set; }
        public DateTime DateCreated { get; set; }

        //Navigation Properties
        public int? UserID { get; set; }
        public required UserModel User { get; set; }
        public int ProductID { get; set; }
        public required ProductModel Product { get; set; }
    }

    public class CommentsDTO
    {
        public int ID { get; set; }
        public required string Text { get; set; }
        public DateTime DateCreated { get; set; }

        //Navigation Properties
        public int? UserID { get; set; }
        public required UserInfoDTO User { get; set; }
        public int ProductID { get; set; }
        public required ProductInfoDTO Product { get; set; }
    }
}
