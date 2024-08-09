using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Products;

namespace Pexita.Data.Entities.Brands
{
    public class BrandInfoVM
    {
        public int ID { get; set; }
        public string? JWToken { get; set; }
        public BrandRefreshToken? RefreshToken { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? BrandPicURL { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public DateTime DateCreated { get; set; }
        public List<ProductModel>? Products { get; set; }
    }

    public class BrandCreateDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? Brandpic { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string Email { get; set; }
    }

    public class BrandUpdateVM
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? BrandPic { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string Email { get; set; }
    }
}
