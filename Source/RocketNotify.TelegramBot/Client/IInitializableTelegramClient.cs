namespace RocketNotify.TelegramBot.Client
{
    /// <summary>
    /// Provides operations for initializing a Telegram client.
    /// </summary>
    public interface IInitializableTelegramClient
    {
        /// <summary>
        /// Initializes a client.
        /// </summary>
        void Initialize();
    }
}
