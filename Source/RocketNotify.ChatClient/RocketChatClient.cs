namespace RocketNotify.ChatClient
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using RocketNotify.ChatClient.ApiClient;
    using RocketNotify.ChatClient.Dto.Messages;
    using RocketNotify.ChatClient.Model;
    using RocketNotify.ChatClient.Model.Messages;
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
        /// The most recent message timestamp.
        /// </summary>
        private DateTime _lastMessageTimeStamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketChatClient"/> class.
        /// </summary>
        /// <param name="restApiClient">REST API Rocket.Chat client.</param>
        /// <param name="settingsProvider">Rocket.Chat client settings provider.</param>
        public RocketChatClient(IRestApiClient restApiClient, IClientSettingsProvider settingsProvider)
        {
            _restApiClient = restApiClient;
            _settingsProvider = settingsProvider;

            _lastMessageTimeStamp = default;
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
            UpdateLastMessageTimeStamp(lastMessage);

            return _lastMessageTimeStamp;
        }

        /// <inheritdoc />
        public async Task<Message[]> GetRecentMessagesAsync()
        {
            var groupName = _settingsProvider.GetGroupName();
            if (string.IsNullOrWhiteSpace(groupName))
                throw new InvalidOperationException("Rocket.Chat group chat name cannot be empty.");

            var messageCount = _settingsProvider.GetRequestedMessageCount();
            if (messageCount == 0)
                throw new InvalidOperationException("The number of chat messages to request cannot be less than 1.");

            var recentMessagesDto = await GetNewMessagesAsync(groupName, messageCount).ConfigureAwait(false);
            var messages = recentMessagesDto.Select(dto => MessageMapping.ToMessage(dto)).ToArray();

            return messages;
        }

        /// <summary>
        /// Returns only new messages that were sent to chat after the <see cref="_lastMessageTimeStamp"/> time stamp.
        /// </summary>
        /// <param name="groupName">Group chat name.</param>
        /// <param name="messageCount">Number of messages to get.</param>
        /// <returns>New chat messages.</returns>
        private async Task<MessageDto[]> GetNewMessagesAsync(string groupName, int messageCount)
        {
            var messages = await _restApiClient.GetRecentMessagesInGroupAsync(groupName, messageCount).ConfigureAwait(false);
            if (messages.Length == 0)
                return messages;

            var recentMessages = messages.TakeWhile(m => m.TimeStamp > _lastMessageTimeStamp).ToArray();
            if (recentMessages.Length == 0)
                return recentMessages;

            var latestMessage = recentMessages.First();
            UpdateLastMessageTimeStamp(latestMessage);

            return recentMessages;
        }

        /// <summary>
        /// Updates the most recent message time stamp.
        /// </summary>
        /// <param name="latestMessage">The latest message received from chat.</param>
        private void UpdateLastMessageTimeStamp(MessageDto latestMessage)
        {
            if (latestMessage == null)
                return;

            _lastMessageTimeStamp = latestMessage.TimeStamp;
        }
    }
}
