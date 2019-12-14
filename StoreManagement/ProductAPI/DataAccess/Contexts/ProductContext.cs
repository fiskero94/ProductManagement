using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;

namespace ProductAPI.DataAccess.Contexts
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}