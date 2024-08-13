using Pexita.Data.Entities.Products;

namespace Pexita.Data.Entities.Tags
{
    public class TagCreateDTO
    {
        public required string Title { get; set; }

    }
    public class TagInfoDTO
    {
        public required int ID { get; set; }
        public required string Title { get; set; }
        public List<ProductInfoDTO>? Products { get; set; }
        public int TimesUsed { get; set; }
    }
}
