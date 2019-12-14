using Microsoft.EntityFrameworkCore;
using OrderManager.DBContexts;
using OrderManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManager.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _dbContext;

        public OrderRepository(OrderContext orderContext)
        {
            _dbContext = orderContext;
        }

        public async Task<IEnumerable<Order>> GetAllProductsAsync()
        {
            return await _dbContext.Orders.ToListAsync();
        }

        public async Task<Order> GetOrderByIDAsync(int OrderId)
        {
            return await _dbContext.Orders.FindAsync(OrderId);
        }

        public async Task InsertOrderAsync(Order order)
        {
            _dbContext.Add(order);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public static OrderRepository GetRepository()
        {
            DbContextOptionsBuilder<OrderContext> options = new DbContextOptionsBuilder<OrderContext>();
            options.UseSqlServer(OrderContext.ConnectionString);

            return new OrderRepository(new OrderContext(options.Options));
        }
    }
}
