namespace RocketNotify.TelegramBot.Settings
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Telegram bot settings provider.
    /// </summary>
    public class BotSettingsProvider : IBotSettingsProvider
    {
        /// <summary>
        /// Application settings.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotSettingsProvider"/> class.
        /// </summary>
        /// <param name="configuration">Application settings.</param>
        public BotSettingsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        public string GetAuthToken()
        {
            return _configuration.GetSection("Telegram")?["AuthToken"] ?? string.Empty;
        }

        /// <inheritdoc/>
        public string GetBotUserName()
        {
            var botName = _configuration.GetSection("Telegram")?["BotName"] ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(botName) && !botName.StartsWith('@'))
                botName = $"@{botName}";

            return botName;
        }
    }
}
