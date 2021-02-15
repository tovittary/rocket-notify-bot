namespace RocketNotify.BackgroundServices
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;

    using RocketNotify.TelegramBot.Client;

    /// <summary>
    /// The background service whose sole task is to start and stop the process of receiving messages by the bot.
    /// </summary>
    public class TelegramBotBackgroundService : IHostedService
    {
        /// <summary>
        /// Telegram bot client that uses polling mechanism to receive messages.
        /// </summary>
        private readonly ITelegramMessagePollingClient _telegramBot;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotBackgroundService"/> class.
        /// </summary>
        /// <param name="telegramBot">Telegram bot client that uses polling mechanism to receive messages.</param>
        public TelegramBotBackgroundService(ITelegramMessagePollingClient telegramBot)
        {
            _telegramBot = telegramBot;
        }

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _telegramBot.Initialize();
            _telegramBot.StartPolling(cancellationToken);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _telegramBot.StopPolling();

            return Task.CompletedTask;
        }
    }
}
