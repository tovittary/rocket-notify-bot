namespace RocketNotify.TelegramBot.MessageProcessing
{
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Invokes the message processor capable of handling the message.
    /// </summary>
    public class MessageProcessorInvoker : IMessageProcessorInvoker
    {
        /// <summary>
        /// Message processors storage.
        /// </summary>
        private readonly IMessageProcessorStorage _processorStorage;

        /// <summary>
        /// Message processors factory.
        /// </summary>
        private readonly IMessageProcessorFactory _processorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessorInvoker"/> class.
        /// </summary>
        /// <param name="processorStorage">Message processors storage.</param>
        /// <param name="processorFactory">Message processors factory.</param>
        public MessageProcessorInvoker(IMessageProcessorStorage processorStorage, IMessageProcessorFactory processorFactory)
        {
            _processorStorage = processorStorage;
            _processorFactory = processorFactory;
        }

        /// <inheritdoc/>
        public Task InvokeAsync(BotMessage message)
        {
            var processor = _processorStorage.GetExisting(message);
            if (processor == null)
                processor = _processorFactory.CreateProcessor(message);

            return InvokeAsync(message, processor);
        }

        /// <summary>
        /// Invokes message processing with the provided processor.
        /// </summary>
        /// <param name="message">The message that should be processed.</param>
        /// <param name="processor">The processor instance capable of processing the message.</param>
        /// <returns>A task representing the message processing operation.</returns>
        private async Task InvokeAsync(BotMessage message, IMessageProcessor processor)
        {
            var processingResult = await processor.ProcessAsync(message).ConfigureAwait(false);

            if (!processingResult.IsFinal)
                _processorStorage.StoreProcessor(processor);
        }
    }
}
