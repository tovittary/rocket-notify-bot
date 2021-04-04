namespace RocketNotify.TelegramBot.Tests.MessageProcessing.Help
{
    using System.Threading.Tasks;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.MessageProcessing.Commands;
    using RocketNotify.TelegramBot.MessageProcessing.Help;
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    [TestFixture]
    public class HelpCommandProcessorTests
    {
        private Mock<ITelegramMessageSender> _responder;

        private Mock<ICommandInfoAggregator> _commandInfoAggragator;

        private HelpCommandProcessor _processor;

        [SetUp]
        public void SetUp()
        {
            _responder = new Mock<ITelegramMessageSender>();
            _commandInfoAggragator = new Mock<ICommandInfoAggregator>();
            _processor = new HelpCommandProcessor(_commandInfoAggragator.Object, _responder.Object);
        }

        [TestCase("")]
        [TestCase("Test text")]
        [TestCase("Test text with a 'Help' word in it")]
        public void IsRelevant_DoesNotContainCommand_ShouldReturnFalse(string text)
        {
            var message = new BotMessage { Text = text };
            var actual = _processor.IsRelevant(message);

            Assert.False(actual);
        }

        [TestCase("/help")]
        [TestCase("Test text with a /Help command in it")]
        [TestCase("/help@bot_name")]
        [TestCase("/help @bot_name")]
        [TestCase("@bot_name /help")]
        public void IsRelevant_ContainsCommand_ShouldReturnFalse(string text)
        {
            var message = new BotMessage { Text = text };
            var actual = _processor.IsRelevant(message);

            Assert.True(actual);
        }

        [Test]
        public async Task ProcessAsync_ShouldSendMessage()
        {
            _commandInfoAggragator.Setup(x => x.GetDescriptions()).Returns(new[] { new CommandDescription("/test", "test") });

            var message = new BotMessage { Sender = new MessageSender { Id = 1, Name = "User" }, Text = "/help" };
            var result = await _processor.ProcessAsync(message).ConfigureAwait(false);

            Assert.True(result.IsFinal);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>()), Times.Once);
        }
    }
}
