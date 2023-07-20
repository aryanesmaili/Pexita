namespace Pexita.Data.Entities.Brands
{
    public class BrandInfoVM
    {
        public required int ID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? BrandPicURL { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required DateTime DateCreated { get; set; }
    }
    public class BrandCreateVM
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required List<IFormFile> BrandPics { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}
