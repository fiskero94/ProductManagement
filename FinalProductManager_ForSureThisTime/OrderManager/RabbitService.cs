﻿using Microsoft.Extensions.Hosting;
using OrderManager.Model;
using OrderManager.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderManager
{
    public class RabbitService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;

        public RabbitService()
        {
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "172.22.146.161", UserName = "rabbitmquser", Password = "appelsin123" };

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("order.exchange", ExchangeType.Topic);
            _channel.QueueDeclare("order.queue.log", false, false, false, null);
            _channel.QueueBind("order.queue.log", "order.exchange", "order.queue.*", null);
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

            _channel.BasicConsume("order.queue.log", false, consumer);
            return Task.CompletedTask;
        }

        public void PublishProductOrdered(int productId)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(productId.ToString());
            IBasicProperties properties = _channel.CreateBasicProperties();
            properties.Type = "ProductOrdered";
            _channel.BasicPublish("product.exchange", "product.queue.*", properties, bytes);
        }

        private async Task HandleMessageAsync(string content, string type)
        {
            switch (type)
            {
                case "ProductCreated":
                    await HandleProductCreatedAsync(content);
                    break;
                case "ProductStockTooLow":
                    await HandleProductStockTooLowAsync(content);
                    break;
                case "ProductOrderSuccess":
                    await HandleProductOrderSuccessAsync(content);
                    break;
                default:
                    break;
            }
        }

        private async Task HandleProductCreatedAsync(string content)
        {
            string[] productArray = content.Split(null);
            ProductRepository productRepository = ProductRepository.GetRepository();
            Product product = new Product();
            product.ProductId = int.Parse(productArray[0]);
            product.Stock = int.Parse(productArray[1]);
            await productRepository.InsertProductAsync(product);
        }

        private async Task HandleProductStockTooLowAsync(string content)
        {
            throw new Exception();
        }

        private async Task HandleProductOrderSuccessAsync(string content)
        {
            ProductRepository productRepository = ProductRepository.GetRepository();
            var oldProduct = await productRepository.GetProductByIDAsync(int.Parse(content));
            var updatedProduct = oldProduct;
            updatedProduct.Stock -= 1;
            await productRepository.UpdateProductAsync(oldProduct, updatedProduct);
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