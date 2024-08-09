using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;

namespace Pexita.Data.Entities.User
{
    public class UserCreateVM
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Email { get; set; }
    }

    public class UserLoginVM
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public required string Password { get; set; }
    }
    public class UserInfoVM
    {
        public required int ID { get; set; }
        public string? JWToken { get; set; }
        public UserRefreshToken? RefreshToken { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Address>? Addresses { get; set; }
        public DateTime DateCreated { get; set; }
        public List<OrdersModel>? Orders { get; set; }

        public List<BrandNewsletterModel>? BrandNewsletters { get; set; }
        public List<ProductNewsLetterModel>? ProductNewsletters { get; set; }
    }

    public class UserUpdateVM
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? Email { get; set; }

        public List<Address>? Addresses { get; set; }
    }
}
