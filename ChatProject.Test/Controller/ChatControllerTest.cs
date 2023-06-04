using ChatProject.Api.Controllers;
using ChatProject.Models.Request;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using RabbitMQ.Client;
using RabbitMqClient.Services;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

namespace ChatProject.Test.Controller
{
    public class ChatControllerTest
    {
        private readonly Mock<IRabbitMqService> _mockService;
        private readonly Mock<IConnection> _connectionMock;
        public ChatControllerTest()
        {
            _mockService = new Mock<IRabbitMqService>();
            _connectionMock = new Mock<IConnection>();
        }

        [Fact]
        public void Should_Send_Message_To_Service()
        {
            var identityMock = new Mock<IIdentity>();
            identityMock.SetupGet(x => x.Name).Returns("MyName");
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.SetupGet(x => x.Identity).Returns(identityMock.Object);
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.User).Returns(claimsPrincipalMock.Object);
            var routeDataMock = new Mock<RouteData>();
            _mockService.Setup(x => x.GetConnection()).Returns(_connectionMock.Object);
            _mockService.Setup(x => x.SendMessage(It.IsAny<IConnection>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var request = new PostMessageRequest
            {
                Message = "My message to send"
            };
            var controller = new ChatController(_mockService.Object);
            controller.ControllerContext = new ControllerContext(new ActionContext(httpContextMock.Object, routeDataMock.Object, new ControllerActionDescriptor()));
            var result = controller.SendMessage("stock", request);

            result.Value.Should().Be(null);
        }

        [Fact]
        public void Should_Receive_Messages_From_RabbitMq()
        {
            var message = "MyMessage";
            _mockService.Setup(x => x.GetConnection()).Returns(_connectionMock.Object);
            _mockService.Setup(x => x.ReceiveMessage(It.IsAny<IConnection>(), It.IsAny<string>())).Returns(message);

            var controller = new ChatController(_mockService.Object);
            var result = controller.ReceiveMessage("stock");

            result.Value.Should().Be(message);
        }
    }
}
