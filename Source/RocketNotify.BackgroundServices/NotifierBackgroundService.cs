namespace RocketNotify.BackgroundServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using RocketNotify.BackgroundServices.Settings;
    using RocketNotify.ChatClient;
    using RocketNotify.Subscription.Services;
    using RocketNotify.TelegramBot.Client;

    /// <summary>
    /// Rocket.Chat message notification background service.
    /// </summary>
    public class NotifierBackgroundService : BackgroundService
    {
        /// <summary>
        /// Client used for sending Telegram messages.
        /// </summary>
        private readonly ITelegramMessageSender _telegramClient;

        /// <summary>
        /// Rocket.Chat client.
        /// </summary>
        private readonly IRocketChatClient _rocketChatClient;

        /// <summary>
        /// Notifications subscriptions service.
        /// </summary>
        private readonly ISubscriptionService _subscriptionService;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<NotifierBackgroundService> _logger;

        /// <summary>
        /// Service settings provider.
        /// </summary>
        private readonly IServicesSettingsProvider _settingsProvider;

        /// <summary>
        /// New messages check interval.
        /// </summary>
        private TimeSpan _delayTime;

        /// <summary>
        /// The last received message timestamp.
        /// </summary>
        private DateTime _lastMessageTimeStamp = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifierBackgroundService"/> class.
        /// </summary>
        /// <param name="telegramClient">Client used for sending Telegram messages.</param>
        /// <param name="rocketChatClient">Rocket.Chat client.</param>
        /// <param name="subscriptionService">Notifications subscriptions service.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settingsProvider">Service settings provider.</param>
        public NotifierBackgroundService(
            ITelegramMessageSender telegramClient,
            IRocketChatClient rocketChatClient,
            ISubscriptionService subscriptionService,
            ILogger<NotifierBackgroundService> logger,
            IServicesSettingsProvider settingsProvider)
        {
            _telegramClient = telegramClient;
            _rocketChatClient = rocketChatClient;
            _subscriptionService = subscriptionService;
            _logger = logger;
            _settingsProvider = settingsProvider;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _delayTime = _settingsProvider.GetMessageCheckInterval();
            _logger.LogInformation($"Checking new messages every {_delayTime.TotalSeconds} seconds");

            try
            {
                await _telegramClient.Initialize().ConfigureAwait(false);
                await _rocketChatClient.InitializeAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return;
            }

            _lastMessageTimeStamp = await _rocketChatClient.GetLastMessageTimeStampAsync().ConfigureAwait(false);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_delayTime, stoppingToken).ConfigureAwait(false);

                var newMessageTimeStamp = await _rocketChatClient.GetLastMessageTimeStampAsync().ConfigureAwait(false);
                if (newMessageTimeStamp > _lastMessageTimeStamp)
                {
                    _lastMessageTimeStamp = newMessageTimeStamp;
                    _logger.LogInformation("New Rocket.Chat message received");

                    await NotifySubscribersAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Notifies subscribers about the discovered message.
        /// </summary>
        /// <returns>A task that represents the subscribers notification process.</returns>
        private async Task NotifySubscribersAsync()
        {
            var subscribers = await _subscriptionService.GetAllSubscriptionsAsync().ConfigureAwait(false);
            foreach (var subs in subscribers)
                await _telegramClient.SendMessageAsync(subs.ChatId, "New Rocket.Chat message received").ConfigureAwait(false);
        }
    }
}
