using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.User;
using Pexita.Services.Interfaces;

namespace Pexita.Data.Entities.Newsletter
{
    public class ProductNewsLetterModel
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public ProductModel? Product { get; set; }
        public int UserID { get; set; }
        public UserModel User { get; set; }
        public DateTime SubscribedAt { get; set; }
    }

    public class ProductAvailableEvent
    {
        /// <value>
        /// the product ID of the product that its state has changed
        /// </value>
        public int ProductId { get; set; }

        public ProductAvailableEvent(int productId)
        {
            ProductId = productId;
        }
    }
    public class ProductAvailableEventHandler
    {
        private readonly AppDBContext _context;
        private readonly IEmailService _emailService;

        public ProductAvailableEventHandler(AppDBContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        /// <summary>
        /// Handles <see cref="ProductAvailableEvent"/> by sending emails to its subscribers using <see cref="IEmailService"/>.
        /// </summary>
        /// <param name="e"> the event instance that is to be handled.</param>
        public void Handle(ProductAvailableEvent e)
        {
            var subscribers = _context.ProductNewsletters.Where(x => x.ProductID == e.ProductId)
                .Select(x => x.User)
                .ToList(); // Listing the subscribers to that product.

            string subject = $"Product {e.ProductId} is now available";
            string body = $"Product {e.ProductId} is now available for purchase";

            subscribers.ForEach(n => _emailService.SendEmail(n.Email, subject, body)); // sending email to each subscriber.
        }
    }
}
