namespace RocketNotify.TelegramBot.Client
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides functionality for sending Telegram messages.
    /// </summary>
    public interface ITelegramMessageSender : IInitializableTelegramClient
    {
        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <param name="text">The message text.</param>
        /// <returns>A task that represents the message sending process.</returns>
        Task SendMessageAsync(long chatId, string text);
    }
}
