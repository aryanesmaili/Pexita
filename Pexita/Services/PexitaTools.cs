using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.Tags;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;
using System.Text;

namespace Pexita.Services
{
    public class PexitaTools : IPexitaTools
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDBContext _Context;
        public PexitaTools(IWebHostEnvironment webHostEnvironment, AppDBContext Context)
        {
            _webHostEnvironment = webHostEnvironment;
            _Context = Context;
        }
        /// <summary>
        /// checks if a given picture file is valid.
        /// </summary>
        /// <param name="file">the file to be checked.</param>
        /// <param name="MaxSizeMB">max size a file can be in MB.</param>
        /// <returns></returns>
        public bool PictureFileValidation(IFormFile file, int MaxSizeMB)
        {
            string[] allowedTypes = new[] { "image/jpeg", "image/png" };

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
        /// <param name="file_save_folder">identifier indicating where they should be saved at.</param>
        /// <param name="isUpdate">is this an update operation or a creation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<string> SaveProductImages(List<IFormFile> files, string file_save_folder, bool isUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(file_save_folder))
                throw new ArgumentException("Identifier cannot be null or empty.", nameof(file_save_folder));

            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, $"Images/{file_save_folder}");

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
        public async Task<string> SaveProductImages(IFormFile file, string identifier, bool isUpdate = false)
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
            string uniqueFileName = $"{identifier}_{DateTime.UtcNow.Ticks}{Path.GetExtension(file.FileName)}";
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
        /// given a string of tags, returns a list of <see cref="TagModel"/> objects.
        /// </summary>
        /// <param name="Tag">a string containing list of tags.</param>
        /// <returns>the list of tag model objects.</returns>
        public List<TagModel> StringToTags(string Tag)
        {
            if (string.IsNullOrEmpty(Tag))
            {
                return new List<TagModel>();
            }
            var tags = Tag.Split(',');
            List<TagModel> res = new();

            foreach (string tag in tags)
            {
                TagModel t = _Context.Tags.Single(t => t.Title == tag);
                t.TimesUsed++;
                res.Add(t);
            }
            return res;
        }
        /// <summary>
        /// gets the average of list of ratings.
        /// </summary>
        /// <param name="Ratings">List of ratings.</param>
        /// <returns></returns>
        public double GetRating(List<int> Ratings) => Ratings.Average();
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
        /// <param name="VMAddresses"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<List<Address>> ValidateAddresses(int UserID, List<Address> VMAddresses)
        {

            try
            {
                HashSet<Address> addresses = new(VMAddresses);

                UserModel user = await _Context.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.ID == UserID);

                foreach (var address in addresses)
                {
                    if (user.Addresses.FirstOrDefault(a => a.ID == address.ID) == null)
                    {
                        user.Addresses.Add(address);
                    }
                }
                _Context.SaveChanges();

                return user.Addresses.ToList();
            }
            catch (InvalidOperationException)
            {
                throw new NotFoundException($"User With ID:{UserID} Not Found");
            }
        }
        /// <summary>
        /// checks if a product row can be accessed and modified by a certain user. the user can only make changes if they're an admin or owner of the record.
        /// </summary>
        /// <param name="id">id of the product that wants to be </param>
        /// <param name="username">the user requesting to modify a product.</param>
        /// <returns>the product row to be edited.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="NotAuthorizedException"></exception>
        public async Task<ProductModel> AuthorizeProductAccessAsync(int id, string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            ProductModel product = _Context.Products
                .Include(p => p.Brand)
                .Include(p => p.Comments)
                .Include(p => p.Tags)
                .FirstOrDefault(p => p.ID == id)
                ?? throw new NotFoundException($"Entity {id} not found in products.");

            var reqUser = await _Context.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == username)
                ?? throw new NotFoundException($"User {username} not found in Users.");

            bool isAdmin = reqUser.Role == "admin";
            bool isOwner = reqUser.ID == product.Brand.ID;

            if (!isAdmin && !isOwner) // only an admin or the owner of the record can modify it.
            {
                throw new NotAuthorizedException($"User {username} is not authorized to modify product {id}");
            }
            return product;
        }
        /// <summary>
        /// checks if an incoming request is authorized to add a product to a brand's collection. the user can only make changes if they're an admin or owner of the record.
        /// </summary>
        /// <param name="targetBrand">the brand the requester wants to add products to.</param>
        /// <param name="requesterUsername">the requester who wants to add a product</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="NotAuthorizedException"></exception>
        public async Task AuthorizeProductCreationAsync(string targetBrand, string requesterUsername)
        {
            if (string.IsNullOrEmpty(requesterUsername))
                throw new ArgumentNullException(nameof(requesterUsername));

            BrandModel brandToBeEdited = await _Context.Brands
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(b => b.Username == targetBrand)
                                                ?? throw new NotFoundException($"Brand {targetBrand} not found");

            var requester = await _Context.Brands
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(u => u.Username == requesterUsername);

            bool isAdmin = false;
            if (requester == null)
            {
                var possibleAdmin = await _Context.Users
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(u => u.Username == requesterUsername)
                                                ?? throw new NotFoundException($"Username {requesterUsername} is neither an admin nor a brand.");
                isAdmin = possibleAdmin.Role == "admin";
            }

            bool isOwner = requesterUsername == brandToBeEdited.Username;

            if (!isAdmin && !isOwner)
                throw new NotAuthorizedException($"User {requesterUsername} is not authorized to create products for brand {targetBrand}");
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
                .FirstOrDefaultAsync(x => x.Username == username)
                ?? throw new NotFoundException($"User {username} not found in Users.");

            bool isAdmin = reqUser.Role == "admin";
            bool isOwner = reqUser.ID == brand.ID;

            if (!isAdmin && !isOwner) // only an admin or the owner of the record can modify it.
            {
                throw new NotAuthorizedException($"User {username} is not authorized to modify brand {id}");
            }
            return brand;
        }
    }
}
