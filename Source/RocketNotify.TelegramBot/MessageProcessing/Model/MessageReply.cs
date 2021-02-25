namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    /// <summary>
    /// Information about the message that the current message replies to.
    /// </summary>
    internal class MessageReply
    {
        /// <summary>
        /// Gets or sets the replied message identifier.
        /// </summary>
        public long SourceMessageId { get; set; }
    }
}
