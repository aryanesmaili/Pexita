using Pexita.Data.Models;
namespace Pexita.Data
{
    public class InitialData
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var ServiceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = ServiceScope.ServiceProvider.GetService<AppDBContext>();

                if (!context!.Products.Any())
                {
                    context.Products.AddRange(new ProductModel
                    {
                        Title = "Blue Jean",
                        Description = "Blue jean Made to Scare people",
                        IsPurchasedBefore = true,
                        DatePurchased = DateTime.Now.AddDays(-25),
                        Rate = 3.5,
                        Category = "Jean",
                        BrandName = "Gucci",
                        ProductPicURL = "https://",
                        DateAdded = DateTime.Now.AddDays(-50),
                    }, new ProductModel()
                    {
                        Title = "Red Sweather",
                        Description = "Red Sweather made to attract people",
                        IsPurchasedBefore = false,
                        Rate = 3.5,
                        Category = "Sweather",
                        BrandName = "Fendi",
                        ProductPicURL = "https://",
                        DateAdded = DateTime.Now.AddDays(-37),
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
