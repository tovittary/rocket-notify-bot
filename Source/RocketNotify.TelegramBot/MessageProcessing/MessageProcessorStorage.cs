namespace RocketNotify.TelegramBot.MessageProcessing
{
    using System.Collections.Generic;
    using System.Linq;

    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Stores message processors awaiting for more messages.
    /// </summary>
    public class MessageProcessorStorage : IMessageProcessorStorage
    {
        /// <summary>
        /// The list of existing message processors.
        /// </summary>
        private readonly List<IMessageProcessor> _processors = new List<IMessageProcessor>();

        /// <inheritdoc/>
        public IMessageProcessor GetExisting(BotMessage message)
        {
            var existing = _processors.FirstOrDefault(p => p.IsRelevant(message));
            _processors.Remove(existing);

            return existing;
        }

        /// <inheritdoc/>
        public bool IsRelevantToAny(BotMessage message)
        {
            return _processors.Any(p => p.IsRelevant(message));
        }

        /// <inheritdoc/>
        public void StoreProcessor(IMessageProcessor processor)
        {
            _processors.Add(processor);
        }
    }
}
