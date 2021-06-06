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
    using RocketNotify.ChatClient.Exceptions;
    using RocketNotify.ChatClient.Model.Messages;
    using RocketNotify.Notification;
    using RocketNotify.Notification.Model;
    using RocketNotify.TelegramBot.Client;

    using Telegram.Bot.Exceptions;

    [TestFixture]
    public class NotifierBackgroundServiceTests
    {
        private Mock<ITelegramMessageSender> _messageSender;

        private Mock<IRocketChatClient> _rocketChatClient;

        private Mock<INotificationProvider> _notificationProvider;

        private Mock<IServicesSettingsProvider> _settingsProvider;

        private NotifierBackgroundService _backgroundService;

        [SetUp]
        public void SetUp()
        {
            _messageSender = new Mock<ITelegramMessageSender>();
            _rocketChatClient = new Mock<IRocketChatClient>();
            _notificationProvider = new Mock<INotificationProvider>();
            _settingsProvider = new Mock<IServicesSettingsProvider>();

            var logger = new Mock<ILogger<NotifierBackgroundService>>();
            _backgroundService = new NotifierBackgroundService(
                _messageSender.Object,
                _rocketChatClient.Object,
                _notificationProvider.Object,
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
            var notification = new Notification { ChatId = 1, Text = string.Empty };
            var currentDateTime = DateTime.Now;

            _settingsProvider.Setup(x => x.GetMessageCheckInterval()).Returns(TimeSpan.FromMilliseconds(1));
            _notificationProvider.Setup(x => x.GenerateNotificationsAsync(It.IsAny<MessagesData>())).ReturnsAsync(new[] { notification });

            var counter = 0;
            _rocketChatClient.Setup(x => x.GetLastMessageTimeStampAsync()).ReturnsAsync(() => default);
            _rocketChatClient.Setup(x => x.GetRecentMessagesAsync()).ReturnsAsync(
                () => ++counter < 2
                ? Array.Empty<Message>()
                : new Message[] { new Message { TimeStamp = currentDateTime, Mentions = Array.Empty<Mention>() } });

            await _backgroundService.StartAsync(CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(50).ConfigureAwait(false);
            await _backgroundService.StopAsync(CancellationToken.None).ConfigureAwait(false);

            _rocketChatClient.Verify(x => x.GetLastMessageTimeStampAsync(), Times.Exactly(1));
            _rocketChatClient.Verify(x => x.GetRecentMessagesAsync(), Times.AtLeast(2));
            _messageSender.Verify(x => x.SendMessageAsync(notification.ChatId, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task StartAsync_ChatClientThrowsException_ShouldAttemptTenTimesAndReturn()
        {
            _settingsProvider.Setup(x => x.GetMessageCheckInterval()).Returns(TimeSpan.FromMilliseconds(1));
            _rocketChatClient.Setup(x => x.GetLastMessageTimeStampAsync()).ThrowsAsync(new RocketChatApiException("Test"));
            _rocketChatClient.Setup(x => x.GetRecentMessagesAsync()).ThrowsAsync(new RocketChatApiException("Test"));

            await _backgroundService.StartAsync(CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(200).ConfigureAwait(false);
            await _backgroundService.StopAsync(CancellationToken.None).ConfigureAwait(false);

            _rocketChatClient.Verify(x => x.GetLastMessageTimeStampAsync(), Times.Exactly(1));
            _rocketChatClient.Verify(x => x.GetRecentMessagesAsync(), Times.Exactly(9));
        }

        [Test]
        public async Task StartAsync_TelegramClientThrowsException_ShouldAttemptTenTimesAndReturn()
        {
            var notification = new Notification { ChatId = 1, Text = string.Empty };
            _notificationProvider.Setup(x => x.GenerateNotificationsAsync(It.IsAny<MessagesData>())).ReturnsAsync(new[] { notification });
            _settingsProvider.Setup(x => x.GetMessageCheckInterval()).Returns(TimeSpan.FromMilliseconds(1));
            _rocketChatClient.Setup(x => x.GetLastMessageTimeStampAsync()).ReturnsAsync(() => DateTime.Now);
            _rocketChatClient.Setup(x => x.GetRecentMessagesAsync()).ReturnsAsync(() => new Message[] { new Message { TimeStamp = DateTime.Now, Mentions = Array.Empty<Mention>() } });
            _messageSender.Setup(x => x.SendMessageAsync(It.IsAny<long>(), It.IsAny<string>())).ThrowsAsync(new ApiRequestException("Test"));

            await _backgroundService.StartAsync(CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(200).ConfigureAwait(false);
            await _backgroundService.StopAsync(CancellationToken.None).ConfigureAwait(false);

            _rocketChatClient.Verify(x => x.GetLastMessageTimeStampAsync(), Times.Exactly(1));
            _rocketChatClient.Verify(x => x.GetRecentMessagesAsync(), Times.Exactly(10));
            _messageSender.Verify(x => x.SendMessageAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Exactly(10));
        }
    }
}
