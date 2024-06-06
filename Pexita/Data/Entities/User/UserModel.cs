using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.ShoppingCart;

namespace Pexita.Data.Entities.User
{
    public class UserModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } = "user";
        public required string PhoneNumber {  get; set; } // TODO: add phone number to mapping 
        public List<Address> Addresses { get; set; }
        public  DateTime DateCreated { get; set; }

        //Navigation Properties
        public List<OrdersModel> Orders { get; set; }
        public List<ShoppingCartModel> ShoppingCarts { get; set; }
        public List<BrandNewsletterModel> BrandNewsletters { get; set; }
        public List<ProductNewsLetterModel> ProductNewsletters { get; set; }
        public List<CommentsModel> Comments { get; set; }
    }
    public class Address
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public UserModel User { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Text { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
    }
}
