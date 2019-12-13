using OrderManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManager.Repository
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetProducts();
        Order GetOrderByID(int OrderId);
        void InsertOrder(Order order);
        void Save();
    }
}
