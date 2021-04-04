namespace RocketNotify.TelegramBot.Tests.Filtration
{
    using Moq;

    using NUnit.Framework;

    using RocketNotify.TelegramBot.Filtration;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    [TestFixture]
    public class ChatTypeMessageFilterTests
    {
        private Mock<IMessageFilter> _nextFilter;

        private ChatTypeMessageFilter _filter;

        [SetUp]
        public void SetUp()
        {
            _nextFilter = new Mock<IMessageFilter>();
            _nextFilter.Setup(x => x.Filter(It.IsAny<Message>())).Returns(true);

            _filter = new ChatTypeMessageFilter();
            _filter.SetNextFilter(_nextFilter.Object);
        }

        [TestCase(ChatType.Supergroup)]
        [TestCase(ChatType.Channel)]
        public void Filter_SupergroupOrChannel_ShouldReturnFalse(ChatType chatType)
        {
            var message = new Message { Chat = new Chat { Type = chatType } };
            var actual = _filter.Filter(message);

            Assert.False(actual);
            _nextFilter.Verify(x => x.Filter(It.IsAny<Message>()), Times.Never);
        }

        [TestCase(ChatType.Group)]
        [TestCase(ChatType.Private)]
        public void Filter_PrivateOrGroup_ShouldInvokeNextFilter(ChatType chatType)
        {
            var message = new Message { Chat = new Chat { Type = chatType } };
            var actual = _filter.Filter(message);

            Assert.True(actual);
            _nextFilter.Verify(x => x.Filter(message), Times.Once);
        }
    }
}
