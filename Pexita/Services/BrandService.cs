using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pexita.Additionals.Exceptions;
using Pexita.Data;
using Pexita.Data.Entities.Brands;
using Pexita.Exceptions;
using Pexita.Services.Interfaces;
using Pexita.Utility;

namespace Pexita.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDBContext _Context;
        private readonly IPexitaTools _pexitaTools;
        private readonly IMapper _mapper;
        public BrandService(AppDBContext Context, IPexitaTools PexitaTools, IMapper Mapper)
        {
            _Context = Context;
            _pexitaTools = PexitaTools;
            _mapper = Mapper;
        }

        public bool AddBrand(BrandCreateVM createVM)
        {
            try
            {
                BrandModel Brand = _mapper.Map<BrandModel>(createVM);

                _Context.Brands.Add(Brand);
                _Context.SaveChanges();
                return true;
            }
            catch (ValidationException e)
            {
                throw new ValidationException(e.Message);
            }
           catch(Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public List<BrandInfoVM> GetBrands()
        {
            try
            {
                List<BrandInfoVM> list = _Context.Brands.Include(b => b.Products)!.ThenInclude(p => p.Comments).Include(b => b.Products)!.ThenInclude(p => p.Tags).AsNoTracking().Select(BrandModelToInfo).ToList();
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
        }
        public List<BrandInfoVM> GetBrands(int count)
        {
            try
            {
                List<BrandInfoVM> brands = _Context.Brands.Include(b => b.Products)!.ThenInclude(p => p.Comments).Include(b => b.Products)!.ThenInclude(p => p.Tags).AsNoTracking()
                    .Take(count).Select(BrandModelToInfo)
                    .ToList();
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
        public BrandInfoVM GetBrandByID(int id)
        {
            return BrandModelToInfo(_Context.Brands.Include(b => b.Products!).ThenInclude(pc => pc.Tags).AsNoTracking().FirstOrDefault(b => b.ID == id) ?? throw new NotFoundException());
        }

        public BrandModel GetBrandByName(string name)
        {
            return _Context.Brands.FirstOrDefault(x => x.Name == name) ?? throw new NotFoundException();
        }

        public BrandInfoVM UpdateBrandInfo(int id, BrandUpdateVM model)
        {
            var brand = _Context.Brands.FirstOrDefault(x => x.ID == id) ?? throw new NotFoundException();
            try
            {

                _mapper.Map(model, brand);
                _Context.SaveChanges();

                return BrandModelToInfo(brand);

            }
            catch (ValidationException e)
            {

                throw new ValidationException(e.Message);
            }
        }
        public bool RemoveBrand(int id)
        {
            try
            {
                var brand = _Context.Brands.FirstOrDefault(x => x.ID == id) ?? throw new NotFoundException();
                _Context.Remove(brand);
                _Context.SaveChanges();
                return true;
            }
            catch (NotFoundException)
            {
                throw new NotFoundException();
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        //TODO : remove throwing exceptions from services and just catch them in controller!
        public BrandInfoVM BrandModelToInfo(BrandModel model)
        {
            return _mapper.Map(model, new BrandInfoVM());
        }

        public bool IsBrand(int id)
        {
            return _Context.Brands.FirstOrDefault(x => x.ID == id) != null;
        }

        public bool IsBrand(string BrandName)
        {
            return _Context.Brands.FirstOrDefault(x => x.Name == BrandName) != null;
        }
    }
}
