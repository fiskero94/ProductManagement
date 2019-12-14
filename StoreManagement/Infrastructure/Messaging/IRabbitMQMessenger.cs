using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public interface IRabbitMQMessenger
    {
        Task PublishAsync(string messageType, object message);
    }
}