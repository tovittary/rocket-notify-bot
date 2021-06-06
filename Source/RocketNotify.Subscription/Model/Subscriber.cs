namespace RocketNotify.Subscription.Model
{
    /// <summary>
    /// Represents a notifications subscriber.
    /// </summary>
    public class Subscriber
    {
        /// <summary>
        /// Gets or sets the subscriber chat identifier.
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// Gets or sets the notification configuration for the subscriber.
        /// </summary>
        public NotificationConfiguration Configuration { get; set; }
    }
}
