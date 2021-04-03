namespace RocketNotify.TelegramBot.Tests.Filtration
{
    using NUnit.Framework;

    using RocketNotify.TelegramBot.Filtration;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    [TestFixture]
    public class ContainsCommandMessageFilterTests
    {
        private ContainsCommandMessageFilter _filter;

        [SetUp]
        public void SetUp()
        {
            _filter = new ContainsCommandMessageFilter();
        }

        [Test]
        public void Filter_MessageDoesNotContainCommand_ShouldReturnFalse()
        {
            var message = new Message();

            var actual = _filter.Filter(message);

            Assert.False(actual);
        }

        [Test]
        public void Filter_MessageContainsCommand_ShouldReturnTrue()
        {
            var message = new Message
            {
                Entities = new MessageEntity[]
                {
                    new MessageEntity { Type = MessageEntityType.BotCommand }
                }
            };

            var actual = _filter.Filter(message);

            Assert.True(actual);
        }
    }
}
