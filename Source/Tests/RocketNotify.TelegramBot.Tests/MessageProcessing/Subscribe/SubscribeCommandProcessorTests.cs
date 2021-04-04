namespace RocketNotify.TelegramBot.Tests.MessageProcessing.Subscribe
{
    using System;
    using System.Threading.Tasks;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Services;
    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.MessageProcessing.Model;
    using RocketNotify.TelegramBot.MessageProcessing.Subscribe;

    using Telegram.Bot.Types.ReplyMarkups;

    [TestFixture]
    public class SubscribeCommandProcessorTests
    {
        private Mock<ISubscriptionService> _subscriptionService;

        private SubscriptionCompleteState _completedState;

        private VerifySubscriptionState _verifyState;

        private InitialSubscribeState _initialState;

        private Mock<ITelegramMessageSender> _responder;

        private SubscribeCommandProcessor _processor;

        [SetUp]
        public void SetUp()
        {
            _subscriptionService = new Mock<ISubscriptionService>();

            _completedState = new SubscriptionCompleteState();
            _verifyState = new VerifySubscriptionState(_subscriptionService.Object, () => _completedState);
            _initialState = new InitialSubscribeState(_subscriptionService.Object, () => _verifyState, () => _completedState);

            _responder = new Mock<ITelegramMessageSender>();
            _processor = new SubscribeCommandProcessor(_responder.Object, () => _initialState);
        }

        [TestCase("")]
        [TestCase("Test text")]
        [TestCase("Test text with a 'Unsubscribe' word in it")]
        [TestCase("/sub")]
        public void IsRelevant_InitialState_DoesNotContainCommand_ShouldReturnFalse(string text)
        {
            var message = new BotMessage { Text = text };
            var actual = _processor.IsRelevant(message);

            Assert.False(actual);
        }

        [TestCase("/Subscribe")]
        [TestCase("Test text with a /subscribe command in it")]
        [TestCase("/subscribe@bot_name")]
        [TestCase("/subscribe @bot_name")]
        [TestCase("@bot_name /subscribe")]
        public void IsRelevant_InitialState_ContainsCommand_ShouldReturnFalse(string text)
        {
            var message = new BotMessage { Text = text };
            var actual = _processor.IsRelevant(message);

            Assert.True(actual);
        }

        [Test]
        public void IsRelevant_VerifyState_DoesNotContainReply_ShouldReturnFalse()
        {
            var botResponse = new BotMessage { MessageId = 1 };

            _processor.Context.LastResponse = botResponse;
            _processor.ChangeCurrentState(_verifyState);

            var message = new BotMessage { Text = "secret" };
            var actual = _processor.IsRelevant(message);

            Assert.False(actual);
        }

        [Test]
        public void IsRelevant_VerifyState_ContainsReply_ShouldReturnTrue()
        {
            var botResponse = new BotMessage { MessageId = 1 };

            _processor.Context.LastResponse = botResponse;
            _processor.ChangeCurrentState(_verifyState);

            var message = new BotMessage { Text = "secret", ReplyInfo = new MessageReply { SourceMessageId = botResponse.MessageId } };
            var actual = _processor.IsRelevant(message);

            Assert.True(actual);
        }

        [Test]
        public void IsRelevant_VerifyState_ContextIsEmpty_ShouldThrowException()
        {
            _processor.ChangeCurrentState(_verifyState);

            var message = new BotMessage { Text = "secret" };
            Assert.Throws<InvalidOperationException>(() => _processor.IsRelevant(message));
        }

        [Test]
        public void IsRelevant_CompletedState_ShouldReturnFalse()
        {
            _processor.ChangeCurrentState(_completedState);

            var message = new BotMessage { Text = "some_text" };
            var actual = _processor.IsRelevant(message);

            Assert.False(actual);
        }

        [Test]
        public async Task ProcessAsync_InitialState_NoNeedForVerification_ShouldSubscribeAndRespond()
        {
            var message = new BotMessage { Sender = new MessageSender { Id = 1, Name = "User" }, Text = "/subscribe" };
            var result = await _processor.ProcessAsync(message).ConfigureAwait(false);

            Assert.True(result.IsFinal);
            Assert.AreEqual(_completedState, _processor.CurrentState);
            _subscriptionService.Verify(x => x.AddSubscriptionAsync(message.Sender.Id, string.Empty), Times.Once);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ProcessAsync_InitialState_NoNeedForVerification_ErrorDuringSubscription_ShouldSendResponse()
        {
            _subscriptionService.Setup(x => x.CheckSubscriptionKeyNeeded()).Returns(false);
            _subscriptionService.Setup(x => x.AddSubscriptionAsync(It.IsAny<long>(), It.IsAny<string>())).ThrowsAsync(new SubscriberAlreadyExistsException("test"));

            var message = new BotMessage { Sender = new MessageSender { Id = 1, Name = "User" }, Text = "/subscribe" };
            var result = await _processor.ProcessAsync(message).ConfigureAwait(false);

            Assert.True(result.IsFinal);
            Assert.AreEqual(_completedState, _processor.CurrentState);
            _subscriptionService.Verify(x => x.AddSubscriptionAsync(message.Sender.Id, string.Empty), Times.Once);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ProcessAsync_InitialState_NoNeedForVerification_UnexpectedError_ShouldThrow()
        {
            _subscriptionService.Setup(x => x.CheckSubscriptionKeyNeeded()).Returns(false);
            _subscriptionService.Setup(x => x.AddSubscriptionAsync(It.IsAny<long>(), It.IsAny<string>())).ThrowsAsync(new InvalidOperationException());

            var message = new BotMessage { Sender = new MessageSender { Id = 1, Name = "User" }, Text = "/subscribe" };
            Assert.ThrowsAsync<InvalidOperationException>(() => _processor.ProcessAsync(message));

            _subscriptionService.Verify(x => x.AddSubscriptionAsync(message.Sender.Id, string.Empty), Times.Once);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ProcessAsync_InitialState_VerificationNeeded_ShouldRequestVerificationKey()
        {
            _subscriptionService.Setup(x => x.CheckSubscriptionKeyNeeded()).Returns(true);

            var message = new BotMessage { Sender = new MessageSender { Id = 1, Name = "User" }, Text = "/subscribe" };
            var result = await _processor.ProcessAsync(message).ConfigureAwait(false);

            Assert.False(result.IsFinal);
            Assert.AreEqual(_verifyState, _processor.CurrentState);
            Assert.AreEqual(message, _processor.Context.LastMessage);
            _subscriptionService.Verify(x => x.AddSubscriptionAsync(message.Sender.Id, string.Empty), Times.Never);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>(), It.IsAny<IReplyMarkup>()), Times.Once);
        }

        [Test]
        public async Task ProcessAsync_VerifyState_VerificationKeyReceived_ShouldSubscribeAndRespond()
        {
            _subscriptionService.Setup(x => x.CheckSubscriptionKeyNeeded()).Returns(true);

            _processor.ChangeCurrentState(_verifyState);
            var message = new BotMessage
            {
                Sender = new MessageSender { Id = 2, Name = "User" },
                Text = "secret",
                ReplyInfo = new MessageReply { SourceMessageId = 1 }
            };
            var result = await _processor.ProcessAsync(message).ConfigureAwait(false);

            Assert.True(result.IsFinal);
            Assert.AreEqual(_completedState, _processor.CurrentState);
            _subscriptionService.Verify(x => x.AddSubscriptionAsync(message.Sender.Id, message.Text), Times.Once);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ProcessAsync_VerifyState_ErrorDuringSubscription_ShouldRespond()
        {
            _subscriptionService.Setup(x => x.CheckSubscriptionKeyNeeded()).Returns(true);
            _subscriptionService.Setup(x => x.AddSubscriptionAsync(It.IsAny<long>(), It.IsAny<string>())).ThrowsAsync(new SubscriptionNotAllowedException("invalid key"));

            _processor.ChangeCurrentState(_verifyState);
            var message = new BotMessage { Sender = new MessageSender { Id = 2, Name = "User" }, Text = "secret" };
            var result = await _processor.ProcessAsync(message).ConfigureAwait(false);

            Assert.True(result.IsFinal);
            Assert.AreEqual(_completedState, _processor.CurrentState);
            _subscriptionService.Verify(x => x.AddSubscriptionAsync(message.Sender.Id, message.Text), Times.Once);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ProcessAsync_VerifyState_UnexpectedError_ShouldThrow()
        {
            _subscriptionService.Setup(x => x.CheckSubscriptionKeyNeeded()).Returns(true);
            _subscriptionService.Setup(x => x.AddSubscriptionAsync(It.IsAny<long>(), It.IsAny<string>())).ThrowsAsync(new InvalidOperationException());

            _processor.ChangeCurrentState(_verifyState);
            var message = new BotMessage { Sender = new MessageSender { Id = 2, Name = "User" }, Text = "secret" };
            Assert.ThrowsAsync<InvalidOperationException>(() => _processor.ProcessAsync(message));

            _subscriptionService.Verify(x => x.AddSubscriptionAsync(message.Sender.Id, message.Text), Times.Once);
            _responder.Verify(x => x.SendMessageAsync(message.Sender.Id, It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ProcessAsync_CompletedState_ShouldThrowException()
        {
            _processor.ChangeCurrentState(_completedState);

            var message = new BotMessage();
            Assert.ThrowsAsync<InvalidOperationException>(() => _processor.ProcessAsync(message));
        }
    }
}
