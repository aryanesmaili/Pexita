using Pexita.Data;
using Pexita.Data.Entities.Brands;
using Pexita.Services.Interfaces;

namespace Pexita.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDBContext _Context;
        public BrandService(AppDBContext Context)
        {
            _Context = Context;
        }

        public bool AddBrand()
        {
            throw new NotImplementedException();
        }

        public List<BrandModel> GetAllBrnds()
        {
            throw new NotImplementedException();
        }

        public BrandModel GetBrandByID(int id)
        {
            throw new NotImplementedException();
        }

        public BrandModel GetBrandByName(string name)
        {
            throw new NotImplementedException();
        }

        public bool RemoveBrand(int id)
        {
            throw new NotImplementedException();
        }

        public BrandInfoVM UpdateBrandInfo(int id, BrandInfoVM model)
        {
            throw new NotImplementedException();
        }
        public BrandInfoVM BrandModelToInfo(BrandModel model)
        {
            throw new NotImplementedException();
        }
    }
}
