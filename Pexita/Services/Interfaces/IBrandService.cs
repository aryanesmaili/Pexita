using Pexita.Data.Entities.Brands;

namespace Pexita.Services.Interfaces
{
    public interface IBrandService
    {
        public bool AddBrand();
        public List<BrandModel> GetAllBrnds();
        public BrandModel GetBrandByID(int id);
        public BrandModel GetBrandByName(string name);
        public BrandInfoVM UpdateBrandInfo(int id, BrandInfoVM model);
        public bool RemoveBrand(int id);
        public BrandInfoVM BrandModelToInfo(BrandModel model);

    }
}
