namespace RocketNotify.TelegramBot.Commands
{
    using Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Represents a command execution result.
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Gets or sets the response text.
        /// </summary>
        public string ResponseText { get; set; }

        /// <summary>
        /// Gets or sets the markup that a user can use to reply to the bot message.
        /// </summary>
        public IReplyMarkup ReplyMarkup { get; set; }
    }
}
