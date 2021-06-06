namespace RocketNotify.Notification.Model
{
    using System;

    /// <summary>
    /// Aggregated data on messages received from Rocket.Chat.
    /// </summary>
    public class MessagesData
    {
        /// <summary>
        /// Gets or sets the latest group chat message timestamp.
        /// </summary>
        public DateTime LatestMessageTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the usernames mentioned in messages.
        /// </summary>
        public string[] MentionedUsernames { get; set; }
    }
}
