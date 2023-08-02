using Pexita.Data.Entities.Tags;
using Pexita.Data.Entities.User;

namespace Pexita.Services.Interfaces
{
    public interface IPexitaTools
    {
        string GenerateRandomPassword(int length);
        double GetRating(List<int> Ratings);
        Task<string> SaveProductImages(IFormFile file, string identifier);
        Task<string> SaveProductImages(List<IFormFile> files, string identifier);
        List<TagModel> StringToTags(string Tag);
        List<Address> ValidateAddresses(int UserID, List<Address> VMAddresses);
    }
}