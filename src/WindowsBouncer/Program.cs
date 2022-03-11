using WindowsBouncer.Core;
using WindowsBouncer.Persistence;

namespace WindowsBouncer
{
    public delegate Handler HandlerResolver(string key);

    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureLogging((hostingContext, config) =>
            {
                config.AddLog4Net("log4net.config", true)
                    .SetMinimumLevel(LogLevel.Debug);
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<ILogger, Log4NetLogger>()
                    .AddSingleton<IDataService, LiteDBDataService>()
                    .AddTransient<FirewallHandler>()
                    .AddTransient<PersistenceHandler>()
                    .AddTransient<HandlerResolver>(serviceProvider => key =>
                    {
                        switch (key)
                        {
                            case "FirewallHandler":
                                return serviceProvider.GetService<FirewallHandler>();
                            case "PersistenceHandler":
                                return serviceProvider.GetService<PersistenceHandler>();
                            default:
                                return null;
                        }
                    })
                    .AddSingleton<EventReader>()
                    .AddHostedService<Worker>();
            });
        }
    }
}