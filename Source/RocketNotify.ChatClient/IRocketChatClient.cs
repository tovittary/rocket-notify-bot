namespace RocketNotify.ChatClient
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides functionality for getting data from Rocket.Chat.
    /// </summary>
    public interface IRocketChatClient
    {
        /// <summary>
        /// Initializes the client.
        /// </summary>
        /// <returns>The client initialization task.</returns>
        Task InitializeAsync();

        /// <summary>
        /// Gets the latest group chat message timestamp.
        /// </summary>
        /// <returns>The latest group chat message timestamp.</returns>
        Task<DateTime> GetLastMessageTimeStampAsync();
    }
}
