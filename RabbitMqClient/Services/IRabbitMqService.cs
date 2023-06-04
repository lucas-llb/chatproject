using RabbitMQ.Client;

namespace RabbitMqClient.Services
{
    public interface IRabbitMqService
    {
        IConnection GetConnection();
        bool SendMessage(IConnection connection, string message, string chatRoom);
        string ReceiveMessage(IConnection connection, string chatRoom);
    }
}
