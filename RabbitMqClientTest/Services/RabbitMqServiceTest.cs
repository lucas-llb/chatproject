using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using RabbitMqClient.Configuration;
using RabbitMqClient.Services;
using System.Text;
using Xunit;

namespace RabbitMqClient.Test.Services
{
    public class RabbitMqServiceTest
    {
        private readonly Mock<IOptions<RabbitMqConfiguration>> _options;
        private readonly Mock<IConnection> _connection;

        public RabbitMqServiceTest()
        {
            _options = new Mock<IOptions<RabbitMqConfiguration>>();
            _connection = new Mock<IConnection>();
        }

        [Fact]
        public void Should_Send_Message_Correctly()
        {
            var config = new RabbitMqConfiguration
            {
                Host = "localhost",
                User = "user",
                Password = "password"
            };
            _options.SetupGet(x => x.Value).Returns(config);

            _connection.Setup(x => x.CreateModel()).Returns(new Mock<IModel>().Object);
            var service = new RabbitMqService(_options.Object);

            var flag = service.SendMessage(_connection.Object, "my message", "myQueue");

            flag.Should().BeTrue();

        }

        [Fact]
        public void Should_Receive_Message_From_Broker()
        {
            var config = new RabbitMqConfiguration
            {
                Host = "localhost",
                User = "user",
                Password = "password"
            };
            var channel = new Mock<IModel>();
            var message = "My Message to validate";
            var bytes = Encoding.UTF8.GetBytes(message);
            channel.Setup(x => x.BasicGet(It.IsAny<string>(), It.IsAny<bool>())).Returns(new BasicGetResult(1, true, "", "", 1, null, bytes));
            _options.SetupGet(x => x.Value).Returns(config);

            _connection.Setup(x => x.CreateModel()).Returns(channel.Object);
            var service = new RabbitMqService(_options.Object);

            var recievedMessage = service.ReceiveMessage(_connection.Object, "myQueue");

            recievedMessage.Should().Be(message);
        }
    }
}
