using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Messaging
{
    public interface IRabbitMQHandler
    {
        Task HandleAsync(string messageType, JObject message);
    }
}