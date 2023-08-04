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
        public int ID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public BrandInfoVM Brand { get; set; }
        public double? Rate { get; set; }
        public List<TagInfoVM> Tags { get; set; }
        public string ProductPics { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class ProductUpdateVM
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
}
