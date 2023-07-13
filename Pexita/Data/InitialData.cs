namespace Pexita.Data
{
    public class InitialData
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            /*using (var ServiceScope = applicationBuilder.ApplicationServices.CreateScope())
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
                        Brand = new BrandModel { Name="Gucci", BrandPicURL="", },
                        ProductPicURL = "https://",
                        DateAdded = DateTime.Now.AddDays(-50),
                    }, new ProductModel()
                    {
                        Title = "Red Sweather",
                        Description = "Red Sweather made to attract people",
                        IsPurchasedBefore = false,
                        Rate = 3.5,
                        Category = "Sweather",
                        Brand = new BrandModel { Name = "Fendi", BrandPicURL = "" },
                        ProductPicURL = "https://",
                        DateAdded = DateTime.Now.AddDays(-37),
                    });
                    context.SaveChanges();
                }
            }*/
        }
    }
}
