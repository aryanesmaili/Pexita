namespace Pexita.Data.Entities.Tags
{
    public class TagCreateVM
    {
        public required string Title { get; set; }

    }
    public class TagInfoVM
    {
        public required int ID { get; set; }
        public required string Title { get; set; }
        public int TimesUsed { get; set; }
    }
}
