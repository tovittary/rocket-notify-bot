namespace RocketNotify.TelegramBot.MessageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RocketNotify.TelegramBot.MessageProcessing.Model;
    using RocketNotify.TelegramBot.MessageProcessing.Unsupported;

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
        /// A delegate used for obtaining an instance of the <see cref="IUnsupportedCommandProcessor"/> interface implementation.
        /// </summary>
        private readonly Func<IUnsupportedCommandProcessor> _getUnsupportedCommandProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessorFactory"/> class.
        /// </summary>
        /// <param name="getProcessors">A delegate used for obtaining instances of the <see cref="IMessageProcessor"/> interface implementations.</param>
        /// <param name="getUnsupportedCommandProcessor">A delegate used for obtaining an instance of the <see cref="IUnsupportedCommandProcessor"/> interface implementation.</param>
        public MessageProcessorFactory(Func<IEnumerable<IMessageProcessor>> getProcessors, Func<IUnsupportedCommandProcessor> getUnsupportedCommandProcessor)
        {
            _getProcessors = getProcessors;
            _getUnsupportedCommandProcessor = getUnsupportedCommandProcessor;
        }

        /// <inheritdoc/>
        public IMessageProcessor CreateProcessor(BotMessage message)
        {
            return _getProcessors().FirstOrDefault(p => p.IsRelevant(message)) ?? _getUnsupportedCommandProcessor();
        }
    }
}
