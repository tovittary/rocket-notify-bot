namespace RocketNotify.TelegramBot.Messages
{
    using System.Threading.Tasks;

    using Telegram.Bot.Types;

    /// <summary>
    /// Provides functionality for handling the event of a message received by the bot.
    /// </summary>
    public interface IBotMessageHandler
    {
        /// <summary>
        /// Handles the received message.
        /// </summary>
        /// <param name="message">Message instance.</param>
        /// <returns>The task object that represents the message handling process.</returns>
        Task HandleAsync(Message message);
    }
}
