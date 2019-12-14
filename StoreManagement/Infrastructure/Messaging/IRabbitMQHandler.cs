using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public interface IRabbitMQHandler
    {
        Task HandleAsync(string messageType, JObject message);
    }
}
