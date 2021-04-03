namespace RocketNotify.TelegramBot.Filtration
{
    using RocketNotify.TelegramBot.MessageProcessing;
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    using Telegram.Bot.Types;

    /// <summary>
    /// Filters a message based on existance of a message processor waiting for the message.
    /// </summary>
    public class RelevantMessageFilter : IChainedMessageFilter
    {
        /// <summary>
        /// Message processors storage.
        /// </summary>
        private readonly IMessageProcessorStorage _processors;

        /// <summary>
        /// Next message filter.
        /// </summary>
        private IMessageFilter _nextFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelevantMessageFilter"/> class.
        /// </summary>
        /// <param name="processors">Message processors storage.</param>
        public RelevantMessageFilter(IMessageProcessorStorage processors)
        {
            _processors = processors;
        }

        /// <inheritdoc/>
        public bool Filter(Message message)
        {
            var botMessage = MessageConverter.Convert(message);
            if (_processors.IsRelevantToAny(botMessage))
                return true;

            return _nextFilter.Filter(message);
        }

        /// <inheritdoc/>
        public void SetNextFilter(IMessageFilter nextFilter) => _nextFilter = nextFilter;
    }
}
