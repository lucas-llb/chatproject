using Microsoft.Extensions.DependencyInjection;
using RabbitMqClient.Services;

namespace RabbitMqClient.Extensions
{
    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqService(this IServiceCollection services)
        {
            return services.AddScoped<IRabbitMqService, RabbitMqService>();
        }
    }
}
