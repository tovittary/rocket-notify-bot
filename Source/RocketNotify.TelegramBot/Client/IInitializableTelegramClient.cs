namespace RocketNotify.TelegramBot.Client
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides operations for initializing a Telegram client.
    /// </summary>
    public interface IInitializableTelegramClient
    {
        /// <summary>
        /// Initializes a client.
        /// </summary>
        /// <returns>A task that represents an initialization process.</returns>
        Task Initialize();
    }
}
