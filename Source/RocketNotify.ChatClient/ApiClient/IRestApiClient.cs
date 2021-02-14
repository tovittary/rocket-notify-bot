namespace RocketNotify.ChatClient.ApiClient
{
    using System;
    using System.Threading.Tasks;

    using RocketNotify.ChatClient.Dto.Messages;
    using RocketNotify.ChatClient.Model;

    /// <summary>
    /// Provides functionality for Rocket.Chat REST API access.
    /// </summary>
    public interface IRestApiClient : IDisposable
    {
        /// <summary>
        /// Gets authorization data for sending API requests.
        /// </summary>
        AuthorizationData AuthData { get; }

        /// <summary>
        /// Authenticates on the Rocket.Chat server.
        /// </summary>
        /// <param name="server">Rocket.Chat server host.</param>
        /// <param name="authData">Authentication data.</param>
        /// <returns>The task object that represents the authentication process.</returns>
        Task AuthenticateAsync(string server, AuthenticationData authData);

        /// <summary>
        /// Gets the latest message in a group chat with a specified name.
        /// </summary>
        /// <param name="groupName">Group chat name.</param>
        /// <returns>DTO containing the latest message data.</returns>
        Task<MessageDto> GetLastMessageInGroupAsync(string groupName);
    }
}
