namespace RocketNotify.TelegramBot.Messages.Filtration
{
    using System.Linq;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    /// <summary>
    /// Filters a message based on whether it contains a command.
    /// </summary>
    public class ContainsCommandMessageFilter : IChainedMessageFilter
    {
        /// <inheritdoc/>
        public bool Filter(Message message)
        {
            if (message.Entities == null)
                return false;

            if (message.Entities.Any(e => e.Type == MessageEntityType.BotCommand))
                return true;

            return false;
        }

        /// <inheritdoc/>
        public void SetNextFilter(IMessageFilter nextFilter)
        {
        }
    }
}
