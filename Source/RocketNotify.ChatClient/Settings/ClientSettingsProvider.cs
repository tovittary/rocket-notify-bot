namespace RocketNotify.ChatClient.Settings
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Rocket.Chat client settings provider.
    /// </summary>
    public class ClientSettingsProvider : IClientSettingsProvider
    {
        /// <summary>
        /// Application settings.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSettingsProvider"/> class.
        /// </summary>
        /// <param name="configuration">Application settings.</param>
        public ClientSettingsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        public string GetServer()
        {
            var server = _configuration.GetSection("RocketChat")?["Server"] ?? string.Empty;
            if (!string.IsNullOrEmpty(server))
                server = server.TrimEnd('/');

            return server;
        }

        /// <inheritdoc />
        public string GetUserName()
        {
            return _configuration.GetSection("RocketChat")?["UserName"] ?? string.Empty;
        }

        /// <inheritdoc />
        public string GetPassword()
        {
            return _configuration.GetSection("RocketChat")?["Password"] ?? string.Empty;
        }

        /// <inheritdoc />
        public string GetAuthToken()
        {
            return _configuration.GetSection("RocketChat")?["AuthToken"] ?? string.Empty;
        }

        /// <inheritdoc />
        public string GetGroupName()
        {
            var groupName = _configuration.GetSection("RocketChat")?["GroupName"] ?? string.Empty;
            if (!string.IsNullOrEmpty(groupName))
                groupName = groupName.ToLowerInvariant();

            return groupName;
        }
    }
}
