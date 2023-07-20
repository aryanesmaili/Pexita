using Pexita.Data.Entities.Brands;

namespace Pexita.Services.Interfaces
{
    public interface IBrandService
    {
        public bool AddBrand(BrandCreateVM createVM);
        public List<BrandInfoVM> GetBrands();
        public List<BrandInfoVM> GetBrands(int count);
        public BrandModel GetBrandByID(int id);
        public BrandModel GetBrandByName(string name);
        public BrandInfoVM UpdateBrandInfo(int id, BrandInfoVM model);
        public bool RemoveBrand(int id);
        public BrandInfoVM BrandModelToInfo(BrandModel model);

    }
}
