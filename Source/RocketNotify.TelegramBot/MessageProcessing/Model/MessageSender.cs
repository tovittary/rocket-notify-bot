namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    /// <summary>
    /// Describes the message sender.
    /// </summary>
    internal class MessageSender
    {
        /// <summary>
        /// Gets or sets the message sender identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the message sender name.
        /// </summary>
        public string Name { get; set; }
    }
}
