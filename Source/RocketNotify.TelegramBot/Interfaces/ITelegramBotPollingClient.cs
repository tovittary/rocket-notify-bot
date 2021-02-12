namespace RocketNotify.TelegramBot.Interfaces
{
    using System;
    using System.Threading;

    /// <summary>
    /// An interface of a Telegram bot client using a polling mechanism o receive messages.
    /// </summary>
    public interface ITelegramBotPollingClient : ITelegramBotMessageSender, IDisposable
    {
        /// <summary>
        /// Initializes the client.
        /// </summary>
        void Initialize();

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
