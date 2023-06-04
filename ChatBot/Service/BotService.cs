using Microsoft.Extensions.Logging;
using Quartz;
using RabbitMqClient.Services;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace ChatBot.Service
{
    public class BotService : IJob
    {
        private readonly ILogger<BotService> _logger;
        private readonly IRabbitMqService _rabbitMqService;
        public BotService(ILogger<BotService> logger, IRabbitMqService rabbitMqService)
        {
            _logger = logger;
            _rabbitMqService = rabbitMqService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var client = new RestClient("https://stooq.com");
            var uri = new RestRequest("/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv");

            try
            {
                var response = await client.GetAsync(uri);
                var content = response.Content.Split("\r\n");
                var values = content[1].Split(',');
                var message = $"[{DateTime.Now.ToLocalTime()}] BOT says: {values[0]} quote is ${values[6]} per share";
                var connection = _rabbitMqService.GetConnection();
                _rabbitMqService.SendMessage(connection, message, "appl.us");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred when requested stooq: {ex}");
            }

        }
    }
}
