namespace RocketNotify.TelegramBot.Tests.Filtration
{
    using Moq;

    using NUnit.Framework;

    using RocketNotify.TelegramBot.Filtration;
    using RocketNotify.TelegramBot.MessageProcessing;
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    using Telegram.Bot.Types;

    [TestFixture]
    public class RelevantMessageFilterTests
    {
        private Mock<IMessageProcessorStorage> _messageProcessorStorage;

        private Mock<IMessageFilter> _nextFilter;

        private RelevantMessageFilter _filter;

        [SetUp]
        public void SetUp()
        {
            _messageProcessorStorage = new Mock<IMessageProcessorStorage>();

            _nextFilter = new Mock<IMessageFilter>();
            _nextFilter.Setup(x => x.Filter(It.IsAny<Message>())).Returns(true);

            _filter = new RelevantMessageFilter(_messageProcessorStorage.Object);
            _filter.SetNextFilter(_nextFilter.Object);
        }

        [Test]
        public void Filter_MessageIsRelevant_ShouldReturnTrue()
        {
            var message = new Message { Chat = new Chat() };
            _messageProcessorStorage.Setup(x => x.IsRelevantToAny(It.IsAny<BotMessage>())).Returns(true);

            var actual = _filter.Filter(message);

            Assert.True(actual);
            _messageProcessorStorage.Verify(x => x.IsRelevantToAny(It.IsAny<BotMessage>()), Times.Once);
            _nextFilter.Verify(x => x.Filter(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public void Filter_MessageNotRelevant_ShouldInvokeNextFilter()
        {
            var message = new Message { Chat = new Chat() };

            var actual = _filter.Filter(message);

            Assert.True(actual);
            _messageProcessorStorage.Verify(x => x.IsRelevantToAny(It.IsAny<BotMessage>()), Times.Once);
            _nextFilter.Verify(x => x.Filter(message), Times.Once);
        }
    }
}
