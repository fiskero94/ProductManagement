using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;

namespace Infrastructure.Messaging
{
    public class RabbitMQMessenger
    {
        private readonly RabbitMQConfig _config;
        private readonly JsonSerializerSettings _serializerSettings;

        public RabbitMQMessenger(RabbitMQConfig config)
        {
            _config = config;

            _serializerSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            _serializerSettings.Converters.Add(new StringEnumConverter
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            });
        }

        public Task PublishAsync(string messageType, object message)
        {
            return Task.Run(() => {

                ConnectionFactory factory = new ConnectionFactory()
                {
                    UserName = _config.Username,
                    Password = _config.Password
                };

                using IConnection connection = factory.CreateConnection(_config.Hosts);
                using IModel model = connection.CreateModel();

                model.ExchangeDeclare(_config.Exchange, "fanout", durable: true, autoDelete: false);
                var body = Encoding.UTF8.GetBytes(Serialize(message));
                IBasicProperties properties = model.CreateBasicProperties();
                properties.Headers = new Dictionary<string, object> { { "MessageType", messageType } };

                model.BasicPublish(_config.Exchange, string.Empty, properties, body);

            });
        }

        private string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, _serializerSettings);
        }
    }
}