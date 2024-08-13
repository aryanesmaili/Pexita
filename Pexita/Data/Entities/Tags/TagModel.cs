using Pexita.Data.Entities.Products;

namespace Pexita.Data.Entities.Tags
{
    public class TagModel
    {
        public int ID { get; set; }
        public required string Title { get; set; }
        public List<ProductModel>? Products { get; set; }
        public int TimesUsed { get; set; }
    }
}
