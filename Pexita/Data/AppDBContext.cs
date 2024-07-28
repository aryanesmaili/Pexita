using Microsoft.EntityFrameworkCore;
using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Newsletter;
using Pexita.Data.Entities.Orders;
using Pexita.Data.Entities.Payment;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.ShoppingCart;
using Pexita.Data.Entities.Tags;
using Pexita.Data.Entities.User;

namespace Pexita.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasMany(a => a.Addresses)
                .WithOne(u => u.User)
                .HasForeignKey(ui => ui.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BrandOrder>()
                .HasKey(bo => new { bo.BrandID, bo.OrderID });

            modelBuilder.Entity<BrandOrder>()
                .HasOne(bo => bo.Brand)
                .WithMany(b => b.BrandOrders)
                .HasForeignKey(bo => bo.BrandID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<BrandOrder>()
                .HasOne(bo => bo.Order)
                .WithMany(o => o.BrandOrders)
                .HasForeignKey(bo => bo.OrderID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandID);

            modelBuilder.Entity<ProductModel>()
                .HasMany(ci => ci.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductID);

            modelBuilder.Entity<ShoppingCartModel>()
                .HasMany(ci => ci.CartItems)
                .WithOne(sc => sc.ShoppingCart)
                .HasForeignKey(ci => ci.ShoppingCartID)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<ShoppingCartModel>()
                .HasOne(sc => sc.Order)
                .WithOne(o => o.ShoppingCart)
                .HasForeignKey<OrdersModel>(sci => sci.ShoppingCartID);

            modelBuilder.Entity<ProductModel>()
                .HasMany(c => c.Comments)
                .WithOne(p => p.Product)
                .HasForeignKey(pi => pi.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductModel>()
                .HasMany(t => t.Tags)
                .WithMany(p => p.Products);

            modelBuilder.Entity<ShoppingCartModel>()
                .HasMany(p => p.Payments)
                .WithOne(sc => sc.ShoppingCart)
                .HasForeignKey(sci => sci.ShoppingCartID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserModel>()
                .HasMany(c => c.Comments)
                .WithOne(u => u.User)
                .HasForeignKey(ui => ui.UserID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProductModel>()
                .HasMany(pnl => pnl.NewsLetters)
                .WithOne(p => p.Product)
                .HasForeignKey(pi => pi.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                .HasMany(pnl => pnl.ProductNewsletters)
                .WithOne(u => u.User)
                .HasForeignKey(ui => ui.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BrandModel>()
                .HasMany(bnl => bnl.BrandNewsLetters)
                .WithOne(p => p.Brand)
                .HasForeignKey(bi => bi.BrandID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                .HasMany(bnl => bnl.BrandNewsletters)
                .WithOne(u => u.User)
                .HasForeignKey(ui => ui.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                .HasMany(o => o.Orders)
                .WithOne(u => u.User)
                .HasForeignKey(ui => ui.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductModel>()
                .HasMany(p => p.Rating)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductID);

            modelBuilder.Entity<UserModel>()
                .HasMany(a => a.Addresses)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                .HasMany(pnl => pnl.ProductNewsletters)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                .HasMany(bnl => bnl.BrandNewsletters)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                .HasMany(t => t.RefreshTokens)
                .WithOne(u => u.User)
                .HasForeignKey(user => user.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        public virtual DbSet<ProductModel> Products { get; set; }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<CommentsModel> Comments { get; set; }
        public DbSet<OrdersModel> Orders { get; set; }
        public DbSet<ProductNewsLetterModel> ProductNewsletters { get; set; }
        public DbSet<BrandNewsletterModel> BrandNewsletters { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<ShoppingCartModel> ShoppingCarts { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<BrandOrder> BrandOrder { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
