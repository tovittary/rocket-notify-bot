namespace RocketNotify.TelegramBot.Tests.MessageProcessing
{
    using System.Threading.Tasks;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.TelegramBot.MessageProcessing;
    using RocketNotify.TelegramBot.MessageProcessing.Model;
    using RocketNotify.TelegramBot.MessageProcessing.Unsupported;

    [TestFixture]
    public class MessageProcessorInvokerTests
    {
        private Mock<IMessageProcessor> _firstProcessor;

        private Mock<IMessageProcessor> _secondProcessor;

        private Mock<IUnsupportedCommandProcessor> _unsupportedProcessor;

        private MessageProcessorStorage _processorStorage;

        private MessageProcessorFactory _processorFactory;

        private MessageProcessorInvoker _processorInvoker;

        [SetUp]
        public void SetUp()
        {
            _firstProcessor = new Mock<IMessageProcessor>();
            _secondProcessor = new Mock<IMessageProcessor>();
            _unsupportedProcessor = new Mock<IUnsupportedCommandProcessor>();

            _processorStorage = new MessageProcessorStorage();
            _processorFactory = new MessageProcessorFactory(() => new[] { _firstProcessor.Object, _secondProcessor.Object }, () => _unsupportedProcessor.Object);
            _processorInvoker = new MessageProcessorInvoker(_processorStorage, _processorFactory);
        }

        [Test]
        public async Task InvokeAsync_RelevantToFirstProcessor_ShouldInvokeFirstProcessor()
        {
            _firstProcessor.Setup(x => x.ProcessAsync(It.IsAny<BotMessage>())).ReturnsAsync(ProcessResult.Final());
            _firstProcessor.Setup(x => x.IsRelevant(It.IsAny<BotMessage>())).Returns(true);

            var message = new BotMessage();
            await _processorInvoker.InvokeAsync(message).ConfigureAwait(false);

            Assert.AreEqual(0, _processorStorage.Count);
            _firstProcessor.Verify(x => x.ProcessAsync(message), Times.Once);
            _firstProcessor.Verify(x => x.IsRelevant(message), Times.Once);
            _secondProcessor.Verify(x => x.ProcessAsync(message), Times.Never);
            _secondProcessor.Verify(x => x.IsRelevant(message), Times.Never);
            _unsupportedProcessor.Verify(x => x.ProcessAsync(message), Times.Never);
        }

        [Test]
        public async Task InvokeAsync_NotRelevantToAny_ShouldInvokeUnsupportedProcessor()
        {
            _unsupportedProcessor.Setup(x => x.ProcessAsync(It.IsAny<BotMessage>())).ReturnsAsync(ProcessResult.Final());

            var message = new BotMessage();
            await _processorInvoker.InvokeAsync(message).ConfigureAwait(false);

            Assert.AreEqual(0, _processorStorage.Count);
            _firstProcessor.Verify(x => x.ProcessAsync(message), Times.Never);
            _firstProcessor.Verify(x => x.IsRelevant(message), Times.Once);
            _secondProcessor.Verify(x => x.ProcessAsync(message), Times.Never);
            _secondProcessor.Verify(x => x.IsRelevant(message), Times.Once);
            _unsupportedProcessor.Verify(x => x.ProcessAsync(message), Times.Once);
        }

        [Test]
        public async Task InvokeAsync_RelevantToExistingProcessor_ShouldInvokeExistingProcessor()
        {
            _secondProcessor.Setup(x => x.ProcessAsync(It.IsAny<BotMessage>())).ReturnsAsync(ProcessResult.Final());
            _secondProcessor.Setup(x => x.IsRelevant(It.IsAny<BotMessage>())).Returns(true);

            var message = new BotMessage();
            _processorStorage.StoreProcessor(_secondProcessor.Object);
            Assert.AreEqual(1, _processorStorage.Count);
            await _processorInvoker.InvokeAsync(message).ConfigureAwait(false);

            Assert.AreEqual(0, _processorStorage.Count);
            _firstProcessor.Verify(x => x.ProcessAsync(message), Times.Never);
            _firstProcessor.Verify(x => x.IsRelevant(message), Times.Never);
            _secondProcessor.Verify(x => x.ProcessAsync(message), Times.Once);
            _secondProcessor.Verify(x => x.IsRelevant(message), Times.Once);
            _unsupportedProcessor.Verify(x => x.ProcessAsync(message), Times.Never);
        }

        [Test]
        public async Task InvokeAsync_ProcessorAwaitsAnotherMessage_ShouldInvokeProcessorTwice()
        {
            var invokeCount = 0;
            _secondProcessor.Setup(x => x.ProcessAsync(It.IsAny<BotMessage>())).ReturnsAsync(() => ++invokeCount < 2 ? new ProcessResult() : ProcessResult.Final());
            _secondProcessor.Setup(x => x.IsRelevant(It.IsAny<BotMessage>())).Returns(true);

            var message = new BotMessage();
            await _processorInvoker.InvokeAsync(message).ConfigureAwait(false);

            Assert.AreEqual(1, _processorStorage.Count);
            _firstProcessor.Verify(x => x.ProcessAsync(message), Times.Never);
            _firstProcessor.Verify(x => x.IsRelevant(message), Times.Once);
            _secondProcessor.Verify(x => x.ProcessAsync(message), Times.Once);
            _secondProcessor.Verify(x => x.IsRelevant(message), Times.Once);
            _unsupportedProcessor.Verify(x => x.ProcessAsync(message), Times.Never);

            await _processorInvoker.InvokeAsync(message).ConfigureAwait(false);

            Assert.AreEqual(0, _processorStorage.Count);
            _firstProcessor.Verify(x => x.ProcessAsync(message), Times.Never);
            _firstProcessor.Verify(x => x.IsRelevant(message), Times.Once);
            _secondProcessor.Verify(x => x.ProcessAsync(message), Times.Exactly(2));
            _secondProcessor.Verify(x => x.IsRelevant(message), Times.Exactly(2));
            _unsupportedProcessor.Verify(x => x.ProcessAsync(message), Times.Never);
        }
    }
}
