namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    using RocketNotify.TelegramBot.MessageProcessing.Model.Markups;

    /// <summary>
    /// Describes a message.
    /// </summary>
    public class BotMessage
    {
        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        public int MessageId { get; set; }

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
        public MessageReply ReplyInfo { get; set; }

        /// <summary>
        /// Gets or sets the message markup.
        /// </summary>
        public IMessageMarkup Markup { get; set; }
    }
}
