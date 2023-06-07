namespace Pexita.Data.ViewModels
{
    public class ProductVM
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public string BrandName { get; set; }
        public bool IsPurchasedBefore { get; set; }
        public DateTime? DatePurchased { get; set; }
        public double? Rate { get; set; }
        public string Category { get; set; }
        public string ProductPicURL { get; set; }
    }
}
