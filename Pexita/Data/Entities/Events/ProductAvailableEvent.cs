﻿using Microsoft.EntityFrameworkCore;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Data.Entities.Events
{
    /// <summary>
    /// an event object that is triggered when a certain product changes its state to available
    /// </summary>
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
        public async Task Handle(ProductAvailableEvent e)
        {
            var subscribers = _context.ProductNewsletters.AsNoTracking().Where(x => x.ProductID == e.ProductId)
                .Select(x => x.User)
                .ToList(); // Listing the subscribers to that product.
            string ProductName = (await _context.Products.FindAsync(e.ProductId) ?? throw new NotFoundException()).Title;

            string subject = $"Product {ProductName} is now available";
            string body = $"Product {ProductName} is now available for purchase";

            subscribers.ForEach(n => _emailService.SendEmail(n.Email, subject, body)); // sending email to each subscriber.
        }
    }

}
