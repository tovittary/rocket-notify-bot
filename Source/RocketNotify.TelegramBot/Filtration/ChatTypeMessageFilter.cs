namespace RocketNotify.TelegramBot.Filtration
{
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    /// <summary>
    /// Filters a message based on the type of chat it came from.
    /// </summary>
    public class ChatTypeMessageFilter : IChainedMessageFilter
    {
        /// <summary>
        /// Next message filter.
        /// </summary>
        private IMessageFilter _nextFilter;

        /// <inheritdoc/>
        public bool Filter(Message message)
        {
            if (message.Chat.Type == ChatType.Supergroup || message.Chat.Type == ChatType.Channel)
                return false;

            return _nextFilter.Filter(message);
        }

        /// <inheritdoc/>
        public void SetNextFilter(IMessageFilter nextFilter) => _nextFilter = nextFilter;
    }
}
