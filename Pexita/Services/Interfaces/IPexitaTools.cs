using Pexita.Data;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.Tags;
using Pexita.Data.Entities.User;

namespace Pexita.Services.Interfaces
{
    public interface IPexitaTools
    {
        string GenerateRandomPassword(int length);
        double GetRating(List<int> Ratings);
        Task<string> SaveProductImages(IFormFile file, string identifier, bool isUpdate = false);
        Task<string> SaveProductImages(List<IFormFile> files, string identifier, bool isUpdate = false);
        List<TagModel> StringToTags(string Tag);
        List<Address> ValidateAddresses(int UserID, List<Address> VMAddresses);
        public bool PictureFileValidation(IFormFile file, int MaxSizeMB);
        public Task<ProductModel> AuthorizeProductRequest(int id, string Username);
    }
}