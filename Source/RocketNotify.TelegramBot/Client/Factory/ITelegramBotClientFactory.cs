namespace RocketNotify.TelegramBot.Client.Factory
{
    using Telegram.Bot;

    /// <summary>
    /// Provides operations for getting implementations of the <see cref="ITelegramBotClient"/> interface.
    /// </summary>
    public interface ITelegramBotClientFactory
    {
        /// <summary>
        /// Gets an implementation of the <see cref="ITelegramBotClient"/> interface.
        /// </summary>
        /// <returns>An implementation of the <see cref="ITelegramBotClient"/> interface.</returns>
        ITelegramBotClient GetClient();
    }
}
