namespace RocketNotify.Notifier
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using RocketNotify.ChatClient;
    using RocketNotify.Subscription.Services;
    using RocketNotify.TelegramBot.Interfaces;

    /// <summary>
    /// Messages notification service.
    /// </summary>
    public class Notifier : INotifier
    {
        /// <summary>
        /// New messages check interval.
        /// </summary>
        private static readonly TimeSpan _delayTime = TimeSpan.FromSeconds(6);

        /// <summary>
        /// Client used for sending Telegram messages.
        /// </summary>
        private readonly ITelegramBotMessageSender _telegramBot;

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
        private readonly ILogger<Notifier> _logger;

        /// <summary>
        /// The last received message timestamp.
        /// </summary>
        private DateTime _lastMessageTimeStamp = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="Notifier"/> class.
        /// </summary>
        /// <param name="telegramBot">Client used for sending Telegram messages.</param>
        /// <param name="rocketChatClient">Rocket.Chat client.</param>
        /// <param name="subscriptionService">Notifications subscriptions service.</param>
        /// <param name="logger">Logger.</param>
        public Notifier(
            ITelegramBotMessageSender telegramBot,
            IRocketChatClient rocketChatClient,
            ISubscriptionService subscriptionService,
            ILogger<Notifier> logger)
        {
            _telegramBot = telegramBot;
            _rocketChatClient = rocketChatClient;
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken token)
        {
            await _rocketChatClient.InitializeAsync().ConfigureAwait(false);

            var firstRun = true;
            while (!token.IsCancellationRequested)
            {
                var newMessageTimeStamp = await _rocketChatClient.GetLastMessageTimeStampAsync().ConfigureAwait(false);
                if (newMessageTimeStamp > _lastMessageTimeStamp)
                {
                    _lastMessageTimeStamp = newMessageTimeStamp;

                    if (firstRun)
                    {
                        firstRun = false;

                        await Task.Delay(_delayTime, token).ConfigureAwait(false);
                        continue;
                    }

                    _logger.LogInformation($"[{DateTime.Now}] New Rocket.Chat message received");

                    var subscribers = await _subscriptionService.GetAllSubscriptionsAsync().ConfigureAwait(false);
                    foreach (var subs in subscribers)
                        await _telegramBot.SendMessageAsync(subs.ChatId, "New Rocket.Chat message received").ConfigureAwait(false);
                }

                await Task.Delay(_delayTime, token).ConfigureAwait(false);
            }
        }
    }
}
