﻿using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Products;

namespace Pexita.Data.Entities.Brands
{
    public class BrandModel
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? BrandPicURL { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public string? ResetPasswordCode { get; set; }

        // Navigation Properties
        public List<ProductModel>? Products { get; set; }
        public List<BrandOrder>? BrandOrders { get; set; }
        public List<BrandNewsletterModel>? BrandNewsLetters { get; set; }
        public List<ProductNewsLetterModel>? ProductNewsLetters { get; set; }
        public List<BrandRefreshToken>? BrandRefreshTokens { get; set; }

    }

    public class BrandOrder
    {
        public int BrandID { get; set; }
        public required BrandModel Brand { get; set; }

        public int OrderID { get; set; }
        public required OrdersModel Order { get; set; }
    }
}
