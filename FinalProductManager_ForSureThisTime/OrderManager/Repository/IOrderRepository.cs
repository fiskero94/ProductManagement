using OrderManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManager.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllProductsAsync();
        Task<Order> GetOrderByIDAsync(int OrderId);
        Task InsertOrderAsync(Order order);
        Task SaveAsync();
    }
}
