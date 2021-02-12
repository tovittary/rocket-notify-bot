namespace RocketNotify.TelegramBot.Interfaces
{
    using System.Threading.Tasks;

    using Telegram.Bot.Types;

    /// <summary>
    /// Provides functionality for processing messages from the bot.
    /// </summary>
    public interface IBotMessageProcessor
    {
        /// <summary>
        /// Performs the message processing.
        /// </summary>
        /// <param name="message">Message instance.</param>
        /// <param name="client">Client used for sending Telegram messages.</param>
        /// <returns>A task that represents the message processing process.</returns>
        Task ProcessMessageAsync(Message message, ITelegramBotMessageSender client);
    }
}
