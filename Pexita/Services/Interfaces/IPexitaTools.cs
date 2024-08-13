using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.Tags;
using Pexita.Data.Entities.User;

namespace Pexita.Services.Interfaces
{
    public interface IPexitaTools
    {
        string GenerateRandomPassword(int length);
        double? GetRating(List<int>? Ratings);
        Task<string> SaveEntityImages(IFormFile file, string identifier, bool isUpdate = false);
        Task<string> SaveEntityImages(List<IFormFile> files, string identifier, bool isUpdate = false);
        public Task<List<TagModel>> StringToTags(string Tag);
        Task<List<Address>> ValidateAddresses(int UserID, List<Address> AddressesDTO);
        public bool PictureFileValidation(IFormFile file, int MaxSizeMB);
        public Task<ProductModel> AuthorizeProductAccessAsync(int id, string Username);
        public Task AuthorizeProductCreationAsync(string targetBrand, string reqUser);
        public Task<BrandModel> AuthorizeBrandAccessAsync(int id, string Username);
        public Task<UserModel> AuthorizeUserAccessAsync(int userID, string Username);
        public string GenerateJWToken(string Username, string Role, string Email);
        public string GenerateRefreshToken();
        public bool IsEmail(string input);
    }
}