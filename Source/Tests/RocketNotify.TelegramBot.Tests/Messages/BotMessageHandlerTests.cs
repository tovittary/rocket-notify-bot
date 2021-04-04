namespace RocketNotify.TelegramBot.Tests.Messages
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.TelegramBot.Filtration;
    using RocketNotify.TelegramBot.Filtration.Factory;
    using RocketNotify.TelegramBot.MessageProcessing;
    using RocketNotify.TelegramBot.MessageProcessing.Model;
    using RocketNotify.TelegramBot.Messages;

    using Telegram.Bot.Types;

    [TestFixture]
    public class BotMessageHandlerTests
    {
        private Mock<IMessageFilter> _messageFilter;

        private Mock<IMessageProcessorInvoker> _messageProcessorInvoker;

        private Mock<ILogger<BotMessageHandler>> _logger;

        private BotMessageHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _messageFilter = new Mock<IMessageFilter>();
            var messageFilterFactory = new Mock<IMessageFilterFactory>();
            messageFilterFactory.Setup(x => x.GetFilter()).Returns(_messageFilter.Object);

            _messageProcessorInvoker = new Mock<IMessageProcessorInvoker>();
            _logger = new Mock<ILogger<BotMessageHandler>>();

            _handler = new BotMessageHandler(messageFilterFactory.Object, _messageProcessorInvoker.Object, _logger.Object);
        }

        [Test]
        public async Task HandleAsync_MessageFilteredOut_ShouldReturn()
        {
            var message = CreateTestMessage();
            await _handler.HandleAsync(message).ConfigureAwait(false);

            _messageProcessorInvoker.Verify(x => x.InvokeAsync(It.IsAny<BotMessage>()), Times.Never);
        }

        [Test]
        public async Task HandleAsync_ExceptionDuringFiltration_ShouldReturn()
        {
            _messageFilter.Setup(x => x.Filter(It.IsAny<Message>())).Throws<InvalidOperationException>();

            var message = CreateTestMessage();
            await _handler.HandleAsync(message).ConfigureAwait(false);

            _messageProcessorInvoker.Verify(x => x.InvokeAsync(It.IsAny<BotMessage>()), Times.Never);
        }

        [Test]
        public async Task HandleAsync_MessagePassedFilters_ShouldProcess()
        {
            _messageFilter.Setup(x => x.Filter(It.IsAny<Message>())).Returns(true);

            var message = CreateTestMessage();
            await _handler.HandleAsync(message).ConfigureAwait(false);

            _messageProcessorInvoker.Verify(x => x.InvokeAsync(It.IsAny<BotMessage>()), Times.Once);
        }

        private Message CreateTestMessage()
        {
            return new Message
            {
                From = new User { Username = "User" },
                Chat = new Chat { Title = "Chat", FirstName = "Chat" },
                Text = "Text"
            };
        }
    }
}
