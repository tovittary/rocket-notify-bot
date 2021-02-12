namespace RocketNotify.ChatClient.Settings
{
    /// <summary>
    /// Rocket.Chat client settings provider interface.
    /// </summary>
    public interface IClientSettingsProvider
    {
        /// <summary>
        /// Gets the Rocket.Chat server host.
        /// </summary>
        /// <returns>Rocket.Chat server host.</returns>
        string GetServer();

        /// <summary>
        /// Gets the user name for client authentication.
        /// </summary>
        /// <returns>User name.</returns>
        string GetUserName();

        /// <summary>
        /// Gets the password for client authentication.
        /// </summary>
        /// <returns>Password.</returns>
        string GetPassword();

        /// <summary>
        /// Gets the existing resumable authorization token.
        /// </summary>
        /// <returns>Authorization token.</returns>
        string GetAuthToken();

        /// <summary>
        /// Gets the name of the private or public group in Rocket.Chat from which the messages are going to be received.
        /// </summary>
        /// <returns>The name of the group.</returns>
        string GetGroupName();
    }
}
