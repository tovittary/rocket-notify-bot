namespace RocketNotifyBot
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using RocketNotify.ChatClient;
    using RocketNotify.ChatClient.ApiClient;
    using RocketNotify.ChatClient.Settings;
    using RocketNotify.Notifier;
    using RocketNotify.Subscription.Data;
    using RocketNotify.Subscription.Services;
    using RocketNotify.TelegramBot;
    using RocketNotify.TelegramBot.Commands;
    using RocketNotify.TelegramBot.Interfaces;
    using RocketNotify.TelegramBot.Settings;

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

            using var cts = new CancellationTokenSource();

            try
            {
                var notifyTask = RunNotifyBotAsync(host.Services, cts.Token);
                await host.RunAsync();

                cts.Cancel();
                await notifyTask;
            }
            catch (TaskCanceledException)
            {
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
                .ConfigureServices(RegisterAllServices);
        }

        /// <summary>
        /// Configures logging providers.
        /// </summary>
        /// <param name="logging">An interface for configuring loggin providers.</param>
        private static void AddLoggers(ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddConsole();
        }

        /// <summary>
        /// Registers all services.
        /// </summary>
        /// <param name="services">Registered services.</param>
        private static void RegisterAllServices(IServiceCollection services)
        {
            services.AddSingleton<ISubscribersRepository, JsonSubscribersRepository>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();

            services.AddTransient<ICommand, HelpCommand>();
            services.AddTransient<ICommand, StartCommand>();
            services.AddTransient<ICommand, SubscribeCommand>();
            services.AddTransient<ICommand, UnsubscribeCommand>();

            services.AddTransient<IBotSettingsProvider, BotSettingsProvider>();
            services.AddTransient<IBotMessageProcessor, BotMessageProcessor>();
            services.AddSingleton<ITelegramBotPollingClient, TelegramBotPollingClient>();
            services.AddSingleton<ITelegramBotMessageSender>(srv => srv.GetRequiredService<ITelegramBotPollingClient>());

            services.AddTransient<IClientSettingsProvider, ClientSettingsProvider>();
            services.AddTransient<IRestApiClient, RestApiClient>();
            services.AddTransient<IRocketChatClient, RocketChatClient>();

            services.AddSingleton<INotifier, Notifier>();
        }

        /// <summary>
        /// Starts Rocket.Chat messages monitoring process.
        /// </summary>
        /// <param name="services">Service provider.</param>
        /// <param name="token">A token for stopping the monitoring process.</param>
        /// <returns>A task representing the messages monitoring process.</returns>
        private static async Task RunNotifyBotAsync(IServiceProvider services, CancellationToken token)
        {
            var telegramBot = services.GetRequiredService<ITelegramBotPollingClient>();
            telegramBot.Initialize();
            telegramBot.StartPolling(token);

            var notifier = services.GetRequiredService<INotifier>();
            await notifier.StartAsync(token).ConfigureAwait(false);

            telegramBot.StopPolling();
        }
    }
}
