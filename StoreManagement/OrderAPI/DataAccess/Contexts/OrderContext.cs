using Microsoft.EntityFrameworkCore;
using OrderAPI.Models;

namespace OrderAPI.DataAccess.Contexts
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
    }
}