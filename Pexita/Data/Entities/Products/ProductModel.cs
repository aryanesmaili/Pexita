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
        public required string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public required bool IsAvailable { get; set; }
        public string? Colors { get; set; }
        public List<ProductRating> Rating { get; set; }
        public required string ProductPicsURL { get; set; }
        public required DateTime DateAdded { get; set; }

        //Navigation Properties
        public int BrandID { get; set; }
        public required BrandModel Brand { get; set; }
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
