namespace Pexita.Data.Models
{
    public class ProductModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public string BrandName { get; set; }
        public bool IsPurchasedBefore { get; set; }
        public DateTime? DatePurchased { get; set; }
        public double? Rate { get; set; }
        public string Category { get; set; }
        public string ProductPicURL { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
