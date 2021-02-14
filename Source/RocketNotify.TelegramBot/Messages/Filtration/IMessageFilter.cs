namespace RocketNotify.TelegramBot.Messages.Filtration
{
    using Telegram.Bot.Types;

    /// <summary>
    /// Represents bot messages filtration functionality.
    /// </summary>
    public interface IMessageFilter
    {
        /// <summary>
        /// Filters the message.
        /// </summary>
        /// <param name="message">The message received by the bot.</param>
        /// <returns>Description of the filtering result, including suggested actions to take.</returns>
        FiltrationResult Filter(Message message);
    }
}
