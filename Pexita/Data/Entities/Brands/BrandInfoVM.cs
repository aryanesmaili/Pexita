using Pexita.Data.Entities.Products;
using System.ComponentModel.DataAnnotations;

namespace Pexita.Data.Entities.Brands
{
    public class BrandInfoVM
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
        public string? BrandPicURL { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public List<ProductInfoVM>? Products { get; set; }
    }

    public class BrandCreateVM
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public IFormFile BrandPic { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
    }

    public class BrandUpdateVM
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? BrandPic { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public required string Email { get; set; }
    }
}
