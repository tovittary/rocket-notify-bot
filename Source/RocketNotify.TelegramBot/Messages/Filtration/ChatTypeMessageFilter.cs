namespace RocketNotify.TelegramBot.Messages.Filtration
{
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    /// <summary>
    /// Filters a message based on the type of chat it came from.
    /// </summary>
    public class ChatTypeMessageFilter : IMessageFilter
    {
        /// <inheritdoc/>
        public FiltrationResult Filter(Message message)
        {
            if (message.Chat.Type == ChatType.Supergroup || message.Chat.Type == ChatType.Channel)
                return FiltrationResult.Ignore();

            if (message.Chat.Type == ChatType.Private)
                return FiltrationResult.NextFilter(typeof(ContainsCommandMessageFilter));

            return FiltrationResult.NextFilter(typeof(BotMentionMessageFilter));
        }
    }
}
