using Microsoft.IdentityModel.Tokens;
using Pexita.Data;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Services
{
    public class NewsletterService : INewsletterService
    {
        private readonly AppDBContext _Context;
        private readonly IUserService _userService;
        private readonly IPexitaTools _pexitaTools;
        private readonly IBrandService _brandService;


        public NewsletterService(IUserService userService, IPexitaTools pexitaTools, AppDBContext context, IBrandService brandService)
        {
            _userService = userService;
            _pexitaTools = pexitaTools;
            _Context = context;
            _brandService = brandService;
        }
        /// <summary>
        /// Adds a newsletter subscription for a given brand's product release to a user.
        /// </summary>
        /// <param name="UserID">the user we're adding subscription to.</param>
        /// <param name="BrandID">the brand that's going to release products.</param>
        /// <param name="requestingUsername">username of the user requesting this change.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task AddBrandNewProductNewsLetter(int UserID, int BrandID, string requestingUsername)
        {
            UserModel user = await _pexitaTools.AuthorizeUserAccessAsync(UserID, requestingUsername);
            BrandModel brand = await _Context.Brands.FindAsync(BrandID) ?? throw new NotFoundException($"brand {BrandID} not Found in brands.");
            BrandNewsletterModel bnl = new()
            {
                BrandID = BrandID,
                Brand = brand,
                SubscribedAt = DateTime.UtcNow,
                User = user,
                UserID = user.ID
            };
            if (user.BrandNewsletters.IsNullOrEmpty())
                user.BrandNewsletters = new List<BrandNewsletterModel>();

            user.BrandNewsletters?.Add(bnl);
            await _Context.SaveChangesAsync();
        }
        /// <summary>
        /// Adds a newsletter subscription for a change of stock status in a certain product to a user.
        /// </summary>
        /// <param name="UserID"> the user we're adding the subscription to.</param>
        /// <param name="productid">the product the user is subscribing to.</param>
        /// <param name="requestingUsername">username of the user requesting the change.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task AddProductNewsLetter(int UserID, int productid, string requestingUsername)
        {
            UserModel user = await _pexitaTools.AuthorizeUserAccessAsync(UserID, requestingUsername);
            ProductModel product = await _Context.Products.FindAsync(productid) ?? throw new NotFoundException($"Product {productid} was not found in products");
            ProductNewsLetterModel productNewsletter = new()
            {
                ProductID = productid,
                Product = product,
                UserID = UserID,
                User = user,
                SubscribedAt = DateTime.UtcNow,
            };
            if (user.ProductNewsletters.IsNullOrEmpty())
                user.ProductNewsletters = new List<ProductNewsLetterModel>();

            user.ProductNewsletters?.Add(productNewsletter);
            await _Context.SaveChangesAsync();
        }
    }
}
