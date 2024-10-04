using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pexita.Data;
using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.Tags;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Pexita.Services
{
    public class PexitaTools : IPexitaTools
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDBContext _Context;
        private readonly JwtSettings _jwtSettings;
        public PexitaTools(IWebHostEnvironment webHostEnvironment, AppDBContext Context, JwtSettings jwtSettings)
        {
            _webHostEnvironment = webHostEnvironment;
            _Context = Context;
            _jwtSettings = jwtSettings;
        }
        /// <summary>
        /// checks if a given picture file is valid.
        /// </summary>
        /// <param name="file">the file to be checked.</param>
        /// <param name="MaxSizeMB">max size a file can be in MB.</param>
        /// <returns></returns>
        public bool PictureFileValidation(IFormFile file, int MaxSizeMB)
        {
            string[] allowedTypes = ["image/jpeg", "image/png"];

            if (!allowedTypes.Contains(file.ContentType))
                return false;
            if (file.Length > MaxSizeMB * 1024 * 1024)
                return false;

            return true;
        }
        /// <summary>
        /// saves a list of given images.
        /// </summary>
        /// <param name="files">list of files.</param>
        /// <param name="identifier">identifier indicating where they should be saved at.</param>
        /// <param name="isUpdate">is this an update operation or a creation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<string> SaveEntityImages(List<IFormFile> files, string identifier, bool isUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Identifier cannot be null or empty.", nameof(identifier));

            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, $"Images/{identifier}");

            // this is for POST requests or when the directory doesn't exist
            if (!isUpdate || !Directory.Exists(imagePath))
            {
                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);

                return await SaveNewImages(files, imagePath);
            }

            // For PUT and PATCH requests
            return await UpdateExistingImages(files, imagePath);
        }
        /// <summary>
        /// Saves a single image file.
        /// </summary>
        /// <param name="file"> file to be saved.</param>
        /// <param name="identifier"> location where this will be saved.</param>
        /// <param name="isUpdate">whether this is a update or a creation operation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<string> SaveEntityImages(IFormFile file, string identifier, bool isUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Identifier cannot be null or empty.", nameof(identifier));

            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, $"Images/{identifier}");

            if (!Directory.Exists(imagePath))
                Directory.CreateDirectory(imagePath);

            // If it's an update and the file is empty, don't overwrite existing file
            if (isUpdate && (file == null || file.Length == 0))
                return imagePath;

            // Generate a unique filename
            string uniqueFileName = $"{identifier.Split("/")[1]}_{DateTime.UtcNow.Ticks}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(imagePath, uniqueFileName);

            // If it's an update, remove the old file (if it exists)
            if (isUpdate)
            {
                var existingFiles = Directory.GetFiles(imagePath);
                if (existingFiles.Length > 0)
                {
                    File.Delete(existingFiles[0]); // Assuming only one file per identifier
                }
            }

            // Save the new file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }
        /// <summary>
        /// saves a new image to disc in where you show.
        /// </summary>
        /// <param name="files">the list of files to be saved.</param>
        /// <param name="imagePath">location to save images at.</param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        private static async Task<string> SaveNewImages(List<IFormFile> files, string imagePath)
        {
            if (files == null || files.Count == 0)
                return imagePath;

            for (int i = 0; i < files.Count; i++)
            {
                string uniqueFileName = $"{imagePath.Split("/").Last()}_{i:000}{Path.GetExtension(files[i].FileName)}";
                string filePath = Path.Combine(imagePath, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await files[i].CopyToAsync(stream);
                }
            }

            return imagePath;
        }
        /// <summary>
        /// updates a list of existing images with the new images given to it.
        /// </summary>
        /// <param name="files">List of images</param>
        /// <param name="imagePath">path to save the images at.</param>
        /// <returns></returns>
        private static async Task<string> UpdateExistingImages(List<IFormFile> files, string imagePath)
        {
            if (files == null || files.Count == 0)
                return imagePath;

            var existingFiles = Directory.GetFiles(imagePath);
            var existingFileNames = existingFiles.Select(Path.GetFileName).ToList();

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Length == 0) continue; // Skip empty files

                string fileName = $"{imagePath.Split("/").Last()}_{i:000}{Path.GetExtension(files[i].FileName)}";
                string filePath = Path.Combine(imagePath, fileName);

                // If file exists and new file has content, overwrite
                if (existingFileNames.Contains(fileName))
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await files[i].CopyToAsync(stream);
                    }
                    existingFileNames.Remove(fileName);
                }
                // If file doesn't exist, create new
                else
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await files[i].CopyToAsync(stream);
                    }
                }
            }

            // Remaining files in existingFileNames were not updated, so we keep them

            return imagePath;
        }
        /// <summary>
        /// Used in Object creation and updates. Given a string of tags, returns a list of <see cref="TagModel"/> objects.
        /// </summary>
        /// <param name="tagString">A string containing a list of tags, separated by commas.</param>
        /// <returns>The list of <see cref="TagModel"/> objects.</returns>
        public async Task<List<TagModel>> StringToTags(string tagString, ProductModel product)
        {
            List<TagModel> result = [];
            if (string.IsNullOrEmpty(tagString))
            {
                return result;
            }

            product.Tags ??= [];

            var tags = tagString
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(tag => tag.Trim().ToLowerInvariant())
                .ToHashSet();

            var existingTags = await _Context.Tags
                .Where(t => tags.Contains(t.Title.ToLower()))
                .ToDictionaryAsync(t => t.Title.ToLower(), StringComparer.OrdinalIgnoreCase);

            foreach (var tag in tags)
            {
                if (existingTags.TryGetValue(tag, out TagModel? tagModel))
                {
                    if (!product.Tags.Any(x => x.ID == tagModel.ID))
                    {
                        tagModel.TimesUsed++;
                        tagModel.Products?.Add(product);
                        product.Tags.Add(tagModel);
                    }
                    result.Add(tagModel);
                }
                else
                {
                    TagModel newTag = new() { Title = tag, TimesUsed = 1, Products = [product] };
                    _Context.Tags.Add(newTag);
                    product.Tags.Add(newTag);
                    result.Add(newTag);
                }
            }
            await _Context.SaveChangesAsync();
            return result;
        }
        /// <summary>
        /// gets the average of list of ratings.
        /// </summary>
        /// <param name="Ratings">List of ratings.</param>
        /// <returns></returns>
        public double? GetRating(List<int>? Ratings)
        {
            if (Ratings.IsNullOrEmpty())
                return 0;
            return Ratings?.Average();
        }
        public string GenerateRandomPassword(int length)
        {
            Random random = new();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder passwordBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(0, chars.Length);
                char randomChar = chars[randomIndex];
                passwordBuilder.Append(randomChar);
            }

            return passwordBuilder.ToString();
        }
        /// <summary>
        /// Validates if an address
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="AddressesDTO"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<List<Address>> ValidateAddresses(int UserID, List<Address> AddressesDTO)
        {

            HashSet<Address> addresses = new(AddressesDTO);

            UserModel user = await _Context.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.ID == UserID) ?? throw new NotFoundException($"user {UserID} does not exist.");

            foreach (var address in addresses)
            {
                if (user.Addresses?.FirstOrDefault(a => a.ID == address.ID) == null)
                {
                    user.Addresses?.Add(address);
                }
            }
            await _Context.SaveChangesAsync();

            return user.Addresses?.ToList() ?? [];
        }
        /// <summary>
        /// checks if a product row can be accessed and modified by a certain user. the user can only make changes if they're an admin or owner of the record.
        /// </summary>
        /// <param name="id">id of the product that wants to be </param>
        /// <param name="requesterUsername">the user requesting to modify a product.</param>
        /// <returns>the product row to be edited.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="NotAuthorizedException"></exception>
        public async Task<ProductModel> AuthorizeProductAccessAsync(int id, string requesterUsername)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(requesterUsername);

            ProductModel productToAccess = _Context.Products
                .Include(p => p.Brand)
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .FirstOrDefault(p => p.ID == id)
                ?? throw new NotFoundException($"Entity {id} not found in products.");

            BrandModel? requester = await _Context.Brands // looking for the brand requesting the addition
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(u => u.Username == requesterUsername);
            bool isAdmin = false;
            if (requester == null)
            {
                var possibleAdmin = await _Context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Username == requesterUsername)
                    ?? throw new NotFoundException($"User {requesterUsername} not found in Users.");
                isAdmin = possibleAdmin.Role == "admin";
            }
            bool isOwner = requesterUsername == productToAccess.Brand.Username;

            if (!isAdmin && !isOwner) // only an admin or the owner of the record can modify it.
            {
                throw new NotAuthorizedException($"User {requesterUsername} is not authorized to Access or  modify product {id}");
            }
            return productToAccess;
        }
        /// <summary>
        /// checks if an incoming request is authorized to add a product to a brand's collection. the user can only make changes if they're an admin or owner of the record.
        /// </summary>
        /// <param name="targetBrandID">the brand the requester wants to add products to.</param>
        /// <param name="requesterUsername">the requester who wants to add a product</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="NotAuthorizedException"></exception>
        public async Task<BrandModel> AuthorizeProductCreationAsync(int targetBrandID, string requesterUsername)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(requesterUsername);

            BrandModel brandToBeAccessed = await _Context.Brands
                                                .FindAsync(targetBrandID)
                                                ?? throw new NotFoundException($"Brand {targetBrandID} not found");

            BrandModel? requester = await _Context.Brands // looking for the brand requesting the addition
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(u => u.Username == requesterUsername);

            bool isAdmin = false;
            if (requester == null) // then maybe it is an admin requesting the addition
            {
                var possibleAdmin = await _Context.Users
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(u => u.Username == requesterUsername)
                                                ?? throw new NotFoundException($"Username {requesterUsername} is neither an admin nor a brand.");
                isAdmin = possibleAdmin.Role == "admin";
            }

            bool isOwner = requesterUsername == brandToBeAccessed.Username;

            if (!isAdmin && !isOwner)
                throw new NotAuthorizedException($"User {requesterUsername} is not authorized to create products for brand {targetBrandID}");
            return brandToBeAccessed;
        }
        /// <summary>
        /// Checks if a username has enough authorization to modify a certain brand's record in database. the user can only make changes if they're an admin or owner of the record.
        /// </summary>
        /// <param name="id">Brand ID</param>
        /// <param name="username">username of the person requesting to access.</param>
        /// <returns>BrandModel to be modified.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="NotAuthorizedException"></exception>
        public async Task<BrandModel> AuthorizeBrandAccessAsync(int id, string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            BrandModel brand = await _Context.Brands
                .FindAsync(id)
                ?? throw new NotFoundException($"Entity {id} not found in Brands.");

            var reqUser = await _Context.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == username);

            bool isAdmin = reqUser?.Role == "admin";
            bool isOwner = false;
            BrandModel reqBrand;
            if (reqUser == null || !isAdmin)
            {
                reqBrand = _Context.Brands.AsNoTracking().FirstOrDefault(b => b.Username == username) ?? throw new NotFoundException($"Brand {username} not found in Brands."); ;
                isOwner = reqBrand.ID == brand.ID;
            }

            if (!isAdmin && !isOwner) // only an admin or the owner of the record can modify it.
            {
                throw new NotAuthorizedException($"User {username} is not authorized to modify brand {id}");
            }
            return brand;
        }
        /// <summary>
        /// Checks if a username as enough authorization to modify a user records' info.
        /// </summary>
        /// <param name="id"> id of the user record to be modified.</param>
        /// <param name="Username">Username of the user trying to modify.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="NotAuthorizedException"></exception>
        public async Task<UserModel> AuthorizeUserAccessAsync(int id, string Username)
        {
            if (string.IsNullOrEmpty(Username))
                throw new ArgumentNullException(nameof(Username));

            UserModel user = await _Context.Users.Include(a => a.Addresses).FirstOrDefaultAsync(u => u.ID == id) ?? throw new NotFoundException($"User {id} not found in Users Table.");
            var reqUser = await _Context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == Username) ?? throw new NotFoundException($"User {id} not found in Users Table.");

            bool isAdmin = reqUser.Role == "admin";
            bool isOwner = user.ID == reqUser.ID;

            if (!isAdmin && !isOwner)
            {
                throw new NotAuthorizedException($"User {Username} is not authorized to modify User {id}");
            }
            return user;
        }
        /// <summary>
        /// Generates a JWToken for a user based on their creds
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Role"></param>
        /// <param name="Email"></param>
        /// <returns> a string containing JWT token</returns>
        public string GenerateJWToken(string Username, string Role, string Email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, Username),
                    new Claim(ClaimTypes.Role, Role),
                    new Claim(ClaimTypes.Email, Email),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        /// <summary>
        /// Generates the refresh token needed for user to refresh their JWToken.
        /// </summary>
        /// <returns>a random string containing the new RefreshToken</returns>
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
        /// <summary>
        /// validates if a given string is an Email or not.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>True if it is an emailf false otherwise.</returns>
        public bool IsEmail(string input)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(input, emailPattern);
        }
        /// <summary>
        /// Checks if a username as the authority to access a certain DB record.
        /// </summary>
        /// <param name="cartID">ID of the cart record the username wants to access.</param>
        /// <param name="reqUsername">Username of the user requesting the access.</param>
        /// <returns>a <see cref="ShoppingCartModel"/> Database record.</returns>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="NotAuthorizedException"></exception>
        public async Task<ShoppingCartModel> AuthorizeCartAccess(int cartID, string reqUsername)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(reqUsername);

            ShoppingCartModel cartToAccess = await _Context.ShoppingCarts
                .Include(x => x.User)
                .Include(x => x.Order)
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.ID == cartID)
                ?? throw new NotFoundException($"Entity {cartID} not found in Carts.");

            UserModel requester = await _Context.Users
                .FirstOrDefaultAsync(x => x.Username == reqUsername)
                ?? throw new NotFoundException($"User {reqUsername} not found in Users.");

            bool isAdmin = requester.Role == "admin";
            bool isOwner = cartToAccess.UserID == requester.ID;
            if (!isAdmin && !isOwner)
            {
                throw new NotAuthorizedException($"User {reqUsername} is not authorized to modify or Access cart {cartID}");
            }
            return cartToAccess;
        }
        /// <summary>
        /// Checks if a username as the authority to create a DB record.
        /// </summary>
        /// <param name="cartID">ID of the cart record the username wants to create record.</param>
        /// <param name="reqUsername">Username of the user requesting the creation.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="NotAuthorizedException"></exception>
        public async Task<UserModel> AuthorizeCartCreation(int UserID, string reqUsername)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(reqUsername);

            UserModel userToBeAccessed = await _Context.Users.FindAsync(UserID) ?? throw new NotFoundException($"User {UserID} does not exist.");

            UserModel requester = await _Context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == reqUsername) 
                ?? throw new NotFoundException($"User {reqUsername} does not exist.");

            bool isAdmin = requester.Role == "admin";
            bool isOwner = requester.Username == userToBeAccessed.Username;
            if (!isAdmin && !isOwner)
            {
                throw new NotAuthorizedException($"User {reqUsername} does not have permission to Access or Modify User {UserID}");
            }
            return userToBeAccessed;
        }
    }
}
