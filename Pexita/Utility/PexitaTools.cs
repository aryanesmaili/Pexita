using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Tags;
using System.Text;
using Pexita.Data.Entities.User;
using Pexita.Additionals;
using Pexita.Exceptions;
using Pexita.Services.Interfaces;

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

        public async Task<string> SaveProductImages(List<IFormFile> files, string identifier)
        {
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, $"/Images/{identifier}");

            if (!Directory.Exists(imagePath))
                Directory.CreateDirectory(imagePath);

            for (int i = 0; i < files.Count; i++)
            {
               
                string uniqueFileName = $"{identifier}_{i:03}{Path.GetExtension(files[i].FileName)}";
                string filePath = Path.Combine(imagePath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await files[i].CopyToAsync(stream);
                }
            }
            return imagePath;
        }
        public async Task<string> SaveProductImages(IFormFile file, string identifier)
        {
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, $"/Images/{identifier}");

            if (!Directory.Exists(imagePath))
                Directory.CreateDirectory(imagePath);

            string uniqueFileName = $"{identifier}_{DateTime.UtcNow.Ticks}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(imagePath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

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
    }
}
