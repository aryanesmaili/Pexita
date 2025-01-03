﻿namespace Pexita.Data.Entities.Tags
{
    public class ProductTagInfoDTO
    {
        public required int ID { get; set; }
        public required string Title { get; set; }
    }
    public class TagInfoDTO
    {
        public required int ID { get; set; }
        public required string Title { get; set; }
        public List<int>? Products { get; set; }
        public int TimesUsed { get; set; }
    }
}
