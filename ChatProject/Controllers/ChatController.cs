using ChatProject.Models;
using ChatProject.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMqClient.Services;
using System;
using System.Diagnostics;

namespace ChatProject.Api.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IRabbitMqService _service;

        public ChatController(IRabbitMqService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendMessage([FromQuery(Name = "stock")] string stock, [FromBody] PostMessageRequest request)
        {
            var user = HttpContext.User.Identity.Name;
            var fullMessage = $"[{DateTime.Now.ToLocalTime()}] {user} says: {request.Message}";

            var connection = _service.GetConnection();
            var flag = _service.SendMessage(connection, fullMessage, stock);
            return Json(null);
        }

        [HttpGet]
        public JsonResult ReceiveMessage([FromQuery(Name = "stock")] string stock)
        {
            var connection = _service.GetConnection();
            string message = _service.ReceiveMessage(connection, stock);
            return Json(message);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
