using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Tags;
using System.Text;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;
using Microsoft.IdentityModel.Tokens;
using Pexita.Data.Entities.Products;
using Azure.Core;

namespace Pexita.Utility
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
        public bool PictureFileValidation(IFormFile file, int MaxSizeMB)
        {
            string[] allowedTypes = new[] { "image/jpeg", "image/png" };

            if (!allowedTypes.Contains(file.ContentType))
                return false;
            if (file.Length > MaxSizeMB * 1024 * 1024)
                return false;

            return true;
        }

        public async Task<string> SaveProductImages(List<IFormFile> files, string identifier, bool isUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Identifier cannot be null or empty.", nameof(identifier));

            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, $"Images/{identifier}");

            // For POST requests or when the directory doesn't exist
            if (!isUpdate || !Directory.Exists(imagePath))
            {
                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);

                return await SaveNewImages(files, imagePath, identifier);
            }

            // For PUT and PATCH requests
            return await UpdateExistingImages(files, imagePath, identifier);
        }
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

            return imagePath;
        }
        private static async Task<string> SaveNewImages(List<IFormFile> files, string imagePath, string identifier)
        {
            if (files == null || files.Count == 0)
                return imagePath;

            for (int i = 0; i < files.Count; i++)
            {
                string uniqueFileName = $"{identifier}_{i:000}{Path.GetExtension(files[i].FileName)}";
                string filePath = Path.Combine(imagePath, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await files[i].CopyToAsync(stream);
                }
            }

            return imagePath;
        }
        private static async Task<string> UpdateExistingImages(List<IFormFile> files, string imagePath, string identifier)
        {
            if (files == null || files.Count == 0)
                return imagePath;

            var existingFiles = Directory.GetFiles(imagePath);
            var existingFileNames = existingFiles.Select(Path.GetFileName).ToList();

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Length == 0) continue; // Skip empty files

                string fileName = $"{identifier}_{i:000}{Path.GetExtension(files[i].FileName)}";
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
        public List<Address> ValidateAddresses(int UserID, List<Address> VMAddresses)
        {

            try
            {
                HashSet<Address> addresses = new(VMAddresses);

                UserModel user = _Context.Users.Include(u => u.Addresses).Single(u => u.ID == UserID);

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

        public async Task<ProductModel> AuthorizeProductRequest(int id, string username)
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

            if (!isAdmin && !isOwner)
            {
                throw new NotAuthorizedException($"User is not authorized to modify product {id}");
            }
            return product;
        }
    }
}
