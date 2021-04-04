namespace RocketNotify.BackgroundServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using RocketNotify.BackgroundServices.Settings;
    using RocketNotify.ChatClient;
    using RocketNotify.ChatClient.Exceptions;
    using RocketNotify.Subscription.Model;
    using RocketNotify.Subscription.Services;
    using RocketNotify.TelegramBot.Client;

    using Telegram.Bot.Exceptions;

    /// <summary>
    /// Rocket.Chat message notification background service.
    /// </summary>
    public class NotifierBackgroundService : BackgroundService
    {
        /// <summary>
        /// The maximum number of Rocket.Chat or Telegram errors.
        /// When this number is reached, execution will stop.
        /// </summary>
        private const int MaxErrorsCount = 10;

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
        /// Current Rocket.Chat client errors count.
        /// </summary>
        private int _chatClientErrorsCount;

        /// <summary>
        /// Current Telegram errors count.
        /// </summary>
        private int _telegramErrorsCount;

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
                await _telegramClient.InitializeAsync().ConfigureAwait(false);
                await _rocketChatClient.InitializeAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return;
            }

            _lastMessageTimeStamp = await GetLastMessageTimeStampAsync().ConfigureAwait(false);
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_chatClientErrorsCount >= MaxErrorsCount || _telegramErrorsCount >= MaxErrorsCount)
                {
                    _logger.LogCritical("Maximum number of errors reached, execution stopped");
                    return;
                }

                await Task.Delay(_delayTime, stoppingToken).ConfigureAwait(false);

                var newMessageTimeStamp = await GetLastMessageTimeStampAsync().ConfigureAwait(false);
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
            foreach (var sub in subscribers)
                await NotifyAsync(sub).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the latest group chat message timestamp.
        /// </summary>
        /// <returns>The latest group chat message timestamp.</returns>
        private async Task<DateTime> GetLastMessageTimeStampAsync()
        {
            try
            {
                var lastMessageTime = await _rocketChatClient.GetLastMessageTimeStampAsync().ConfigureAwait(false);
                _chatClientErrorsCount = 0;

                return lastMessageTime;
            }
            catch (RocketChatApiException ex)
            {
                _chatClientErrorsCount++;
                _logger.LogError(ex, $"Error occurred while obtaining time of the latest message in Rocket.Chat group ({_chatClientErrorsCount} / {MaxErrorsCount})");

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical error occurred");
                throw;
            }
        }

        /// <summary>
        /// Sends notification to the subscriber.
        /// </summary>
        /// <param name="subscriber">Subscriber info.</param>
        /// <returns>A task that represents the subscriber notification process.</returns>
        private async Task NotifyAsync(Subscriber subscriber)
        {
            try
            {
                await _telegramClient.SendMessageAsync(subscriber.ChatId, "New Rocket.Chat message received").ConfigureAwait(false);
                _telegramErrorsCount = 0;
            }
            catch (ApiRequestException ex)
            {
                _telegramErrorsCount++;
                _logger.LogError(ex, $"Error occurred while sending notification to {subscriber.ChatId} ({_telegramErrorsCount} / {MaxErrorsCount})");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical error occurred");
                throw;
            }
        }
    }
}
