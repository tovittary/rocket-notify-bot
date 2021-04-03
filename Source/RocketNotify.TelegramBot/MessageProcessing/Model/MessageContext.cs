namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    /// <summary>
    /// Contains information about the message processing context.
    /// </summary>
    public class MessageContext
    {
        /// <summary>
        /// Gets or sets the last message from a user.
        /// </summary>
        public BotMessage LastMessage { get; set; }

        /// <summary>
        /// Gets or sets the last response to the message received.
        /// </summary>
        public BotMessage LastResponse { get; set; }
    }
}
