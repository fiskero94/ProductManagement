using FinalProductManager_ForSureThisTime.Model;
using Microsoft.EntityFrameworkCore;

namespace FinalProductManager_ForSureThisTime.DBContexts
{
    public class ProductContext : DbContext
    {
        public static string ConnectionString = "Data Source=172.22.146.161,1433;Database=ProductsDB;User ID=ayis1;Password=bgf39cvr";

        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Electronics",
                    Description = "Electronic Items",
                },
                new Category
                {
                    Id = 2,
                    Name = "Clothes",
                    Description = "Dresses",
                },
                new Category
                {
                    Id = 3,
                    Name = "Grocery",
                    Description = "Grocery Items",
                }
            );
        }

    }
}