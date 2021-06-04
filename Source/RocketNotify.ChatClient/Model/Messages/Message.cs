namespace RocketNotify.ChatClient.Model.Messages
{
    using System;

    /// <summary>
    /// Chat message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the message timestamp.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// Gets or sets user mentions in the message.
        /// </summary>
        public Mention[] Mentions { get; set; }
    }
}
