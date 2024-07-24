using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Products;

namespace Pexita.Data.Entities.Brands
{
    public class BrandModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? BrandPicURL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }

        // Navigation Properties
        public List<ProductModel>? Products { get; set; }
        public List<BrandOrder>? BrandOrders { get; set; }
        public List<BrandNewsletterModel>? BrandNewsLetters { get; set; }
        public List<ProductNewsLetterModel>? ProductNewsLetters { get; set; }

    }
    public class BrandOrder
    {
        public int BrandID { get; set; }
        public BrandModel Brand { get; set; }

        public int OrderID { get; set; }
        public OrdersModel Order { get; set; }
    }
}
