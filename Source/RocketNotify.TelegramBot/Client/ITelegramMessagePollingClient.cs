namespace RocketNotify.TelegramBot.Client
{
    using System.Threading;

    /// <summary>
    /// An interface of a Telegram client using a polling mechanism to receive messages.
    /// </summary>
    public interface ITelegramMessagePollingClient : ITelegramMessageSender
    {
        /// <summary>
        /// Starts the messages polling process.
        /// </summary>
        /// <param name="token">A token for stopping the messages polling process.</param>
        void StartPolling(CancellationToken token);

        /// <summary>
        /// Manually stops the messages polling process.
        /// </summary>
        void StopPolling();
    }
}
