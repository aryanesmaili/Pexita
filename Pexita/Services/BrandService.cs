using AutoMapper;
using Pexita.Data;
using Pexita.Data.Entities.Brands;
using Pexita.Services.Interfaces;
using Pexita.Utility;

namespace Pexita.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDBContext _Context;
        private readonly PexitaTools _pexitaTools;
        private readonly IMapper _mapper;
        public BrandService(AppDBContext Context, PexitaTools PexitaTools, IMapper Mapper)
        {
            _Context = Context;
            _pexitaTools = PexitaTools;
            _mapper = Mapper;
        }
        // TODO: ADD-MIGRATION BECAUSE OF THE CHANGES YOU MADE TO THE DATABASE SCHEME
        public bool AddBrand(BrandCreateVM createVM)
        {
            if (createVM == null)
                throw new ArgumentNullException(nameof(createVM));

            try
            {
                string identifier = $"{createVM.Name}/{createVM.Name}";

                BrandModel Brand = _mapper.Map<BrandModel>(createVM);

                _Context.Brands.Add(Brand);
                _Context.SaveChanges();
                return true;
            }
            catch (FormatException e)
            {
                throw new FormatException(e.Message);
            }
        }

        public List<BrandInfoVM> GetBrands()
        {
            try
            {
                List<BrandInfoVM> list = _Context.Brands.Select(BrandModelToInfo).ToList();
                return list;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException($"Brands Table is null or Empty!");
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public List<BrandInfoVM> GetBrands(int count)
        {
            try
            {
                List<BrandInfoVM> brands = _Context.Brands.Take(count).Select(BrandModelToInfo).ToList();
                return brands;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException($"Brands Table is null or Empty!");
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
