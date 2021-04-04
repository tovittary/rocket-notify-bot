namespace RocketNotify.TelegramBot.Tests.MessageProcessing.Unsubscribe
{
    using System;
    using System.Threading.Tasks;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Services;
    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.MessageProcessing.Model;
    using RocketNotify.TelegramBot.MessageProcessing.Unsubscribe;

    [TestFixture]
    public class UnsubscribeCommandProcessorTests
    {
        private Mock<ISubscriptionService> _subscriptionService;

        private Mock<ITelegramMessageSender> _responder;

        private UnsubscribeCommandProcessor _processor;

        [SetUp]
        public void SetUp()
        {
            _subscriptionService = new Mock<ISubscriptionService>();
            _responder = new Mock<ITelegramMessageSender>();
            _processor = new UnsubscribeCommandProcessor(_subscriptionService.Object, _responder.Object);
        }

        [TestCase("")]
        [TestCase("Test text")]
        [TestCase("Test text with a 'Unsubscribe' word in it")]
        [TestCase("/unsub")]
        public void IsRelevant_DoesNotContainCommand_ShouldReturnFalse(string text)
        {
            var message = new BotMessage { Text = text };
            var actual = _processor.IsRelevant(message);

            Assert.False(actual);
        }

        [TestCase("/unsubscribe")]
        [TestCase("Test text with a /Unsubscribe command in it")]
        [TestCase("/unsubscribe@bot_name")]
        [TestCase("/unsubscribe @bot_name")]
        [TestCase("@bot_name /unsubscribe")]
        public void IsRelevant_ContainsCommand_ShouldReturnFalse(string text)
        {
            var message = new BotMessage { Text = text };
            var actual = _processor.IsRelevant(message);

            Assert.True(actual);
        }

        [Test]
        public async Task ProcessAsync_Unsubscribed_ShouldSendMessage()
        {
            var message = new BotMessage { Sender = new MessageSender { Id = 1, Name = "User" }, Text = "/unsubscribe" };
            var result = await _processor.ProcessAsync(message).ConfigureAwait(false);

            Assert.True(result.IsFinal);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ProcessAsync_SubscriberNotFound_ShouldSendMessage()
        {
            _subscriptionService.Setup(x => x.RemoveSubscriptionAsync(It.IsAny<long>())).ThrowsAsync(new SubscriberNotFoundException("test"));

            var message = new BotMessage { Sender = new MessageSender { Id = 1, Name = "User" }, Text = "/unsubscribe" };
            var result = await _processor.ProcessAsync(message).ConfigureAwait(false);

            Assert.True(result.IsFinal);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ProcessAsync_SubscriberOperationExceptionOccurred_ShouldSendMessage()
        {
            _subscriptionService.Setup(x => x.RemoveSubscriptionAsync(It.IsAny<long>())).ThrowsAsync(new SubscriberOperationException("test"));

            var message = new BotMessage { Sender = new MessageSender { Id = 1, Name = "User" }, Text = "/unsubscribe" };
            var result = await _processor.ProcessAsync(message).ConfigureAwait(false);

            Assert.True(result.IsFinal);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ProcessAsync_UnexpectedException_ShouldThrow()
        {
            _subscriptionService.Setup(x => x.RemoveSubscriptionAsync(It.IsAny<long>())).ThrowsAsync(new InvalidOperationException("test"));

            var message = new BotMessage { Sender = new MessageSender { Id = 1, Name = "User" }, Text = "/unsubscribe" };
            Assert.ThrowsAsync<InvalidOperationException>(() => _processor.ProcessAsync(message));

            _responder.Verify(x => x.SendMessageAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }
    }
}
