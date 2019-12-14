using Microsoft.EntityFrameworkCore;
using OrderManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManager.DBContexts
{
    public class ProductContext : DbContext
    {
        public static string ConnectionString = "Data Source=172.22.146.161,1433;Database=OrdersDB;User ID=ayis1;Password=bgf39cvr";

        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}
