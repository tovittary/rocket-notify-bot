namespace RocketNotify.TelegramBot.Tests.Filtration
{
    using Moq;

    using NUnit.Framework;

    using RocketNotify.TelegramBot.Filtration;

    using Telegram.Bot.Types;

    [TestFixture]
    public class MessageTypeMessageFilterTests
    {
        private Mock<IMessageFilter> _nextFilter;

        private MessageTypeMessageFilter _filter;

        [SetUp]
        public void SetUp()
        {
            _nextFilter = new Mock<IMessageFilter>();
            _nextFilter.Setup(x => x.Filter(It.IsAny<Message>())).Returns(true);

            _filter = new MessageTypeMessageFilter();
            _filter.SetNextFilter(_nextFilter.Object);
        }

        [Test]
        public void Filter_VideoMessage_ShouldReturnFalse()
        {
            var message = new Message { Video = new Video() };

            var actual = _filter.Filter(message);

            Assert.False(actual);
            _nextFilter.Verify(x => x.Filter(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public void Filter_TextMessage_ShouldInvokeNextFilter()
        {
            var message = new Message { Text = "test" };

            var actual = _filter.Filter(message);

            Assert.True(actual);
            _nextFilter.Verify(x => x.Filter(It.IsAny<Message>()), Times.Once);
        }
    }
}
