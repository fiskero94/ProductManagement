using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManager.Model;
using OrderManager.Repository;

namespace OrderManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var products = await _orderRepository.GetAllProductsAsync();
            return new OkObjectResult(products);
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> GetAsync(int id)
        {
            Order order = await _orderRepository.GetOrderByIDAsync(id);
            return new OkObjectResult(order);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Order order)
        {
            using (var scope = new TransactionScope())
            {
                await _orderRepository.InsertOrderAsync(order);
                scope.Complete();

                RabbitService rabbit = new RabbitService();
                rabbit.PublishProductOrdered(order.Id);

                return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
            }
        }
    }
}
