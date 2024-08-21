using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.Tags;
using Pexita.Data.Entities.User;

namespace Pexita.Services.Interfaces
{
    public interface IPexitaTools
    {
        public string GenerateRandomPassword(int length);
        public double? GetRating(List<int>? Ratings);
        public Task<string> SaveEntityImages(IFormFile file, string identifier, bool isUpdate = false);
        public Task<string> SaveEntityImages(List<IFormFile> files, string identifier, bool isUpdate = false);
        public Task<List<TagModel>> StringToTags(string Tag, ProductModel product);
        public Task<List<Address>> ValidateAddresses(int UserID, List<Address> AddressesDTO);
        public bool PictureFileValidation(IFormFile file, int MaxSizeMB);
        public Task<ProductModel> AuthorizeProductAccessAsync(int id, string Username);
        public Task<BrandModel> AuthorizeProductCreationAsync(int targetBrand, string reqUser);
        public Task<BrandModel> AuthorizeBrandAccessAsync(int id, string Username);
        public Task<UserModel> AuthorizeUserAccessAsync(int userID, string Username);
        public Task<ShoppingCartModel> AuthorizeCartAccess(int cartID, string reqUsername);
        public Task<UserModel> AuthorizeCartCreation(int UserID, string reqUsername);
        public string GenerateJWToken(string Username, string Role, string Email);
        public string GenerateRefreshToken();
        public bool IsEmail(string input);
    }
}