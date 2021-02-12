namespace RocketNotify.TelegramBot.Settings
{
    /// <summary>
    /// Telegram bot settings provider interface.
    /// </summary>
    public interface IBotSettingsProvider
    {
        /// <summary>
        /// Gets the bot access authorization token.
        /// </summary>
        /// <returns>The bot auth token.</returns>
        string GetAuthToken();
    }
}
