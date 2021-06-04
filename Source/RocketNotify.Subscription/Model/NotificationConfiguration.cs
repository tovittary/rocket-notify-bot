namespace RocketNotify.Subscription.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Configures how the subscriber should be notified of chat messages.
    /// </summary>
    public class NotificationConfiguration
    {
        /// <summary>
        /// Gets or sets the settings for notifications about user mentions.
        /// </summary>
        public ICollection<MentionConfiguration> Mentions { get; set; }
    }
}
