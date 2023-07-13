using Pexita.Data.Entities.Brands;

namespace Pexita.Data.Entities.Products
{
    public class ProductVM
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public required BrandModel Brand { get; set; }
        public bool IsPurchasedBefore { get; set; }
        public DateTime? DatePurchased { get; set; }
        public double? Rate { get; set; }
        public required string Category { get; set; }
        public required string ProductPicURL { get; set; }
        public required bool IsAvailable  { get; set; } = true;
    }
}
