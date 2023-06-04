using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqClient.Configuration;
using System;
using System.Text;

namespace RabbitMqClient.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly RabbitMqConfiguration _configuration;
        public RabbitMqService(IOptions<RabbitMqConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }
        public IConnection GetConnection()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri($"amqp://{_configuration.User}:{_configuration.Password}@{_configuration.Host}:5672/");
            return factory.CreateConnection();
        }

        public bool SendMessage(IConnection connection, string message, string chatRoom)
        {
            try
            {
                var channel = connection.CreateModel();
                channel.ExchangeDeclare("messageExchange", ExchangeType.Direct);
                channel.QueueBind(chatRoom, "messageExchange", chatRoom, null);
                var msg = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("messageExchange", chatRoom, null, msg);
            }
            catch (Exception ex)
            {

            }
            return true;
        }

        public string ReceiveMessage(IConnection connection, string chatRoom)
        {
            try
            {
                var channel = connection.CreateModel();
                channel.QueueDeclare(chatRoom, true, false, false, null);
                var consumer = new EventingBasicConsumer(channel);
                var result = channel.BasicGet(chatRoom, true);
                return result != null ? Encoding.UTF8.GetString(result.Body.ToArray()) : null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
