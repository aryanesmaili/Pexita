using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.User;

namespace Pexita.Services.Interfaces
{
    public interface IBrandService
    {
        public Task<BrandInfoDTO> Register(BrandCreateDTO createDTO);
        public Task<BrandInfoDTO> Login(LoginDTO loginInfo);
        public Task<BrandInfoDTO> ResetPassword(string loginInfo);
        public Task<BrandInfoDTO> ChangePassword(BrandInfoDTO brand, string newPassword, string ConfirmPassword, string requestingUsername);
        public Task<BrandInfoDTO> CheckResetCode(BrandInfoDTO brand, string Code);
        public Task<BrandInfoDTO> TokenRefresher(string token);
        public Task RevokeToken(string token);
        public List<BrandInfoDTO> GetBrands();
        public List<BrandInfoDTO> GetBrands(int count);
        public Task<BrandInfoDTO> GetBrandByID(int id);
        public Task<BrandModel> GetBrandByName(string name);
        public Task<BrandInfoDTO> UpdateBrandInfo(int id, BrandUpdateDTO model, string requestingUsername);
        public Task RemoveBrand(int id, string requestingUsername);
        public BrandInfoDTO BrandModelToInfo(BrandModel model);
        public BrandInfoDTO BrandModelToInfo(BrandModel mode, BrandRefreshTokenDTO refreshToken, string AccessToken);
        public bool IsBrand(int id);
        public bool IsBrand(string BrandName);
    }
}
