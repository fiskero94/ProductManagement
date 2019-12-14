using FinalProductManager_ForSureThisTime.Controllers;
using FinalProductManager_ForSureThisTime.DBContexts;
using FinalProductManager_ForSureThisTime.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FinalProductManager_ForSureThisTime
{
    public class RabbitService : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly IProductRepository _productRepository;

        public RabbitService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "172.22.146.161", UserName = "rabbitmquser", Password = "TisseMand1234" };

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("product.exchange", ExchangeType.Topic);
            _channel.QueueDeclare("product.queue.log", false, false, false, null);
            _channel.QueueBind("product.queue.log", "product.exchange", "product.queue.*", null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body);

                // handle the received message  
                await HandleMessageAsync(content, ea.BasicProperties.Type);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("product.queue.log", false, consumer);
            return Task.CompletedTask;
        }

        public void publishSomething(string message)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
            IBasicProperties properties = _channel.CreateBasicProperties();
            properties.Type = "ProductOrdered";
            _channel.BasicPublish("product.exchange", "product.queue.*", properties, bytes);
        }

        private async Task HandleMessageAsync(string content, string type)
        {

            switch (type)
            {
                case "ProductOrdered":
                    await HandleProductOrderedAsync(content);
                    break;
                default:
                    break;
            }
            _logger.LogInformation($"consumer received {content}");
        }

        private async Task HandleProductOrderedAsync(string content)
        {
            var product = _productRepository.GetProductByID(int.Parse(content));
            product.Stock -= 1;
            _productRepository.UpdateProduct(product);
            _productRepository.Save();
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
