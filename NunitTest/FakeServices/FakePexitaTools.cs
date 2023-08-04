using Microsoft.AspNetCore.Http;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.Tags;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitTest.FakeServices
{
    internal class FakePexitaTools : IPexitaTools
    {
        public string GenerateRandomPassword(int length)
        {
            throw new NotImplementedException();
        }

        public double GetRating(List<int> Ratings)
        {
            return 10;
        }

        public bool PictureFileValidation(IFormFile file, int MaxSizeMB)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SaveProductImages(IFormFile file, string identifier)
        {
            // Simulate some async processing (in this case, none) and then return an empty string
            await Task.Delay(0);
            return " ";
        }


        public async Task<string> SaveProductImages(List<IFormFile> files, string identifier)
        {
            await Task.Delay(0);
            return " ";
        }

        public List<TagModel> StringToTags(string Tag)
        {
            return new List<TagModel>()
            {
                new TagModel
                {
                    Title = "Tag1",
                    Products = new List<ProductModel>(),
                    TimesUsed = 5
                },
                new TagModel
                {
                    Title = "Tag2",
                    Products = new List<ProductModel>(),
                    TimesUsed = 3
                },
            };
        }

        public List<Address> ValidateAddresses(int UserID, List<Address> VMAddresses)
        {
            throw new NotImplementedException();
        }
    }
}
