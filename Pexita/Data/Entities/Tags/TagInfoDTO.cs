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
        public int TimesUsed { get; set; }
    }
}
