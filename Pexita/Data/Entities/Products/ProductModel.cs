using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.Tags;

namespace Pexita.Data.Entities.Products
{
    public class ProductModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public bool IsAvailable { get; set; }
        public string? Colors { get; set; }
        public List<ProductRating> Rating { get; set; }
        public string ProductPicsURL { get; set; }
        public DateTime DateAdded { get; set; }

        //Navigation Properties
        public int BrandID { get; set; }
        public BrandModel Brand { get; set; }
        public List<CommentsModel>? Comments { get; set; }
        public List<TagModel>? Tags { get; set; }
        public List<ProductNewsLetterModel>? NewsLetters { get; set; }
        // Add a collection of CartItems
        public List<CartItems>? CartItems { get; set; }
    }
    public class ProductRating
    {
        public int ID { get; set; }
        public int Rating { get; set; }
        public int ProductID { get; set; }
        public ProductModel Product { get; set; }
    }
}
