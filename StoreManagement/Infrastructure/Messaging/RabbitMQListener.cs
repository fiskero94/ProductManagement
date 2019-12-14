using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly RabbitMQConfig _config;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IRabbitMQHandler _handler;
        private IConnection _connection;
        private IModel _model;

        public RabbitMQListener(RabbitMQConfig config, IRabbitMQHandler handler)
        {
            _config = config;
            _handler = handler;

            _serializerSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            _serializerSettings.Converters.Add(new StringEnumConverter
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            });

            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = _config.Username,
                Password = _config.Password
            };

            _connection = factory.CreateConnection(_config.Hosts);
            _model = _connection.CreateModel();

            _model.ExchangeDeclare(_config.Exchange, "fanout", durable: true, autoDelete: false);
            _model.QueueDeclare(_config.Queue, durable: true, autoDelete: false, exclusive: false);
            _model.QueueBind(_config.Queue, _config.Exchange, string.Empty);
            _model.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            EventingBasicConsumer consumer = new EventingBasicConsumer(_model);
            consumer.Received += Consumer_Received;
            _model.BasicConsume(_config.Queue, false, consumer);

            return Task.CompletedTask;
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            string messageType = Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers["MessageType"]);
            string body = Encoding.UTF8.GetString(e.Body);
            JObject message = Deserialize(body);

            _handler.HandleAsync(messageType, message);
            _model.BasicAck(e.DeliveryTag, false);
        }

        private JObject Deserialize(string value)
        {
            return JsonConvert.DeserializeObject<JObject>(value, _serializerSettings);
        }
    }
}