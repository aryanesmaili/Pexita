using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Tags;

namespace Pexita.Data.Entities.Products
{
    public class ProductCreateVM
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public string Brand { get; set; }
        public double? Rate { get; set; }
        public List<IFormFile> ProductPics { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string Tags { get; set; }
        public string? Colors { get; set; }
    }
    public class ProductInfoVM
    {
        public required int ID { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public required BrandInfoVM Brand { get; set; }
        public double? Rate { get; set; }
        public required List<TagInfoVM> Tags { get; set; }
        public required string ProductPics { get; set; }
        public required bool IsAvailable { get; set; }
        public required DateTime DateCreated { get; set; }
    }
    public class ProductUpdateVM
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public required string Brand { get; set; }
        public double? Rate { get; set; }
        public required List<IFormFile> ProductPics { get; set; }
        public required bool IsAvailable { get; set; } = true;
        public required string Tags { get; set; }
        public string? Colors { get; set; }

    }
}
