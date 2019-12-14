using System.Threading.Tasks;
using Infrastructure.DataAccess;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;

namespace ProductAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly Repository<Product> _productRepository;
        private readonly RabbitMQMessenger _messenger;

        public ProductController(Repository<Product> productRepository, RabbitMQMessenger messenger)
        {
            _productRepository = productRepository;
            _messenger = messenger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _productRepository.GetAllAsync());
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> GetAsync(int id)
        {
            Product product = await _productRepository.GetAsync(id);

            if (product == null)
            {
                return NotFound("The product could not be found.");
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product is null.");
            }

            await _productRepository.CreateAsync(product);
            await _messenger.PublishAsync("ProductCreated", product);

            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product is null.");
            }

            Product productToUpdate = await _productRepository.GetAsync(id);
            if (productToUpdate == null)
            {
                return NotFound("The product could not be found.");
            }

            await _productRepository.UpdateAsync(productToUpdate, product);
            await _messenger.PublishAsync("ProductUpdated", product);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Product product = await _productRepository.GetAsync(id);

            if (product == null)
            {
                return NotFound("The product could not be found.");
            }

            await _productRepository.DeleteAsync(product);
            await _messenger.PublishAsync("ProductDeleted", product);

            return NoContent();
        }
    }
}