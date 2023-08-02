using Pexita.Data.Entities.Brands;
using Pexita.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitTest.FakeServices
{
    internal class FakeBrandService : IBrandService
    {
        public bool AddBrand(BrandCreateVM createVM)
        {
            return true;
        }

        public BrandInfoVM BrandModelToInfo(BrandModel model)
        {
            throw new NotImplementedException();
        }

        public BrandInfoVM GetBrandByID(int id)
        {
            throw new NotImplementedException();
        }

        public BrandModel GetBrandByName(string name)
        {
            throw new NotImplementedException();
        }

        public List<BrandInfoVM> GetBrands()
        {
            throw new NotImplementedException();
        }

        public List<BrandInfoVM> GetBrands(int count)
        {
            throw new NotImplementedException();
        }

        public bool RemoveBrand(int id)
        {
            throw new NotImplementedException();
        }

        public BrandInfoVM UpdateBrandInfo(int id, BrandUpdateVM model)
        {
            throw new NotImplementedException();
        }
    }
}
