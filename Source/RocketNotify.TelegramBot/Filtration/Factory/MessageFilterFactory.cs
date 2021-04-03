namespace RocketNotify.TelegramBot.Filtration.Factory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RocketNotify.TelegramBot.Filtration;

    /// <inheritdoc />
    public class MessageFilterFactory : IMessageFilterFactory
    {
        /// <summary>
        /// A delegate used for obtaining a collection of message filters.
        /// </summary>
        private readonly Func<IEnumerable<IChainedMessageFilter>> _getMessageFilters;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFilterFactory"/> class.
        /// </summary>
        /// <param name="getMessageFilters">A delegate used for obtaining a collection of message filters.</param>
        public MessageFilterFactory(Func<IEnumerable<IChainedMessageFilter>> getMessageFilters)
        {
            _getMessageFilters = getMessageFilters;
        }

        /// <inheritdoc/>
        public IMessageFilter GetFilter()
        {
            var messageFilters = _getMessageFilters().ToArray();
            return SetUpFilterChain(messageFilters);
        }

        /// <summary>
        /// Sets up a chain of message filters.
        /// </summary>
        /// <param name="messageFilters">A collection of message filters.</param>
        /// <returns>A first filter in the message filtration chain.</returns>
        private IMessageFilter SetUpFilterChain(IChainedMessageFilter[] messageFilters)
        {
            if (!messageFilters.Any())
                throw new ArgumentException("No message filters found");

            if (messageFilters.Length == 1)
                return messageFilters.First();

            for (var i = 0; i < messageFilters.Length - 1; i++)
            {
                var filter = messageFilters[i];
                var nextFilter = messageFilters[i + 1];

                filter.SetNextFilter(nextFilter);
            }

            return messageFilters.First();
        }
    }
}
