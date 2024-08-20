using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Tags;

namespace Pexita.Data.Entities.Products
{
    public class ProductCreateDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public required int Quantity { get; set; }
        public required int BrandID { get; set; }
        public List<IFormFile>? ProductPics { get; set; }
        public bool IsAvailable => Quantity > 0;
        public string? Tags { get; set; }
        public string? Colors { get; set; }
    }

    public class UpdateProductRateDTO
    {
        public int ProductID { get; set; }
        public int ProductRating { get; set; }
    }

    public class ProductInfoDTO
    {
        public int ID { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public required int Quantity { get; set; }
        public required int BrandID { get; set; }
        public double? Rate { get; set; }
        public List<ProductTagInfoDTO>? Tags { get; set; }
        public string? ProductPics { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime DateCreated { get; set; }
        public List<CommentsDTO>? Comments { get; set; }
    }
    public class ProductUpdateDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public required int BrandID { get; set; }
        public double? Rate { get; set; }
        public List<IFormFile>? ProductPics { get; set; }
        public bool IsAvailable => Quantity > 0;
        public string? Tags { get; set; }
        public string? Colors { get; set; }
    }
}
