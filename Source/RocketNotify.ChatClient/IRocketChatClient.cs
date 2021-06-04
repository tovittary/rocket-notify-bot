namespace RocketNotify.ChatClient
{
    using System;
    using System.Threading.Tasks;

    using RocketNotify.ChatClient.Model.Messages;

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

        /// <summary>
        /// Gets recent messages in a group chat.
        /// </summary>
        /// <returns>Recent messages in a group chat.</returns>
        Task<Message[]> GetRecentMessagesAsync();
    }
}
