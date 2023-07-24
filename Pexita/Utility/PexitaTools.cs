using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Tags;

namespace Pexita.Utility
{
    public class PexitaTools
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDBContext _Context;
        public PexitaTools(IWebHostEnvironment webHostEnvironment, AppDBContext Context)
        {
            _webHostEnvironment = webHostEnvironment;
            _Context = Context;
        }
        public async Task<string> SaveProductImages(List<IFormFile> files, string identifier)
        {
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, $"/Images/{identifier}");
            string[] allowedTypes = new[] { "image/jpeg", "image/png" };

            if (!Directory.Exists(imagePath))
                Directory.CreateDirectory(imagePath);

            for (int i = 0; i < files.Count; i++)
            {
                if (!allowedTypes.Contains(files[i].ContentType))
                    throw new FormatException($"Format Error occured while saving {files[i]},{files[i].FileName}");

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
            string[] allowedTypes = new[] { "image/jpeg", "image/png" };

            if (!Directory.Exists(imagePath))
                Directory.CreateDirectory(imagePath);
            if (!allowedTypes.Contains(file.ContentType))
                throw new FormatException($"Format Error occured while saving {file},{file.FileName}");

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
                if (_Context.Tags.FirstOrDefault(t => t.Title == tag) == null)
                    _Context.Tags.Add(new TagModel() { Title = tag });

                TagModel t = _Context.Tags.Single(t => t.Title == tag);
                t.TimesUsed++;
                res.Add(t);
            }
            return res;
        }
        public double GetRating(List<double> Ratings) => Ratings.Average();

    }
}
