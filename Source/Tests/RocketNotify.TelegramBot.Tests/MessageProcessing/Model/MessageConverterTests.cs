namespace RocketNotify.TelegramBot.Tests.MessageProcessing.Model
{
    using NUnit.Framework;

    using RocketNotify.TelegramBot.MessageProcessing.Model;

    using Telegram.Bot.Types;

    [TestFixture]
    public class MessageConverterTests
    {
        [Test]
        public void Convert_RegularMessageFromGroup_ShouldConvert()
        {
            var message = new Message
            {
                MessageId = 123,
                Chat = new Chat { Title = "TestChat" },
                Text = "Test Message Text"
            };

            var converted = MessageConverter.Convert(message);

            Assert.NotNull(converted);
            Assert.AreEqual(message.MessageId, converted.MessageId);
            Assert.AreEqual(message.Text, converted.Text);
            Assert.AreEqual(message.Chat.Title, converted.Sender.Name);
        }

        [Test]
        public void Convert_RegularMessageFromUser_ShouldConvert()
        {
            var message = new Message
            {
                MessageId = 123,
                Chat = new Chat { FirstName = "TestUserName" },
                Text = "Test Message Text"
            };

            var converted = MessageConverter.Convert(message);

            Assert.NotNull(converted);
            Assert.AreEqual(message.MessageId, converted.MessageId);
            Assert.AreEqual(message.Text, converted.Text);
            Assert.AreEqual(message.Chat.FirstName, converted.Sender.Name);
        }

        [Test]
        public void Convert_ReplyMessage_ShouldConvertWithReply()
        {
            var message = new Message
            {
                MessageId = 123,
                Chat = new Chat { FirstName = "TestUserName" },
                Text = "Test Message Text",
                ReplyToMessage = new Message { MessageId = 122 }
            };

            var converted = MessageConverter.Convert(message);

            Assert.NotNull(converted);
            Assert.AreEqual(message.MessageId, converted.MessageId);
            Assert.AreEqual(message.Text, converted.Text);
            Assert.AreEqual(message.Chat.FirstName, converted.Sender.Name);
            Assert.AreEqual(message.ReplyToMessage.MessageId, converted.ReplyInfo.SourceMessageId);
        }
    }
}
