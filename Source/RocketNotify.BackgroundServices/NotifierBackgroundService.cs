namespace RocketNotify.BackgroundServices
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using RocketNotify.BackgroundServices.Settings;
    using RocketNotify.ChatClient;
    using RocketNotify.ChatClient.Exceptions;
    using RocketNotify.ChatClient.Model.Messages;
    using RocketNotify.Notification;
    using RocketNotify.Notification.Model;
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
        /// Generates notification messages based on data received from Rocket.Chat.
        /// </summary>
        private readonly INotificationProvider _notificationProvider;

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
        /// <param name="notificationProvider">Generates notification messages based on data received from Rocket.Chat.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settingsProvider">Service settings provider.</param>
        public NotifierBackgroundService(
            ITelegramMessageSender telegramClient,
            IRocketChatClient rocketChatClient,
            INotificationProvider notificationProvider,
            ILogger<NotifierBackgroundService> logger,
            IServicesSettingsProvider settingsProvider)
        {
            _telegramClient = telegramClient;
            _rocketChatClient = rocketChatClient;
            _notificationProvider = notificationProvider;
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

                var recentMessages = await GetRecentMessagesAsync().ConfigureAwait(false);
                var newMessageTimeStamp = recentMessages.LatestMessageTimeStamp;

                if (newMessageTimeStamp > _lastMessageTimeStamp)
                {
                    _lastMessageTimeStamp = newMessageTimeStamp;
                    _logger.LogInformation("New Rocket.Chat messages received");

                    await NotifySubscribersAsync(recentMessages).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Notifies subscribers about the discovered message.
        /// </summary>
        /// <param name="recentMessages">Data on recent messages.</param>
        /// <returns>A task that represents the subscribers notification process.</returns>
        private async Task NotifySubscribersAsync(MessagesData recentMessages)
        {
            var notifications = await _notificationProvider.GenerateNotificationsAsync(recentMessages).ConfigureAwait(false);

            foreach (var notification in notifications)
                await NotifyAsync(notification).ConfigureAwait(false);
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
        /// Collects data on the most recent messages from group chat.
        /// </summary>
        /// <returns>The most recent messages from group chat.</returns>
        private async Task<MessagesData> GetRecentMessagesAsync()
        {
            var messages = await SafeGetRecentMessagesAsync().ConfigureAwait(false);

            return new MessagesData
            {
                LatestMessageTimeStamp = messages.FirstOrDefault()?.TimeStamp ?? default,
                MentionedUsernames = messages
                    .SelectMany(m => m.Mentions)
                    .Select(m => m.Username)
                    .Distinct()
                    .ToArray()
            };
        }

        /// <summary>
        /// Gets recent messages from group chat and catches non-critical errors that may occur during the process.
        /// </summary>
        /// <returns>Recent group chat messages.</returns>
        private async Task<Message[]> SafeGetRecentMessagesAsync()
        {
            try
            {
                var messages = await _rocketChatClient.GetRecentMessagesAsync().ConfigureAwait(false);
                _chatClientErrorsCount = 0;

                return messages;
            }
            catch (RocketChatApiException ex)
            {
                _chatClientErrorsCount++;
                _logger.LogError(ex, $"Error occurred while obtaining recent messages in Rocket.Chat group ({_chatClientErrorsCount} / {MaxErrorsCount})");

                return Array.Empty<Message>();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical error occurred");
                throw;
            }
        }

        /// <summary>
        /// Sends notification message.
        /// </summary>
        /// <param name="notification">Notification message info.</param>
        /// <returns>A task that represents the notification sending process.</returns>
        private async Task NotifyAsync(Notification notification)
        {
            try
            {
                await _telegramClient.SendMessageAsync(notification.ChatId, notification.Text).ConfigureAwait(false);
                _telegramErrorsCount = 0;
            }
            catch (ApiRequestException ex)
            {
                _telegramErrorsCount++;
                _logger.LogError(ex, $"Error occurred while sending notification to {notification.ChatId} ({_telegramErrorsCount} / {MaxErrorsCount})");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical error occurred");
                throw;
            }
        }
    }
}
