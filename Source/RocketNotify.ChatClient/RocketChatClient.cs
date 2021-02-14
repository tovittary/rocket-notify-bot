namespace RocketNotify.ChatClient
{
    using System;
    using System.Threading.Tasks;

    using RocketNotify.ChatClient.ApiClient;
    using RocketNotify.ChatClient.Model;
    using RocketNotify.ChatClient.Settings;

    /// <summary>
    /// Rocket.Chat client.
    /// </summary>
    public class RocketChatClient : IRocketChatClient
    {
        /// <summary>
        /// REST API Rocket.Chat client.
        /// </summary>
        private readonly IRestApiClient _restApiClient;

        /// <summary>
        /// Rocket.Chat client settings provider.
        /// </summary>
        private readonly IClientSettingsProvider _settingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketChatClient"/> class.
        /// </summary>
        /// <param name="restApiClient">REST API Rocket.Chat client.</param>
        /// <param name="settingsProvider">Rocket.Chat client settings provider.</param>
        public RocketChatClient(IRestApiClient restApiClient, IClientSettingsProvider settingsProvider)
        {
            _restApiClient = restApiClient;
            _settingsProvider = settingsProvider;
        }

        /// <inheritdoc />
        public Task InitializeAsync()
        {
            var serverHost = _settingsProvider.GetServer();
            if (string.IsNullOrWhiteSpace(serverHost))
                throw new InvalidOperationException("Rocket.Chat server host cannot be empty.");

            var authenticationData = new AuthenticationData
            {
                User = _settingsProvider.GetUserName(),
                Password = _settingsProvider.GetPassword(),
                AuthToken = _settingsProvider.GetAuthToken()
            };

            var authDataValid = !string.IsNullOrEmpty(authenticationData.AuthToken)
                || (!string.IsNullOrEmpty(authenticationData.User) && !string.IsNullOrEmpty(authenticationData.Password));

            if (!authDataValid)
                throw new InvalidOperationException("Either AuthToken or both UserName and Password must be provided.");

            return _restApiClient.AuthenticateAsync(serverHost, authenticationData);
        }

        /// <inheritdoc />
        public async Task<DateTime> GetLastMessageTimeStampAsync()
        {
            var groupName = _settingsProvider.GetGroupName();
            if (string.IsNullOrWhiteSpace(groupName))
                throw new InvalidOperationException("Rocket.Chat group chat name cannot be empty.");

            var lastMessage = await _restApiClient.GetLastMessageInGroupAsync(groupName).ConfigureAwait(false);
            return lastMessage?.TimeStamp ?? DateTime.MinValue;
        }
    }
}
