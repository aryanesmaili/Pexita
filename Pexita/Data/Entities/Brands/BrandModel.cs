using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;

namespace Pexita.Data.Entities.Brands
{
    public class BrandModel
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? BrandPicURL { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }

        // Navigation Properties
        public List<ProductModel> Products { get; set; }
        public List<OrdersModel> Orders { get; set; }
        public List<BrandNewsletterModel> BrandNewsLetters { get; set; }
        public List<ProductNewsLetterModel> ProductNewsLetters { get; set; }

    }
}
