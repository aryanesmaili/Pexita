using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.User;

namespace Pexita.Services.Interfaces
{
    public interface IBrandService
    {
        public Task<BrandInfoVM> Register(BrandCreateDTO createVM);
        public Task<BrandInfoVM> Login(UserLoginVM loginInfo);
        public Task<BrandInfoVM> ResetPassword(string loginInfo);
        public Task<BrandInfoVM> ChangePassword(BrandInfoVM brand, string newPassword, string ConfirmPassword, string requestingUsername);
        public Task<BrandInfoVM> CheckResetCode(BrandInfoVM brand, string Code);
        public Task<BrandInfoVM> TokenRefresher(string token);
        public Task RevokeToken(string token);
        public List<BrandInfoVM> GetBrands();
        public List<BrandInfoVM> GetBrands(int count);
        public Task<BrandInfoVM> GetBrandByID(int id);
        public Task<BrandModel> GetBrandByName(string name);
        public Task<BrandInfoVM> UpdateBrandInfo(int id, BrandUpdateVM model, string requestingUsername);
        public Task RemoveBrand(int id, string requestingUsername);
        public BrandInfoVM BrandModelToInfo(BrandModel model);
        public BrandInfoVM BrandModelToInfo(BrandModel mode, BrandRefreshToken refreshToken, string AccessToken);
        public bool IsBrand(int id);
        public bool IsBrand(string BrandName);
    }
}
