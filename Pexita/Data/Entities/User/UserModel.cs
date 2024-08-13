using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.ShoppingCart;

namespace Pexita.Data.Entities.User
{
    public class UserModel
    {
        public int ID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; } = "user";
        public required string PhoneNumber { get; set; }
        public List<Address>? Addresses { get; set; }
        public DateTime DateCreated { get; set; }
        public string? ResetPasswordCode { get; set; }
        public string? ProfilePicURL { get; set; }

        //Navigation Properties
        public List<OrdersModel>? Orders { get; set; }
        public List<ShoppingCartModel>? ShoppingCarts { get; set; }
        public List<BrandNewsletterModel>? BrandNewsletters { get; set; }
        public List<ProductNewsLetterModel>? ProductNewsletters { get; set; }
        public List<CommentsModel>? Comments { get; set; }
        public List<UserRefreshToken>? RefreshTokens { get; set; }
    }
    public class Address
    {
        public required int ID { get; set; }
        public required int UserID { get; set; }
        public required UserModel User { get; set; }
        public required string Province { get; set; }
        public required string City { get; set; }
        public required string Text { get; set; }
        public required string PhoneNumber { get; set; }
        public required string PostalCode { get; set; }
    }
}
