namespace RocketNotify.Subscription.Model
{
    /// <summary>
    /// Configures how the subscriber should be notified of user mentions in chat messages.
    /// </summary>
    public class MentionConfiguration
    {
        /// <summary>
        /// Gets or sets the username that is mentioned in a message.
        /// </summary>
        public string MentionedUsername { get; set; }

        /// <summary>
        /// Gets or sets the text of the notification to be generated.
        /// </summary>
        public string NotificationText { get; set; }
    }
}
