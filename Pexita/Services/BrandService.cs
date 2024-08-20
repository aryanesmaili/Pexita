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

namespace Pexita.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDBContext _Context;
        private readonly IPexitaTools _pexitaTools;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailService _emailService;

        public BrandService(AppDBContext Context, IPexitaTools PexitaTools, IMapper Mapper, IUserService userService, JwtSettings jwtSettings, IEmailService emailService)
        {
            _Context = Context;
            _pexitaTools = PexitaTools;
            _mapper = Mapper;
            _userService = userService;
            _jwtSettings = jwtSettings;
            _emailService = emailService;
        }
        /// <summary>
        /// Registering a new brand.
        /// </summary>
        /// <param name="createDTO"> object containing the info needed to create a brand</param>
        /// <returns> True if successful, throws Exception on failure.</returns>
        /// <exception cref="ValidationException"> Happens When a field is not as expected.</exception>
        /// <exception cref="Exception"></exception>
        public async Task<BrandInfoDTO> Register(BrandCreateDTO createDTO)
        {
            BrandModel Brand = _mapper.Map<BrandModel>(createDTO); // creating an object that matches our DB table.
            Brand.Password = BCrypt.Net.BCrypt.HashPassword(Brand.Password); // Hashing user's password to ensure security
            await _Context.Brands.AddAsync(Brand); // adding the user to Database
            await _Context.SaveChangesAsync(); // saving changes to Database 
            return BrandModelToInfo(Brand);
        }
        /// <summary>
        /// the function that validates user's input with database and returns JWT token if successful.
        /// </summary>
        /// <param name="userLoginDTO">info that user has entered in our form.</param>
        /// <returns>string containing JWT token if successful.</returns>
        /// <exception cref="NotFoundException">if the user does not exist.</exception>
        /// <exception cref="NotAuthorizedException"></exception>
        public async Task<BrandInfoDTO> Login(LoginDTO userLoginDTO)
        {
            BrandModel? brand = null;

            if (_pexitaTools.IsEmail(userLoginDTO.Identifier))
                brand = await _Context.Brands.FirstOrDefaultAsync(u => u.Email == userLoginDTO.Identifier) ?? throw new NotFoundException($"No brand with email {userLoginDTO.Identifier} exists.");
            else
                brand = await _Context.Brands.FirstOrDefaultAsync(u => u.Username == userLoginDTO.Identifier) ?? throw new NotFoundException($"No brand with username {userLoginDTO.Identifier} exists.");

            if (brand == null || !BCrypt.Net.BCrypt.Verify(userLoginDTO.Password, brand?.Password)) // verifying password with its hash in database.
            {
                throw new ArgumentException("Username or Password is not correct");
            }
            BrandInfoDTO result = BrandModelToInfo(brand!);
            result.JWToken = _pexitaTools.GenerateJWToken(brand!.Username, "Brand", brand.Email);
            string rawRefreshToken = _pexitaTools.GenerateRefreshToken();
            BrandRefreshToken refreshToken = new()
            {
                Token = rawRefreshToken,
                Brand = brand,
                BrandID = brand.ID,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
            };
            _Context.BrandRefreshTokens.Add(refreshToken);
            await _Context.SaveChangesAsync();
            result.RefreshToken = _mapper.Map<BrandRefreshTokenDTO>(refreshToken);
            return result;
        }
        /// <summary>
        /// begins a Change password procedure for the user.
        /// </summary>
        /// <param name="loginInfo">user's input that can be either email or username.</param>
        /// <returns> a <see cref="BrandInfoDTO"/> object containing info about the brand.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<BrandInfoDTO> ResetPassword(string loginInfo)
        {
            if (loginInfo.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(loginInfo));
            BrandModel? brand;

            if (_pexitaTools.IsEmail(loginInfo)) // if the user has entered an email:
                brand = await _Context.Brands.FirstOrDefaultAsync(b => b.Email == loginInfo); // we search by email
            else // if it's not an email then the user has entered their username
                brand = await _Context.Brands.FirstOrDefaultAsync(u => u.Username == loginInfo); // we search by username

            if (brand == null) // if no user exists with that email/username:
                throw new NotFoundException($"Brand {loginInfo} does not exist");

            brand.ResetPasswordCode = _pexitaTools.GenerateRandomPassword(8); // we generate a reset password code for them,
            string Subject = "Pexita Authentication code";
            string Body = $"Your Authentication Code Is {brand.ResetPasswordCode}";

            _emailService.SendEmail(brand.Email, Subject, Body); // we send the code to the user.
            _Context.Update(brand);
            await _Context.SaveChangesAsync();
            return BrandModelToInfo(brand);
        }
        /// <summary>
        /// Changes a brand's password after making sure the input is valid.
        /// </summary>
        /// <param name="brandInfo"> <see cref="BrandInfoDTO"/> object containing info about the brand being edited.</param>
        /// <param name="NewPassword"></param>
        /// <param name="ConfirmPassword"></param>
        /// <param name="requestingUsername">the username requesting the change.</param>
        /// <returns><see cref="BrandInfoDTO"/> object containing information about the record we just edited.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<BrandInfoDTO> ChangePassword(BrandInfoDTO brandInfo, string NewPassword, string ConfirmPassword, string requestingUsername)
        {
            if (NewPassword.IsNullOrEmpty() || ConfirmPassword.IsNullOrEmpty())
                throw new ArgumentNullException("Password fields should not be empty");
            else if (NewPassword != ConfirmPassword)
                throw new ArgumentException($"Entered values {NewPassword} and {ConfirmPassword} Do not match.");

            BrandModel brand = await _pexitaTools.AuthorizeBrandAccessAsync(brandInfo.ID, requestingUsername); // checking if the user requesting the change has authorization to modify this record
            string HashedPassword = BCrypt.Net.BCrypt.HashPassword(NewPassword); // Hashing the password using SHA256
            brand.Password = HashedPassword;
            brand.ResetPasswordCode = null;
            _Context.Update(brand);
            await _Context.SaveChangesAsync();
            return BrandModelToInfo(brand, brandInfo.RefreshToken!, brandInfo.JWToken!);
        }
        /// <summary>
        /// checks if the given code matches the one in Database.
        /// </summary>
        /// <param name="brand">brand whom we want to edit.</param>
        /// <param name="Code">the ResetCode entered by user.</param>
        /// <returns> a <see cref="BrandInfoDTO"/> object containing tokens. the user is verified after this.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<BrandInfoDTO> CheckResetCode(BrandInfoDTO brand, string Code)
        {
            if (Code.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Code));

            BrandModel brandRec = await _Context.Brands.FirstOrDefaultAsync(u => u.ID == brand.ID)
                 ?? throw new NotFoundException($"Brand {brand.ID} Does not Exist");

            string ResetCode = brandRec.ResetPasswordCode ?? throw new ArgumentNullException("Reset Code");

            if (ResetCode != Code)
                throw new ArgumentException("Code is Wrong.");

            var result = BrandModelToInfo(brandRec);
            string token = _pexitaTools.GenerateJWToken(brandRec.Username, "Brand", brandRec.Email);
            string refToken = _pexitaTools.GenerateRefreshToken();

            BrandRefreshToken refreshToken = new()
            {
                Brand = brandRec,
                BrandID = brandRec.ID,
                Token = refToken,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
            };
            _Context.BrandRefreshTokens.Add(refreshToken);
            await _Context.SaveChangesAsync();

            result.RefreshToken = _mapper.Map<BrandRefreshTokenDTO>(refreshToken);
            result.JWToken = token;
            return result;
        }
        /// <summary>
        /// Generates a fresh JWToken for the user given the refreshToken.
        /// </summary>
        /// <param name="refreshToken">the string containing user's given refreshToken.</param>
        /// <returns>an object containing fresh JWToken.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<BrandInfoDTO> TokenRefresher(string refreshToken)
        {
            if (refreshToken.IsNullOrEmpty())
                throw new ArgumentNullException(refreshToken);

            BrandRefreshToken? currentRefreshToken = await _Context.BrandRefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (currentRefreshToken == null || !currentRefreshToken.IsActive)
                throw new NotFoundException($"token {refreshToken} is not valid.");
            BrandModel brand = await _Context.Brands.FindAsync(currentRefreshToken.BrandID) ?? throw new NotFoundException($"User {currentRefreshToken.BrandID} Does not exist");

            var result = BrandModelToInfo(brand);
            // Generating both new JWToken and RefreshToken
            var newRefreshTokenStr = _pexitaTools.GenerateRefreshToken();
            result.JWToken = _pexitaTools.GenerateJWToken(brand.Username, "Brand", brand.Email);

            // Revoking the current token that the user had.
            currentRefreshToken.Revoked = DateTime.UtcNow;
            _Context.BrandRefreshTokens.Update(currentRefreshToken);
            // Creating the new Refresh token object for the user.
            BrandRefreshToken newToken = new BrandRefreshToken()
            {
                Token = newRefreshTokenStr,
                Brand = brand,
                BrandID = brand.ID,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            _Context.BrandRefreshTokens.Add(newToken);
            result.RefreshToken = _mapper.Map<BrandRefreshTokenDTO>(newToken);

            await _Context.SaveChangesAsync();
            return result;
        }
        /// <summary>
        /// revokes a user's refresh token on their logout
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task RevokeToken(string token)
        {
            if (token == null)
                throw new ArgumentNullException(token);

            var tokenRecord = await _Context.BrandRefreshTokens.FirstOrDefaultAsync(t => t.Token == token) ?? throw new NotFoundException();
            if (tokenRecord != null && tokenRecord.IsActive)
            {
                tokenRecord.Revoked = DateTime.UtcNow;
                _Context.BrandRefreshTokens.Update(tokenRecord);
                await _Context.SaveChangesAsync();
                return;
            }
            throw new Exception("token either invalid or already inactive.");
        }
        /// <summary>
        /// Get the list of all brands along with their products, tags and comments of each product.
        /// </summary>
        /// <returns>List of all brands along with their products, tags and comments of each product. </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public List<BrandInfoDTO> GetBrands()
        {
            List<BrandInfoDTO>? list = _Context.Brands
                .Include(b => b.Products).ThenInclude(p => p.Comments)
                .Include(b => b.Products).ThenInclude(p => p.Tags)
                .AsNoTracking()
                .Select(BrandModelToInfo)
                .ToList();
            return list ?? [];
        }
        /// <summary>
        /// Getting a set amount of brands from database along their products, tags and comments of each product.
        /// </summary>
        /// <param name="count"> count of brands you want to get from the database.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>
        public List<BrandInfoDTO> GetBrands(int count)
        {
            List<BrandInfoDTO> brands = _Context.Brands
                .Include(b => b.Products).ThenInclude(p => p.Comments)
                .Include(b => b.Products).ThenInclude(p => p.Tags)
                .AsNoTracking()
                .Take(count).Select(BrandModelToInfo)
                .ToList();
            if (brands.Count < count || brands.IsNullOrEmpty())
                throw new IndexOutOfRangeException("Database either empty or the number entered is out of range.");
            return brands;
        }
        /// <summary>
        /// Getting a brand's info from its ID
        /// </summary>
        /// <param name="id">brand's id</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<BrandInfoDTO> GetBrandByID(int id)
        {
            return BrandModelToInfo(await _Context.Brands
                .Include(b => b.Products)?.ThenInclude(pc => pc.Tags)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ID == id) ?? throw new NotFoundException($"{id}"));
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
        /// <param name="newData">object containing new info</param>
        /// <param name="requestingUsername"> the user requesting the change.</param>
        /// <returns>new brand info.</returns>
        /// <exception cref="NotAuthorizedException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="ValidationException"></exception>
        public async Task<BrandInfoDTO> UpdateBrandInfo(int id, BrandUpdateDTO newData, string requestingUsername)
        {
            BrandModel brand = await _pexitaTools.AuthorizeBrandAccessAsync(id, requestingUsername);

            BrandModel newState = _mapper.Map(newData, brand);

            if (newData.BrandPic != null)
                newState.BrandPicURL = await _pexitaTools.SaveEntityImages(newData.BrandPic, $"Brands/{newData.Name}", true);

            _Context.Update(brand);
            await _Context.SaveChangesAsync();
            var result = BrandModelToInfo(brand);
            result.JWToken = _pexitaTools.GenerateJWToken(brand.Username, "Brand", brand.Email);
            return result;
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
        public async Task RemoveBrand(int id, string requestingUsername)
        {
            BrandModel brand = await _pexitaTools.AuthorizeBrandAccessAsync(id, requestingUsername);

            _Context.Remove(brand);
            await _Context.SaveChangesAsync();
        }
        /// <summary>
        /// Converts a Database row into a VM able to be sent to frontend.
        /// </summary>
        /// <param name="model">the brand model to be converted.</param>
        /// <returns></returns>
        public BrandInfoDTO BrandModelToInfo(BrandModel model)
        {
            var result = _mapper.Map<BrandInfoDTO>(model);
            return result;
        }
        /// <summary>
        /// Maps a BrandModel database record to a representable object.
        /// </summary>
        /// <param name="model">the database record.</param>
        /// <param name="refreshToken">RefreshToken of the Brand.</param>
        /// <param name="AccessToken">JWToken given to user to authenticate their requests.</param>
        /// <returns>a <see cref="BrandInfoDTO"/> object containing information.</returns>
        public BrandInfoDTO BrandModelToInfo(BrandModel model, BrandRefreshTokenDTO refreshToken, string AccessToken)
        {
            return _mapper.Map(model, new BrandInfoDTO() { RefreshToken = refreshToken, JWToken = AccessToken });
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