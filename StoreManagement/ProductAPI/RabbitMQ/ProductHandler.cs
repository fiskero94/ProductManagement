using System.Threading.Tasks;
using Infrastructure.DataAccess;
using Infrastructure.Messages;
using Infrastructure.Messaging;
using Newtonsoft.Json.Linq;
using ProductAPI.Models;

namespace ProductAPI.RabbitMQ
{
    public class ProductHandler : IRabbitMQHandler
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRabbitMQMessenger _messenger;

        public ProductHandler(IRabbitMQMessenger messenger, IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
            _messenger = messenger;
        }

        public async Task HandleAsync(string messageType, JObject message)
        {
            switch (messageType)
            {
                case "OrderCreated": await HandleAsync(message.ToObject<OrderCreated>()); break;
            }
        }

        private async Task HandleAsync(OrderCreated message)
        {
            Product product = await _productRepository.GetAsync(message.ProductId);
            await _productRepository.UpdateAsync(product, new Product()
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock - 1
            });
            await _messenger.PublishAsync("ProductUpdated", product);
        }
    }
}