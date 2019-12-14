using System;

namespace OrderAPI.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public DateTime Date { get; set; }
    }
}