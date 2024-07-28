namespace Pexita.Services.Interfaces
{
    public interface INewsletterService
    {
        public Task AddProductNewsLetter(int UserID, int productid, string requestingUsername);
        public Task AddBrandNewProductNewsLetter(int UserID, int BrandID, string requestingUsername);
    }
}
