namespace RocketNotify.TelegramBot.Client.Factory
{
    using System;

    using RocketNotify.TelegramBot.Settings;

    using Telegram.Bot;

    /// <summary>
    /// Factory for getting the <see cref="ITelegramBotClient"/> interface implementations.
    /// </summary>
    public class TelegramBotClientFactory : ITelegramBotClientFactory
    {
        /// <summary>
        /// Telegram bot settings provider.
        /// </summary>
        private readonly IBotSettingsProvider _settingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotClientFactory"/> class.
        /// </summary>
        /// <param name="settingsProvider">Telegram bot settings provider.</param>
        public TelegramBotClientFactory(IBotSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        /// <inheritdoc/>
        public ITelegramBotClient GetClient()
        {
            var authToken = _settingsProvider.GetAuthToken();
            if (string.IsNullOrEmpty(authToken))
                throw new InvalidOperationException("Telegram bot AuthToken cannot be empty. Check the appsettings.json file.");

            return new TelegramBotClient(authToken);
        }
    }
}
