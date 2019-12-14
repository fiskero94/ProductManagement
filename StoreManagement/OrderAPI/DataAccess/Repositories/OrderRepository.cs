using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using OrderAPI.DataAccess.Contexts;
using OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.DataAccess.Repositories
{
    public class OrderRepository : Repository<Order>
    {
        readonly OrderContext _context;

        public OrderRepository(OrderContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public override async Task<Order> GetAsync(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(order => order.OrderId == id);
        }

        public override async Task CreateAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(Order old, Order updated)
        {
            old.Date = updated.Date;
            old.ProductId = updated.ProductId;
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
