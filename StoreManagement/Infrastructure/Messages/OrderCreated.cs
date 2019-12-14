using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Messages
{
    public class OrderCreated
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public DateTime Date { get; set; }
    }
}
