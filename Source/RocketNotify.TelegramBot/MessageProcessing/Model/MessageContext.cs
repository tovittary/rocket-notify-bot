namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    /// <summary>
    /// Contains information about the message processing context.
    /// </summary>
    internal class MessageContext
    {
        /// <summary>
        /// Gets or sets the last message from a user.
        /// </summary>
        public Message LastMessage { get; set; }

        /// <summary>
        /// Gets or sets the last response to the message received.
        /// </summary>
        public Message LastResponse { get; set; }
    }
}
