namespace RocketNotify.Notification.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.Notification.Model;
    using RocketNotify.Subscription.Model;
    using RocketNotify.Subscription.Services;

    [TestFixture]
    public class NotificationProviderTests
    {
        private Mock<ISubscriptionService> _subscriptionService;

        private NotificationProvider _notificationProvider;

        [SetUp]
        public void SetUp()
        {
            _subscriptionService = new Mock<ISubscriptionService>();
            _notificationProvider = new NotificationProvider(_subscriptionService.Object);
        }

        [Test]
        public async Task GenerateNotificationsAsync_NoMentions_ShouldReturnDefaultNotification()
        {
            var subscriber = new Subscriber { ChatId = 1 };
            _subscriptionService.Setup(x => x.GetAllSubscriptionsAsync()).ReturnsAsync(new[] { subscriber });

            var testData = new MessagesData { LatestMessageTimeStamp = DateTime.Now, MentionedUsernames = Array.Empty<string>() };
            var actual = (await _notificationProvider.GenerateNotificationsAsync(testData).ConfigureAwait(false)).ToArray();

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(subscriber.ChatId, actual.First().ChatId);
            Assert.AreEqual("New Rocket.Chat message received", actual.First().Text);
        }

        [Test]
        public async Task GenerateNotificationsAsync_NoMentionsConfig_ShouldReturnDefaultNotification()
        {
            var subscriber = new Subscriber { ChatId = 1 };
            _subscriptionService.Setup(x => x.GetAllSubscriptionsAsync()).ReturnsAsync(new[] { subscriber });

            var testData = new MessagesData { LatestMessageTimeStamp = DateTime.Now, MentionedUsernames = new[] { "username" } };
            var actual = (await _notificationProvider.GenerateNotificationsAsync(testData).ConfigureAwait(false)).ToArray();

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(subscriber.ChatId, actual.First().ChatId);
            Assert.AreEqual("New Rocket.Chat message received", actual.First().Text);
        }

        [Test]
        public async Task GenerateNotificationsAsync_IrrelevantMention_ShouldReturnDefaultNotification()
        {
            var subscriber = new Subscriber
            {
                ChatId = 1,
                Configuration = new NotificationConfiguration
                {
                    Mentions = new List<MentionConfiguration>
                    {
                        new MentionConfiguration { MentionedUsername = "otherUserName", NotificationText = "TestNotificationText" }
                    }
                }
            };
            _subscriptionService.Setup(x => x.GetAllSubscriptionsAsync()).ReturnsAsync(new[] { subscriber });

            var testData = new MessagesData { LatestMessageTimeStamp = DateTime.Now, MentionedUsernames = new[] { "username" } };
            var actual = (await _notificationProvider.GenerateNotificationsAsync(testData).ConfigureAwait(false)).ToArray();

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(subscriber.ChatId, actual.First().ChatId);
            Assert.AreEqual("New Rocket.Chat message received", actual.First().Text);
        }

        [Test]
        public async Task GenerateNotificationsAsync_RelevantMention_ShouldReturnConfiguredNotificationText()
        {
            var subscriber = new Subscriber
            {
                ChatId = 1,
                Configuration = new NotificationConfiguration
                {
                    Mentions = new List<MentionConfiguration>
                    {
                        new MentionConfiguration { MentionedUsername = "username", NotificationText = "TestNotificationText" }
                    }
                }
            };
            _subscriptionService.Setup(x => x.GetAllSubscriptionsAsync()).ReturnsAsync(new[] { subscriber });

            var testData = new MessagesData { LatestMessageTimeStamp = DateTime.Now, MentionedUsernames = new[] { "username" } };
            var actual = (await _notificationProvider.GenerateNotificationsAsync(testData).ConfigureAwait(false)).ToArray();

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(subscriber.ChatId, actual.First().ChatId);
            Assert.AreEqual(subscriber.Configuration.Mentions.First().NotificationText, actual.First().Text);
        }

        [Test]
        public async Task GenerateNotificationsAsync_SeveralRelevantMention_ShouldReturnJoinedConfiguredNotificationText()
        {
            var subscriber = new Subscriber
            {
                ChatId = 1,
                Configuration = new NotificationConfiguration
                {
                    Mentions = new List<MentionConfiguration>
                    {
                        new MentionConfiguration { MentionedUsername = "username", NotificationText = "TestNotificationText" },
                        new MentionConfiguration { MentionedUsername = "otherUsername", NotificationText = "OtherTestNotificationText" }
                    }
                }
            };
            _subscriptionService.Setup(x => x.GetAllSubscriptionsAsync()).ReturnsAsync(new[] { subscriber });

            var expectedNotificationText = string.Join(Environment.NewLine, subscriber.Configuration.Mentions.Select(m => m.NotificationText));

            var testData = new MessagesData { LatestMessageTimeStamp = DateTime.Now, MentionedUsernames = new[] { "username", "otherUsername" } };
            var actual = (await _notificationProvider.GenerateNotificationsAsync(testData).ConfigureAwait(false)).ToArray();

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(subscriber.ChatId, actual.First().ChatId);
            Assert.AreEqual(expectedNotificationText, actual.First().Text);
        }

        [Test]
        public async Task GenerateNotificationsAsync_SeveralSubscribers_ShouldReturnSeveralNotifications()
        {
            var subscriber1 = new Subscriber { ChatId = 1 };
            var subscriber2 = new Subscriber { ChatId = 2 };
            _subscriptionService.Setup(x => x.GetAllSubscriptionsAsync()).ReturnsAsync(new[] { subscriber1, subscriber2 });

            var testData = new MessagesData { LatestMessageTimeStamp = DateTime.Now, MentionedUsernames = Array.Empty<string>() };
            var actual = (await _notificationProvider.GenerateNotificationsAsync(testData).ConfigureAwait(false)).ToArray();

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(subscriber1.ChatId, actual[0].ChatId);
            Assert.AreEqual(subscriber2.ChatId, actual[1].ChatId);
            Assert.AreEqual("New Rocket.Chat message received", actual[0].Text);
            Assert.AreEqual("New Rocket.Chat message received", actual[1].Text);
        }

        [Test]
        public async Task GenerateNotificationsAsync_NoSubscribers_ShouldReturnEmpty()
        {
            _subscriptionService.Setup(x => x.GetAllSubscriptionsAsync()).ReturnsAsync(Array.Empty<Subscriber>());

            var testData = new MessagesData { LatestMessageTimeStamp = DateTime.Now, MentionedUsernames = new[] { "username" } };
            var actual = (await _notificationProvider.GenerateNotificationsAsync(testData).ConfigureAwait(false)).ToArray();

            Assert.AreEqual(0, actual.Length);
        }
    }
}
