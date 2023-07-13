using Pexita.Data.Entities.Products;

namespace Pexita.Data.Entities.Tags
{
    public class TagsModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public List<ProductModel> Products { get; set; }
        public int TimesUsed { get; set; }
    }
}
