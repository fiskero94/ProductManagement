using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.DataAccess;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Models;
using OrderAPI.Models.ViewModels;

namespace OrderAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly Repository<Order> _orderRepository;
        private readonly Repository<Product> _productRepository;
        private readonly RabbitMQMessenger _messenger;

        public OrderController(Repository<Order> orderRepository, Repository<Product> productRepository, RabbitMQMessenger messenger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _messenger = messenger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _orderRepository.GetAllAsync());
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> GetAsync(int id)
        {
            Order order = await _orderRepository.GetAsync(id);

            if (order == null)
            {
                return NotFound("The order could not be found.");
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] OrderViewModel orderViewModel)
        {
            if (orderViewModel == null)
            {
                return BadRequest("Order is null.");
            }

            Product product = await _productRepository.GetAsync(orderViewModel.ProductId);
            if (product == null)
            {
                return NotFound("The product could not be found.");
            }

            if (product.Stock < 1)
            {
                return BadRequest("The product is not in stock.");
            }

            Order order = new Order()
            {
                Date = DateTime.Now,
                ProductId = product.ProductId
            };
            await _orderRepository.CreateAsync(order);
            await _messenger.PublishAsync("OrderCreated", order);

            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] OrderViewModel orderViewModel)
        {
            if (orderViewModel == null)
            {
                return BadRequest("Order is null.");
            }

            Order orderToUpdate = await _orderRepository.GetAsync(id);
            if (orderToUpdate == null)
            {
                return NotFound("The order could not be found.");
            }

            Product product = await _productRepository.GetAsync(orderViewModel.ProductId);
            if (product == null)
            {
                return NotFound("The product could not be found.");
            }

            Order order = new Order()
            {
                Date = DateTime.Now,
                ProductId = product.ProductId
            };
            await _orderRepository.UpdateAsync(orderToUpdate, order);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Order order = await _orderRepository.GetAsync(id);

            if (order == null)
            {
                return NotFound("The order could not be found.");
            }

            await _orderRepository.DeleteAsync(order);
            return NoContent();
        }
    }
}
