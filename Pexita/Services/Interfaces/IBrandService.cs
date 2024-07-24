using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.User;

namespace Pexita.Services.Interfaces
{
    public interface IBrandService
    {
        public Task<bool> AddBrand(BrandCreateVM createVM);
        public Task<string> Login(UserLoginVM loginInfo);
        public List<BrandInfoVM> GetBrands();
        public List<BrandInfoVM> GetBrands(int count);
        public Task<BrandInfoVM> GetBrandByID(int id);
        public Task<BrandModel> GetBrandByName(string name);
        public Task<BrandInfoVM> UpdateBrandInfo(int id, BrandUpdateVM model, string requestingUsername);
        public Task<bool> RemoveBrand(int id, string requestingUsername);
        public BrandInfoVM BrandModelToInfo(BrandModel model);
        public bool IsBrand(int id);
        public bool IsBrand(string BrandName);
    }
}
