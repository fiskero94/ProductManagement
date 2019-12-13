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

        public IEnumerable<Order> GetProducts()
        {
            return _dbContext.Orders.ToList();
        }

        public Order GetOrderByID(int OrderId)
        {
            return _dbContext.Orders.Find(OrderId);
        }

        public void InsertOrder(Order order)
        {
            _dbContext.Add(order);
            Save();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
