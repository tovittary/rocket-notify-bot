namespace RocketNotify.TelegramBot.Messages.Filtration
{
    using Telegram.Bot.Types;

    /// <summary>
    /// Filters a message based on its type.
    /// </summary>
    public class MessageTypeMessageFilter : IInitialMessageFilter
    {
        /// <inheritdoc/>
        public FiltrationResult Filter(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return FiltrationResult.Ignore();

            return FiltrationResult.NextFilter(typeof(ChatTypeMessageFilter));
        }
    }
}
