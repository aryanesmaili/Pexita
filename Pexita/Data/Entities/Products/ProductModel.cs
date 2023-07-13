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
        public int Quantity { get; set; }
        public required bool IsAvailable { get; set; }
        public string Colors { get; set; }
        public double? Rate { get; set; }
        public string ProductPicURL { get; set; }
        public DateTime DateAdded { get; set; }

        //Navigation Properties
        public int BrandID { get; set; }
        public BrandModel Brand { get; set; }
        public List<CommentsModel> Comments { get; set; }
        public List<TagsModel> Tags { get; set; }
        public List<ProductNewsLetterModel> NewsLetters { get; set; }
        // Add a collection of CartItems
        public List<CartItems> CartItems { get; set; }
    }
}
