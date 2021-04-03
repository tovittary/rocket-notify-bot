namespace RocketNotify.TelegramBot.Messages.Filtration
{
    using Telegram.Bot.Types;

    /// <summary>
    /// Filters a message based on its type.
    /// </summary>
    public class MessageTypeMessageFilter : IChainedMessageFilter
    {
        /// <summary>
        /// Next message filter.
        /// </summary>
        private IMessageFilter _nextFilter;

        /// <inheritdoc/>
        public bool Filter(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return _nextFilter.Filter(message);
        }

        /// <inheritdoc/>
        public void SetNextFilter(IMessageFilter nextFilter) => _nextFilter = nextFilter;
    }
}
