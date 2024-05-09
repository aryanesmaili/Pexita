using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.Tags;
using Pexita.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitTest.FakeServices
{
    internal class FakeBrandService : IBrandService
    {
        public bool AddBrand(BrandCreateVM createVM)
        {
            return true;
        }

        public BrandInfoVM BrandModelToInfo(BrandModel model)
        {
            return new BrandInfoVM
            {
                ID = 1,
                Name = "Example Brand",
                Description = "This is an example brand.",
                BrandPicURL = "https://example.com/brand-image.jpg",
                Username = "example_user",
                Email = "example@example.com",
                DateCreated = DateTime.UtcNow,
                Products = new List<ProductInfoVM>
        {
            new ProductInfoVM
            {
                ID = 101,
                Title = "Product 1",
                Description = "Description of Product 1",
                Price = 29.99,
                Quantity = 100,
                Rate = 4.5,
                Tags = new List<TagInfoVM>
                {
                    new TagInfoVM { ID = 1, Title = "Electronics" },
                    new TagInfoVM { ID = 2, Title = "Gadgets" }
                },
                ProductPics = "https://example.com/product1.jpg",
                IsAvailable = true,
                DateCreated = DateTime.UtcNow
            },
            new ProductInfoVM
            {
                ID = 102,
                Title = "Product 2",
                Description = "Description of Product 2",
                Price = 19.99,
                Quantity = 50,
                Rate = 3.8,
                Tags = new List<TagInfoVM>
                {
                    new TagInfoVM { ID = 3,Title = "Home" },
                    new TagInfoVM { ID = 4,Title = "Kitchen" }
                },
                ProductPics = "https://example.com/product2.jpg",
                IsAvailable = false,
                DateCreated = DateTime.UtcNow
            }
        }
            };
        }

        public BrandInfoVM GetBrandByID(int id)
        {
            throw new NotImplementedException();
        }

        public BrandModel GetBrandByName(string name)
        {
            return new BrandModel();
        }

        public List<BrandInfoVM> GetBrands()
        {
            throw new NotImplementedException();
        }

        public List<BrandInfoVM> GetBrands(int count)
        {
            throw new NotImplementedException();
        }

        public bool IsBrand(int id)
        {
            throw new NotImplementedException();
        }

        public bool IsBrand(string BrandName)
        {
            throw new NotImplementedException();
        }

        public bool RemoveBrand(int id)
        {
            throw new NotImplementedException();
        }

        public BrandInfoVM UpdateBrandInfo(int id, BrandUpdateVM model)
        {
            throw new NotImplementedException();
        }
    }
}
