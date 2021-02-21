namespace RocketNotify.TelegramBot.Commands
{
    using Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Represents a command execution result.
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Gets or sets the reply text.
        /// </summary>
        public string ReplyText { get; set; }

        /// <summary>
        /// Gets or sets markup of the reply message.
        /// </summary>
        public IReplyMarkup ReplyMarkup { get; set; }
    }
}
