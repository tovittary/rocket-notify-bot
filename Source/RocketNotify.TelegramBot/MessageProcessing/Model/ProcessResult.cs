namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    /// <summary>
    /// Describes a message processing result.
    /// </summary>
    /// <seealso cref="Message"/>.
    internal class ProcessResult : Message
    {
        /// <summary>
        /// Gets or sets the message markup.
        /// </summary>
        public MessageMarkup Markup { get; set; }
    }
}
