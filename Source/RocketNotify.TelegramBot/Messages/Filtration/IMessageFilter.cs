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
        /// <returns><c>true</c> if the message should be handled, <c>false</c> otherwise.</returns>
        bool Filter(Message message);
    }
}
