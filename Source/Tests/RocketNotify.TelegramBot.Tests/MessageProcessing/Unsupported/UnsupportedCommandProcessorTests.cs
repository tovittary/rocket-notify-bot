namespace RocketNotify.TelegramBot.Tests.MessageProcessing.Unsupported
{
    using System.Threading.Tasks;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.MessageProcessing.Model;
    using RocketNotify.TelegramBot.MessageProcessing.Unsupported;

    [TestFixture]
    public class UnsupportedCommandProcessorTests
    {
        private Mock<ITelegramMessageSender> _responder;

        private UnsupportedCommandProcessor _processor;

        [SetUp]
        public void SetUp()
        {
            _responder = new Mock<ITelegramMessageSender>();
            _processor = new UnsupportedCommandProcessor(_responder.Object);
        }

        [TestCase("")]
        [TestCase("Test text")]
        public void IsRelevant_DoesNotContainCommand_ShouldReturnFalse(string text)
        {
            var message = new BotMessage { Text = text };
            var actual = _processor.IsRelevant(message);

            Assert.False(actual);
        }

        [TestCase("/not_supported_command")]
        [TestCase("Test text with some /random command in it")]
        [TestCase("/dunno@bot_name")]
        [TestCase("/okay @bot_name")]
        [TestCase("@bot_name /start")]
        public void IsRelevant_ContainsCommand_ShouldReturnFalse(string text)
        {
            var message = new BotMessage { Text = text };
            var actual = _processor.IsRelevant(message);

            Assert.True(actual);
        }

        [Test]
        public async Task ProcessAsync_ShouldSendMessage()
        {
            var message = new BotMessage { Sender = new MessageSender { Id = 1, Name = "User" }, Text = "/some_command" };
            var result = await _processor.ProcessAsync(message).ConfigureAwait(false);

            Assert.True(result.IsFinal);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>()), Times.Once);
        }
    }
}
