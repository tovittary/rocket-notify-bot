namespace RocketNotifyBot
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using RocketNotify.BackgroundServices;
    using RocketNotify.ChatClient;
    using RocketNotify.ChatClient.ApiClient;
    using RocketNotify.ChatClient.Settings;
    using RocketNotify.Subscription.Data;
    using RocketNotify.Subscription.Services;
    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.Client.Factory;
    using RocketNotify.TelegramBot.Commands;
    using RocketNotify.TelegramBot.Messages;
    using RocketNotify.TelegramBot.Messages.Filtration;
    using RocketNotify.TelegramBot.Settings;

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
            var host = CreateHostBuilder(args).Build();
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
            services.AddSingleton<IFileStorage, JsonFileStorage>();
            services.AddSingleton<ISubscribersRepository, JsonSubscribersRepository>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();

            services.AddTransient<ICommand, HelpCommand>();
            services.AddTransient<ICommand, StartCommand>();
            services.AddTransient<ICommand, SubscribeCommand>();
            services.AddTransient<ICommand, UnsubscribeCommand>();

            services.AddTransient<IBotSettingsProvider, BotSettingsProvider>();
            services.AddTransient<IBotMessageProcessor, BotMessageProcessor>();

            services.AddTransient<IInitialMessageFilter, MessageTypeMessageFilter>();
            services.AddTransient<IMessageFilter, ChatTypeMessageFilter>();
            services.AddTransient<IMessageFilter, BotMentionMessageFilter>();
            services.AddTransient<IMessageFilter, ContainsCommandMessageFilter>();
            services.AddTransient<IBotMessageHandler, BotMessageHandler>();

            services.AddTransient<ITelegramBotClientFactory, TelegramBotClientFactory>();
            services.AddSingleton<ITelegramMessagePollingClient, TelegramBotPollingClient>();
            services.AddSingleton<ITelegramMessageSender>(srv => srv.GetRequiredService<ITelegramMessagePollingClient>());

            services.AddTransient<IClientSettingsProvider, ClientSettingsProvider>();
            services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>();
            services.AddTransient<IRestApiClient, RestApiClient>();
            services.AddTransient<IRocketChatClient, RocketChatClient>();

            services.AddHostedService<TelegramBotBackgroundService>();
            services.AddHostedService<NotifierBackgroundService>();
        }
    }
}
