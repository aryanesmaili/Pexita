using Microsoft.EntityFrameworkCore;
using Pexita.Data.Models;

namespace Pexita.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }
        public DbSet<ProductModel> Products { get; set; }
    }
}
