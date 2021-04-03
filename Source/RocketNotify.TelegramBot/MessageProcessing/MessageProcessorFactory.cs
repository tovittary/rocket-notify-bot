namespace RocketNotify.TelegramBot.MessageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// A factory for generating new instances of message processors.
    /// </summary>
    public class MessageProcessorFactory : IMessageProcessorFactory
    {
        /// <summary>
        /// A delegate used for obtaining instances of the <see cref="IMessageProcessor"/> interface implementations.
        /// </summary>
        private readonly Func<IEnumerable<IMessageProcessor>> _getProcessors;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessorFactory"/> class.
        /// </summary>
        /// <param name="getProcessors">A delegate used for obtaining instances of the <see cref="IMessageProcessor"/> interface implementations.</param>
        public MessageProcessorFactory(Func<IEnumerable<IMessageProcessor>> getProcessors)
        {
            _getProcessors = getProcessors;
        }

        /// <inheritdoc/>
        public IMessageProcessor CreateProcessor(BotMessage message)
        {
            return _getProcessors().FirstOrDefault(p => p.IsRelevant(message));
        }
    }
}
