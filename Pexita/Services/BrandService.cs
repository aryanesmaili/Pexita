using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pexita.Data;
using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pexita.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDBContext _Context;
        private readonly IPexitaTools _pexitaTools;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;

        public BrandService(AppDBContext Context, IPexitaTools PexitaTools, IMapper Mapper, IUserService userService, JwtSettings jwtSettings)
        {
            _Context = Context;
            _pexitaTools = PexitaTools;
            _mapper = Mapper;
            _userService = userService;
            _jwtSettings = jwtSettings;
        }
        /// <summary>
        /// Registering a new brand.
        /// </summary>
        /// <param name="createDTO"> object containing the info needed to create a brand</param>
        /// <returns> True if successful, throws Exception on failure.</returns>
        /// <exception cref="ValidationException"> Happens When a field is not as expected.</exception>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AddBrand(BrandCreateVM createDTO)
        {
            try
            {
                if (await _Context.Brands.AnyAsync(u => u.Username == createDTO.Username)) // check if that user already exists.
                    return false;

                BrandModel Brand = _mapper.Map<BrandModel>(createDTO); // creating an object that matches our DB table.
                Brand.Password = BCrypt.Net.BCrypt.HashPassword(Brand.Password); // Hashing user's password to ensure security
                await _Context.Brands.AddAsync(Brand); // adding the user to Database
                await _Context.SaveChangesAsync(); // saving changes to Database 
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
        /// <summary>
        /// the function that validates user's input with database and returns JWT token if successful.
        /// </summary>
        /// <param name="userLoginVM">info that user has entered in our form.</param>
        /// <returns>string containing JWT token if successful.</returns>
        /// <exception cref="NotFoundException">if the user does not exist.</exception>
        /// <exception cref="NotAuthorizedException"></exception>
        public async Task<string> Login(UserLoginVM userLoginVM)
        {
            BrandModel? brand = null;

            if (!string.IsNullOrEmpty(userLoginVM.UserName)) // we find the user based on their username if they've entered username.
                brand = await _Context.Brands.FirstOrDefaultAsync(u => u.Username == userLoginVM.UserName) ?? throw new NotFoundException();

            else if (string.IsNullOrEmpty(userLoginVM.Email)) // we find the user based on their email if they've entered email.
                brand = await _Context.Brands.FirstOrDefaultAsync(u => u.Email == userLoginVM.Email) ?? throw new NotFoundException();

            if (brand == null && !BCrypt.Net.BCrypt.Verify(userLoginVM.Password, brand?.Password)) // verifying password with its hash in database.
            {
                throw new NotAuthorizedException("Username or Password is not correct");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey); // getting the encryption key from app settings.

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] // Defining the information being carried by the token.
                {
                    new Claim(ClaimTypes.Name, brand.Username),
                    new Claim(ClaimTypes.Role, "Brand"),
                    new Claim(ClaimTypes.Email, brand.Email),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes), // defining when the token is going to expire.
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        /// <summary>
        /// Get the list of all brands along with their products, tags and comments of each product.
        /// </summary>
        /// <returns>List of all brands along with their products, tags and comments of each product. </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
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
        /// <summary>
        /// Getting a set amount of brands from database along their products, tags and comments of each product.
        /// </summary>
        /// <param name="count"> count of brands you want to get from the database.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>
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
        /// <summary>
        /// Getting a brand's info from its ID
        /// </summary>
        /// <param name="id">brand's id</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<BrandInfoVM> GetBrandByID(int id)
        {
            return BrandModelToInfo(await _Context.Brands
                .Include(b => b.Products!).ThenInclude(pc => pc.Tags)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ID == id) ?? throw new NotFoundException());
        }
        /// <summary>
        /// Getting a brand's info from its username.
        /// </summary>
        /// <param name="username">brand's username</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<BrandModel> GetBrandByName(string username)
        {
            return await _Context.Brands.AsNoTracking().FirstOrDefaultAsync(x => x.Name == username) ?? throw new NotFoundException();
        }
        /// <summary>
        /// fully updating a brand info in the database table.
        /// </summary>
        /// <param name="id">brand's id</param>
        /// <param name="model">object containing new info</param>
        /// <param name="requestingUsername"> the user requesting the change.</param>
        /// <returns>new brand info.</returns>
        /// <exception cref="NotAuthorizedException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="ValidationException"></exception>
        public async Task<BrandInfoVM> UpdateBrandInfo(int id, BrandUpdateVM model, string requestingUsername)
        {
            try
            {
                BrandModel brand = await _pexitaTools.AuthorizeBrandAccessAsync(id, requestingUsername);

                _mapper.Map(model, brand);
                await _Context.SaveChangesAsync();

                return BrandModelToInfo(brand);
            }
            catch (ValidationException e)
            {
                throw new ValidationException(e.Message);
            }
        }
        /// <summary>
        /// deletes a brand from the database.
        /// </summary>
        /// <param name="id">ID of the brand to be deleted.</param>
        /// <param name="requestingUsername">the user requesting the deletion.</param>
        /// <returns></returns>
        /// <exception cref="NotAuthorizedException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<bool> RemoveBrand(int id, string requestingUsername)
        {
            try
            {
                BrandModel brand = await _pexitaTools.AuthorizeBrandAccessAsync(id, requestingUsername);

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
        /// <summary>
        /// Converts a Database row into a VM able to be sent to frontend.
        /// </summary>
        /// <param name="model">the brand model to be converted.</param>
        /// <returns></returns>
        public BrandInfoVM BrandModelToInfo(BrandModel model)
        {
            return _mapper.Map(model, new BrandInfoVM());
        }
        /// <summary>
        /// checks whether a given id exists in brand table and is a brand.
        /// </summary>
        /// <param name="id">the id to be checked.</param>
        /// <returns>True if exists false otherwise.</returns>
        public bool IsBrand(int id)
        {
            return _Context.Brands.FirstOrDefault(x => x.ID == id) != null;
        }
        /// <summary>
        /// checks whether a given brand name exists in brand table and is a brand.
        /// </summary>
        /// <param name="BrandName">the name of the brand to be checked.</param>
        /// <returns>True if exists false otherwise.</returns>
        public bool IsBrand(string BrandName)
        {
            return _Context.Brands.FirstOrDefault(x => x.Name == BrandName) != null;
        }
    }
}
