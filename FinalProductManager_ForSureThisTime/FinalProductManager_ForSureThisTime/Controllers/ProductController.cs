using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FinalProductManager_ForSureThisTime.Model;
using FinalProductManager_ForSureThisTime.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FinalProductManager_ForSureThisTime.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return new OkObjectResult(products);
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var product = await _productRepository.GetProductByIDAsync(id);
            return new OkObjectResult(product);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Product product)
        {
            using (var scope = new TransactionScope())
            {
                await _productRepository.InsertProductAsync(product);
                scope.Complete();
                new RabbitService().PublishProductCreated(product.Id.ToString() + " " + product.Stock.ToString());
                return CreatedAtAction(nameof(GetAllAsync), new { id = product.Id }, product);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product is null.");
            }

            Product productToUpdate = await _productRepository.GetProductByIDAsync(id);
            if (productToUpdate == null)
            {
                return NotFound("Product not found");
            }
            await _productRepository.UpdateProductAsync(productToUpdate, product);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _productRepository.DeleteProductAsync(id);
            return new OkResult();
        }
    }
}
