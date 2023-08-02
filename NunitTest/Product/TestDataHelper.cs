using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.Tags;

namespace NunitTest.Product
{
    class TestDataHelper
    {
        public static List<ProductModel> GetSomeProducts()
        {
            return new List<ProductModel>
                {
                    new ProductModel
                    {
                        ID = 1,
                        Title = "Product 1",
                        Description = "Description of Product 1",
                        Price = 29.99,
                        Quantity = 100,
                        IsAvailable = true,
                        Colors = "Red, Blue",
                        Rating = new List<ProductRating>
                        {
                            new ProductRating { ID = 1, Rating = 4, ProductID = 1 },
                            new ProductRating { ID = 2, Rating = 5, ProductID = 1 }
                        },
                        ProductPicsURL = "https://example.com/product1.jpg",
                        DateAdded = DateTime.Now,
                        BrandID = 101, // Assuming BrandID for BrandModel with ID = 101
                        Brand = new BrandModel { ID = 101, Name = "Brand A" },
                        Comments = new List<CommentsModel>
                        {
                            new CommentsModel { ID = 1, Text = "Nice product!", ProductID = 1 },
                            new CommentsModel { ID = 2, Text = "Great quality!", ProductID = 1 }
                        },
                        Tags = new List<TagModel>
                        {
                            new TagModel { ID = 1, Title = "Electronics" },
                            new TagModel { ID = 2, Title = "Gadgets" }
                        },
                        NewsLetters = new List<ProductNewsLetterModel>
                        {
                            new ProductNewsLetterModel { ID = 1, ProductID = 1 },
                            new ProductNewsLetterModel { ID = 2, ProductID = 1 }
                        },
                        CartItems = new List<CartItems>
                        {
                            new CartItems { ID = 1, ProductID = 1, Count = 4 },
                            new CartItems { ID = 2, ProductID = 1, Count = 4 }
                        }
                    },
                    new ProductModel
                    {
                        ID = 2,
                        Title = "Product 2",
                        Description = "Description of Product 2",
                        Price = 19.99,
                        Quantity = 50,
                        IsAvailable = true,
                        Colors = "Green, Yellow",
                        Rating = new List<ProductRating>
                        {
                            new ProductRating { ID = 3, Rating = 3, ProductID = 2 },
                            new ProductRating { ID = 4, Rating = 4, ProductID = 2 }
                        },
                        ProductPicsURL = "https://example.com/product2.jpg",
                        DateAdded = DateTime.Now,
                        BrandID = 102, // Assuming BrandID for BrandModel with ID = 102
                        Brand = new BrandModel { ID = 102, Name = "Brand B" },
                        Comments = new List<CommentsModel>
                        {
                            new CommentsModel { ID = 3, Text = "Good product.", ProductID = 2 },
                            new CommentsModel { ID = 4, Text = "Nice color options.", ProductID = 2 }
                        },
                        Tags = new List<TagModel>
                        {
                            new TagModel { ID = 3, Title = "Fashion" },
                            new TagModel { ID = 4, Title = "Trendy" }
                        },
                        NewsLetters = new List<ProductNewsLetterModel>
                        {
                            new ProductNewsLetterModel { ID = 3, ProductID = 2 },
                            new ProductNewsLetterModel { ID = 4, ProductID = 2 }
                        },
                        CartItems = new List<CartItems>
                        {
                            new CartItems { ID = 3, Count = 3, ProductID = 2 },
                            new CartItems { ID = 4, Count = 2, ProductID = 2 }
                        }
                    }
                };
        }
    }
}
