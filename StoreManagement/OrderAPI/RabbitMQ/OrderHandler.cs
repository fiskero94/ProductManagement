using System.Threading.Tasks;
using Infrastructure.DataAccess;
using Infrastructure.Messages;
using Infrastructure.Messaging;
using Newtonsoft.Json.Linq;
using OrderAPI.Models;

namespace OrderAPI.RabbitMQ
{
    public class OrderHandler : IRabbitMQHandler
    {
        private readonly Repository<Product> _productRepository;

        public OrderHandler(Repository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task HandleAsync(string messageType, JObject message)
        {
            switch (messageType)
            {
                case "ProductCreated": await HandleAsync(message.ToObject<ProductCreated>()); break;
                case "ProductUpdated": await HandleAsync(message.ToObject<ProductUpdated>()); break;
                case "ProductDeleted": await HandleAsync(message.ToObject<ProductDeleted>()); break;
            }
        }

        private async Task HandleAsync(ProductCreated message)
        {
            await _productRepository.CreateAsync(new Product()
            {
                ProductId = message.ProductId,
                Name = message.Name,
                Price = message.Price,
                Stock = message.Stock
            });
        }

        private async Task HandleAsync(ProductUpdated message)
        {
            Product product = await _productRepository.GetAsync(message.ProductId);
            await _productRepository.UpdateAsync(product, new Product()
            {
                ProductId = message.ProductId,
                Name = message.Name,
                Price = message.Price,
                Stock = message.Stock
            });
        }

        private async Task HandleAsync(ProductDeleted message)
        {
            Product product = await _productRepository.GetAsync(message.ProductId);
            await _productRepository.DeleteAsync(product);
        }
    }
}