using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDBContext _Context;
        private readonly IPexitaTools _pexitaTools;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public BrandService(AppDBContext Context, IPexitaTools PexitaTools, IMapper Mapper, IUserService userService)
        {
            _Context = Context;
            _pexitaTools = PexitaTools;
            _mapper = Mapper;
            _userService = userService;
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
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public List<BrandInfoVM> GetBrands()
        {
            try
            {
                List<BrandInfoVM> list = _Context.Brands
                    .Include(b => b.Products)!.ThenInclude(p => p.Comments)
                    .Include(b => b.Products)!.ThenInclude(p => p.Tags)
                    .AsNoTracking()
                    .Select(BrandModelToInfo)
                    .ToList();
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
                List<BrandInfoVM> brands = _Context.Brands
                    .Include(b => b.Products)!.ThenInclude(p => p.Comments)
                    .Include(b => b.Products)!.ThenInclude(p => p.Tags)
                    .AsNoTracking()
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
        public async Task<BrandInfoVM> GetBrandByID(int id)
        {
            return BrandModelToInfo(await _Context.Brands
                .Include(b => b.Products!).ThenInclude(pc => pc.Tags)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ID == id) ?? throw new NotFoundException());
        }

        public async Task<BrandModel> GetBrandByName(string name)
        {
            return await _Context.Brands.FirstOrDefaultAsync(x => x.Name == name) ?? throw new NotFoundException();
        }

        public async Task<BrandInfoVM> UpdateBrandInfo(int id, BrandUpdateVM model, string requestingUsername)
        {
            try
            {
                UserModel user = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
                bool isAdmin = user.Role == "admin";
                if (!isAdmin || user.Username != requestingUsername)
                {
                    throw new NotAuthorizedException();
                }
                var brand = await _Context.Brands.FirstOrDefaultAsync(x => x.ID == id) ?? throw new NotFoundException();

                _mapper.Map(model, brand);
                await _Context.SaveChangesAsync();

                return BrandModelToInfo(brand);

            }
            catch (ValidationException e)
            {

                throw new ValidationException(e.Message);
            }
        }
        public async Task<bool> RemoveBrand(int id, string requestingUsername)
        {
            try
            {
                UserModel user = await _Context.Users.SingleAsync(x => x.Username == requestingUsername);
                bool isAdmin = user.Role == "admin";
                if (!isAdmin || user.Username != requestingUsername)
                {
                    throw new NotAuthorizedException();
                }

                var brand = await _Context.Brands.FirstOrDefaultAsync(x => x.ID == id) ?? throw new NotFoundException();
                _Context.Remove(brand);
                await _Context.SaveChangesAsync();
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

        public async Task<bool> IsBrand(int id)
        {
            return await _Context.Brands.FirstOrDefaultAsync(x => x.ID == id) != null;
        }

        public async Task<bool> IsBrand(string BrandName)
        {
            return await _Context.Brands.FirstOrDefaultAsync(x => x.Name == BrandName) != null;
        }
    }
}
