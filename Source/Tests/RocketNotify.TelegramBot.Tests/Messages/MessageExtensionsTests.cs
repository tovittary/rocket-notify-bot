namespace RocketNotify.TelegramBot.Tests.Messages
{
    using NUnit.Framework;

    using RocketNotify.TelegramBot.Messages;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    [TestFixture]
    public class MessageExtensionsTests
    {
        [Test]
        public void GetEntities_NoEntities_ShouldReturnEmptyArray()
        {
            var message = new Message();

            var actual = message.GetEntities();

            Assert.NotNull(actual);
            Assert.IsEmpty(actual);
        }

        [Test]
        public void GetEntities_HasEntities_ShouldReturnArray()
        {
            var message = new Message
            {
                Text = "/start @bot_name http://test.com 555-258-66-77",
                Entities = new MessageEntity[]
                {
                    new MessageEntity { Type = MessageEntityType.BotCommand, Offset = 0, Length = 6 },
                    new MessageEntity { Type = MessageEntityType.Mention, Offset = 7, Length = 9 },
                    new MessageEntity { Type = MessageEntityType.Url, Offset = 17, Length = 15 },
                    new MessageEntity { Type = MessageEntityType.PhoneNumber, Offset = 33, Length = 13 }
                }
            };

            var expected = new (MessageEntityType Type, string Value)[]
            {
                (MessageEntityType.BotCommand, "/start"),
                (MessageEntityType.Mention, "@bot_name"),
                (MessageEntityType.Url, "http://test.com"),
                (MessageEntityType.PhoneNumber, "555-258-66-77"),
            };

            var actual = message.GetEntities();

            Assert.AreEqual(expected.Length, actual.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Type, actual[i].Type);
                Assert.AreEqual(expected[i].Value, actual[i].Value);
            }
        }
    }
}
