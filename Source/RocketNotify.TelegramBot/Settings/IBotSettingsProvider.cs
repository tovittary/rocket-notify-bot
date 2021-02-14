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

        /// <summary>
        /// Gets the bot username, by which he is addressed in chats.
        /// </summary>
        /// <returns>The bot username.</returns>
        string GetBotUserName();
    }
}
