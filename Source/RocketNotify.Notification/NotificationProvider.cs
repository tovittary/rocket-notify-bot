namespace RocketNotify.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using RocketNotify.Notification.Model;
    using RocketNotify.Subscription.Model;
    using RocketNotify.Subscription.Services;

    /// <summary>
    /// Provides functionality for generating notification messages based on data received from Rocket.Chat.
    /// </summary>
    public class NotificationProvider : INotificationProvider
    {
        /// <summary>
        /// The default text of a notification message.
        /// </summary>
        private const string DefaultNotificationText = "New Rocket.Chat message received";

        /// <summary>
        /// Notification subscription manager.
        /// </summary>
        private readonly ISubscriptionService _subscriptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationProvider"/> class.
        /// </summary>
        /// <param name="subscriptionService">Notification subscription manager.</param>
        public NotificationProvider(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Notification>> GenerateNotificationsAsync(MessagesData data)
        {
            var subscribers = await _subscriptionService.GetAllSubscriptionsAsync().ConfigureAwait(false);

            var notifications = subscribers.Select(s => GenerateNotification(s, data));
            return notifications;
        }

        /// <summary>
        /// Generates notification message for subscriber based on messages data provided.
        /// </summary>
        /// <param name="subscriber">Subscriber info.</param>
        /// <param name="messages">Messages data.</param>
        /// <returns>Subscriber notification message.</returns>
        private Notification GenerateNotification(Subscriber subscriber, MessagesData messages)
        {
            if (messages.MentionedUsernames.Length == 0 || subscriber.Configuration == null)
                return new Notification { ChatId = subscriber.ChatId, Text = DefaultNotificationText };

            var notification = new Notification { ChatId = subscriber.ChatId };

            var subscriberMentionsConfig = subscriber.Configuration.Mentions;
            foreach (var mentionedUsername in messages.MentionedUsernames)
            {
                var relevantConfig = subscriberMentionsConfig
                    .FirstOrDefault(c => c.MentionedUsername.Equals(mentionedUsername, StringComparison.InvariantCultureIgnoreCase));

                if (relevantConfig == null)
                    continue;

                notification.Text = $"{notification.Text}{relevantConfig.NotificationText}{Environment.NewLine}";
            }

            notification.Text = string.IsNullOrWhiteSpace(notification.Text) ? DefaultNotificationText : notification.Text.Trim();
            return notification;
        }
    }
}
