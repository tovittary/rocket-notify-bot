namespace RocketNotifyBot
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using RocketNotify.DependencyInjection;

    using RocketNotifyBot.Logging;

    /// <summary>
    /// Application entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <returns>A task representing the application starting process.</returns>
        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                await host.RunAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error has occured.");
            }
        }

        /// <summary>
        /// Creates generic host builder.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <returns>Host builder.</returns>
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(AddLoggers)
                .ConfigureServices(RegisterAllServices)
                .UseWindowsService();
        }

        /// <summary>
        /// Configures logging providers.
        /// </summary>
        /// <param name="hostContext">The host context.</param>
        /// <param name="logging">An interface for configuring loggin providers.</param>
        private static void AddLoggers(HostBuilderContext hostContext, ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleFormatterOptions>();
            logging.AddConsole(opt => opt.FormatterName = CustomConsoleFormatter.FormatterName);

            var serilogConfiguration = hostContext.Configuration.GetSection("Logging").GetSection("Serilog");
            if (serilogConfiguration != null)
                logging.AddFile(serilogConfiguration);
        }

        /// <summary>
        /// Registers all services.
        /// </summary>
        /// <param name="services">Registered services.</param>
        private static void RegisterAllServices(IServiceCollection services)
        {
            new SubscriptionServiceRegistration().Register(services);
            new TelegramBotServiceRegistration().Register(services);
            new ChatClientServiceRegistration().Register(services);
            new BackgroundServiceRegistration().Register(services);
        }
    }
}
