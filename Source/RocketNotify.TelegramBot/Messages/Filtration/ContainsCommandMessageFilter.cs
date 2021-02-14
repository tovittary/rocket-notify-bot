namespace RocketNotify.TelegramBot.Messages.Filtration
{
    using System.Linq;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    /// <summary>
    /// Filters a message based on whether it contains a command.
    /// </summary>
    public class ContainsCommandMessageFilter : IMessageFilter
    {
        /// <inheritdoc/>
        public FiltrationResult Filter(Message message)
        {
            if (message.Entities == null)
                return FiltrationResult.Ignore();

            if (message.Entities.Any(e => e.Type == MessageEntityType.BotCommand))
                return FiltrationResult.Process();

            return FiltrationResult.Ignore();
        }
    }
}
