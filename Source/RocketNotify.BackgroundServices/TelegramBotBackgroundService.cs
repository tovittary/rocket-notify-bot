namespace RocketNotify.BackgroundServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

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
        /// Logger.
        /// </summary>
        private readonly ILogger<TelegramBotBackgroundService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotBackgroundService"/> class.
        /// </summary>
        /// <param name="telegramBot">Telegram bot client that uses polling mechanism to receive messages.</param>
        /// <param name="logger">Logger.</param>
        public TelegramBotBackgroundService(ITelegramMessagePollingClient telegramBot, ILogger<TelegramBotBackgroundService> logger)
        {
            _telegramBot = telegramBot;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _telegramBot.InitializeAsync().ConfigureAwait(false);
                _telegramBot.StartPolling(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
            }
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _telegramBot.StopPolling();

            return Task.CompletedTask;
        }
    }
}
