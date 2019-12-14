using System.Collections.Generic;

namespace Infrastructure.Messaging
{
    public class RabbitMQConfig
    {
        public List<string> Hosts { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Exchange { get; set; }
        public string Queue { get; set; }

        public RabbitMQConfig(string queue)
        {
            Hosts = new List<string>() { "rabbitmq" };
            Username = "rabbitmquser";
            Password = "zrJHWx4G";
            Exchange = "StoreManagement";
            Queue = queue;
        }
    }
}