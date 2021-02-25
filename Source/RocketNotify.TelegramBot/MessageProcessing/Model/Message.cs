namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    /// <summary>
    /// Describes a message.
    /// </summary>
    internal class Message
    {
        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the message sender info.
        /// </summary>
        public MessageSender Sender { get; set; }

        /// <summary>
        /// Gets or sets the message reply info.
        /// </summary>
        public MessageReply Reply { get; set; }
    }
}
