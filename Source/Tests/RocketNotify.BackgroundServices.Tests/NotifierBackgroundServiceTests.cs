namespace RocketNotify.BackgroundServices.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.BackgroundServices.Settings;
    using RocketNotify.ChatClient;
    using RocketNotify.Subscription.Model;
    using RocketNotify.Subscription.Services;
    using RocketNotify.TelegramBot.Client;

    [TestFixture]
    public class NotifierBackgroundServiceTests
    {
        private Mock<ITelegramMessageSender> _messageSender;

        private Mock<IRocketChatClient> _rocketChatClient;

        private Mock<ISubscriptionService> _subscriptionService;

        private Mock<IServicesSettingsProvider> _settingsProvider;

        private NotifierBackgroundService _backgroundService;

        [SetUp]
        public void SetUp()
        {
            _messageSender = new Mock<ITelegramMessageSender>();
            _rocketChatClient = new Mock<IRocketChatClient>();
            _subscriptionService = new Mock<ISubscriptionService>();
            _settingsProvider = new Mock<IServicesSettingsProvider>();

            var logger = new Mock<ILogger<NotifierBackgroundService>>();
            _backgroundService = new NotifierBackgroundService(
                _messageSender.Object,
                _rocketChatClient.Object,
                _subscriptionService.Object,
                logger.Object,
                _settingsProvider.Object);
        }

        [Test]
        public async Task StartAsync_TelegramInitializationError_ShouldReturn()
        {
            _messageSender.Setup(x => x.InitializeAsync()).ThrowsAsync(new InvalidOperationException());

            await _backgroundService.StartAsync(CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(5).ConfigureAwait(false);
            await _backgroundService.StopAsync(CancellationToken.None).ConfigureAwait(false);

            _messageSender.Verify(x => x.InitializeAsync(), Times.Once);
            _rocketChatClient.Verify(x => x.InitializeAsync(), Times.Never);
            _rocketChatClient.Verify(x => x.GetLastMessageTimeStampAsync(), Times.Never);
        }

        [Test]
        public async Task StartAsync_ChatClientInitializationError_ShouldReturn()
        {
            _rocketChatClient.Setup(x => x.InitializeAsync()).ThrowsAsync(new InvalidOperationException());

            await _backgroundService.StartAsync(CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(5).ConfigureAwait(false);
            await _backgroundService.StopAsync(CancellationToken.None).ConfigureAwait(false);

            _messageSender.Verify(x => x.InitializeAsync(), Times.Once);
            _rocketChatClient.Verify(x => x.InitializeAsync(), Times.Once);
            _rocketChatClient.Verify(x => x.GetLastMessageTimeStampAsync(), Times.Never);
        }

        [Test]
        public async Task StartAsync_CancelledAfterStart_ShouldReturn()
        {
            _settingsProvider.Setup(x => x.GetMessageCheckInterval()).Returns(TimeSpan.FromSeconds(6));

            using var cts = new CancellationTokenSource(1);
            await _backgroundService.StartAsync(cts.Token).ConfigureAwait(false);
            await Task.Delay(5).ConfigureAwait(false);
            await _backgroundService.StopAsync(CancellationToken.None).ConfigureAwait(false);

            _messageSender.Verify(x => x.InitializeAsync(), Times.Once);
            _rocketChatClient.Verify(x => x.InitializeAsync(), Times.Once);
            _rocketChatClient.Verify(x => x.GetLastMessageTimeStampAsync(), Times.Once);
        }

        [Test]
        public async Task StartAsync_FoundNewMessage_ShouldNotify()
        {
            var subscriber = new Subscriber { ChatId = 1 };
            var currentDateTime = DateTime.Now;

            _settingsProvider.Setup(x => x.GetMessageCheckInterval()).Returns(TimeSpan.FromMilliseconds(1));
            _subscriptionService.Setup(x => x.GetAllSubscriptionsAsync()).ReturnsAsync(new[] { subscriber });

            var counter = 0;
            _rocketChatClient.Setup(x => x.GetLastMessageTimeStampAsync()).ReturnsAsync(() => ++counter < 3 ? default : currentDateTime);

            await _backgroundService.StartAsync(CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(30).ConfigureAwait(false);
            await _backgroundService.StopAsync(CancellationToken.None).ConfigureAwait(false);

            _rocketChatClient.Verify(x => x.GetLastMessageTimeStampAsync(), Times.AtLeast(3));
            _messageSender.Verify(x => x.SendMessageAsync(subscriber.ChatId, It.IsAny<string>()), Times.Once);
        }
    }
}
