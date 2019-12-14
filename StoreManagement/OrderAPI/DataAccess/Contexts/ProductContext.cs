using Microsoft.EntityFrameworkCore;
using OrderAPI.Models;

namespace OrderAPI.DataAccess.Contexts
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}