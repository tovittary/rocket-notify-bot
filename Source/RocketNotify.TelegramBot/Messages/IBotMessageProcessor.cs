namespace RocketNotify.TelegramBot.Messages
{
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.Client;

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
        /// <param name="messageSender">A service used for sending Telegram messages.</param>
        /// <returns>A task that represents the message processing process.</returns>
        Task ProcessMessageAsync(Message message, ITelegramMessageSender messageSender);
    }
}
