namespace RocketNotify.TelegramBot.Tests.Filtration
{
    using System;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.TelegramBot.Filtration;
    using RocketNotify.TelegramBot.Settings;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    [TestFixture]
    public class BotMentionMessageFilterTests
    {
        private Mock<IBotSettingsProvider> _settingsProvider;

        private Mock<IMessageFilter> _nextFilter;

        private BotMentionMessageFilter _filter;

        [SetUp]
        public void SetUp()
        {
            _settingsProvider = new Mock<IBotSettingsProvider>();
            _nextFilter = new Mock<IMessageFilter>();
            _nextFilter.Setup(x => x.Filter(It.IsAny<Message>())).Returns(true);

            _filter = new BotMentionMessageFilter(_settingsProvider.Object);
            _filter.SetNextFilter(_nextFilter.Object);
        }

        [Test]
        public void Filter_NoBotName_ShouldThrowException()
        {
            var message = new Message { Chat = new Chat { Type = ChatType.Group } };
            Assert.Throws<InvalidOperationException>(() => _filter.Filter(message));
        }

        [Test]
        public void Filter_ChatIsPrivate_ShouldInvokeNextFilter()
        {
            var message = new Message { Chat = new Chat { Type = ChatType.Private } };
            var actual = _filter.Filter(message);

            Assert.True(actual);
            _nextFilter.Verify(x => x.Filter(message), Times.Once);
        }

        [Test]
        public void Filter_MessageContainsMention_ShouldInvokeNextFilter()
        {
            _settingsProvider.Setup(x => x.GetBotUserName()).Returns("@test_bot_name");

            var message = new Message
            {
                Chat = new Chat { Type = ChatType.Group },
                Text = "@test_bot_name",
                Entities = new MessageEntity[]
                {
                    new MessageEntity { Type = MessageEntityType.Mention, Offset = 0, Length = 14 }
                }
            };
            var actual = _filter.Filter(message);

            Assert.True(actual);
            _nextFilter.Verify(x => x.Filter(message), Times.Once);
        }

        [Test]
        public void Filter_MessageContainsCommandWithMention_ShouldInvokeNextFilter()
        {
            _settingsProvider.Setup(x => x.GetBotUserName()).Returns("@test_bot_name");

            var message = new Message
            {
                Chat = new Chat { Type = ChatType.Group },
                Text = "/start@test_bot_name",
                Entities = new MessageEntity[]
                {
                    new MessageEntity { Type = MessageEntityType.BotCommand, Offset = 0, Length = 20 }
                }
            };
            var actual = _filter.Filter(message);

            Assert.True(actual);
            _nextFilter.Verify(x => x.Filter(message), Times.Once);
        }

        [Test]
        public void Filter_MessageIsGroupAndWithoutMention_ShouldReturnFalse()
        {
            _settingsProvider.Setup(x => x.GetBotUserName()).Returns("@test_bot_name");

            var message = new Message { Chat = new Chat { Type = ChatType.Group } };
            var actual = _filter.Filter(message);

            Assert.False(actual);
            _nextFilter.Verify(x => x.Filter(It.IsAny<Message>()), Times.Never);
        }
    }
}
