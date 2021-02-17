namespace RocketNotify.TelegramBot.Tests.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.Client.Factory;
    using RocketNotify.TelegramBot.Messages;

    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    [TestFixture]
    public class TelegramBotPollingClientTests
    {
        private Mock<IBotMessageHandler> _messageHandlerMock;

        private Mock<ITelegramBotClientFactory> _botClientFactoryMock;

        private Mock<ITelegramBotClient> _botClientMock;

        private TelegramBotPollingClient _client;

        [SetUp]
        public void SetUp()
        {
            _messageHandlerMock = new Mock<IBotMessageHandler>();
            _botClientMock = new Mock<ITelegramBotClient>();
            _botClientMock.Setup(x => x.TestApiAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            _botClientFactoryMock = new Mock<ITelegramBotClientFactory>();
            _botClientFactoryMock.Setup(x => x.GetClient()).Returns(_botClientMock.Object);

            _client = new TelegramBotPollingClient(_messageHandlerMock.Object, _botClientFactoryMock.Object);
        }

        [Test]
        public void StartPolling_NoClient_ShouldThrowException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.StartPolling(CancellationToken.None));
        }

        [Test]
        public async Task StartPolling_HasClient_ShouldStartReceiving()
        {
            await _client.Initialize().ConfigureAwait(false);
            _client.StartPolling(CancellationToken.None);

            _botClientMock.Verify(x => x.StartReceiving(null, CancellationToken.None), Times.Once);
        }

        [Test]
        public void StopPolling_NoClient_ShouldThrowException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.StopPolling());
        }

        [Test]
        public async Task StopPolling_HasClient_ShouldStopReceiving()
        {
            _botClientMock.Setup(x => x.IsReceiving).Returns(true);

            await _client.Initialize().ConfigureAwait(false);
            _client.StopPolling();

            _botClientMock.Verify(x => x.StopReceiving(), Times.Once);
        }

        [Test]
        public void SendMessageAsync_NoClient_ShouldThrowException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.SendMessageAsync(123456, string.Empty));
        }

        [Test]
        public async Task SendMessageAsync_HasClient_ShouldSendTextMessageAsync()
        {
            var chatId = 123456;
            var expectedChatId = (ChatId)chatId;
            var expectedText = "Message Text";
            var cancellationToken = CancellationToken.None;

            await _client.Initialize().ConfigureAwait(false);
            _client.StartPolling(cancellationToken);
            await _client.SendMessageAsync(chatId, expectedText).ConfigureAwait(false);

            _botClientMock.Verify(x => x.SendTextMessageAsync(It.IsAny<ChatId>(), expectedText, ParseMode.Default, false, false, 0, null, cancellationToken), Times.Once);
        }
    }
}
