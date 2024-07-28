using Microsoft.EntityFrameworkCore;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Products;
using Pexita.Services.Interfaces;

namespace Pexita.Data.Entities.Events
{
    /// <summary>
    /// an event that is thrown when a certain brand releases a new product.
    /// </summary>
    public class BrandNewProductEvent
    {
        /// <summary>
        /// ID of the brand that released the new product.
        /// </summary>
        public int BrandID { get; set; }
        /// <summary>
        /// Brand record that released the new product
        /// </summary>
        public required BrandModel Brand { get; set; }
        /// <summary>
        /// New product's ID.
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// the database record of the product newly released.
        /// </summary>
        public required ProductModel Product { get; set; }
    }
    /// <summary>
    /// The Handler for events of type <see cref="BrandNewProductEvent"/>
    /// </summary>
    public class BrandNewProductEventHandler
    {
        private readonly AppDBContext _context;
        private readonly IEmailService _emailService;

        public BrandNewProductEventHandler(IEmailService emailService, AppDBContext context)
        {
            _emailService = emailService;
            _context = context;
        }
        /// <summary>
        /// Handles Incoming <see cref="BrandNewProductEvent"/>s by sending email to subscribers.
        /// </summary>
        /// <param name="e"> "e" Standing for Event is the Event object to be handled.</param>
        public void Handle(BrandNewProductEvent e)
        {
            var subscribers = _context.BrandNewsletters.AsNoTracking().Where(u => u.BrandID == e.BrandID)
                .Select(u => u.User)
                .ToList();
            string subject = $"Brand {e.Brand.Name} New Release";
            string body = $"Brand {e.Brand.Name} has released a new Product named {e.Product.Title}";

            subscribers.ForEach(n => _emailService.SendEmail(n.Email, subject, body));
        }
    }
}
