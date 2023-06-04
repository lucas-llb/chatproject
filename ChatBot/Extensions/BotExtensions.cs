using Microsoft.Extensions.DependencyInjection;
using Quartz;
using ChatBot.Service;

namespace ChatBot.Extensions
{
    public static class BotExtensions
    {
        public static IServiceCollection ConfigureBotService(this IServiceCollection services)
        {
            services.AddQuartz(config =>
            {
                config.UseMicrosoftDependencyInjectionJobFactory();
                var jobKey = new JobKey("BotService");
                config.AddJob<BotService>(x => x.WithIdentity(jobKey));
                config.AddTrigger(o => 
                o.ForJob(jobKey)
                .WithIdentity("BotService-trigger")
                .WithCronSchedule("0 */1 * ? * *"));
            });

            return services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }
    }
}
