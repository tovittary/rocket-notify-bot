namespace RocketNotify.Notification.Model
{
    /// <summary>
    /// A notification message.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Gets or sets the telegram chat id to send the notification to.
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// Gets or sets the text of the notification.
        /// </summary>
        public string Text { get; set; }
    }
}
