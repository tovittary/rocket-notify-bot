namespace RocketNotify.Subscription.Exceptions
{
    using System;

    /// <summary>
    /// Represents an error that occurs when the subscriber with the specified chat identifier is not found.
    /// </summary>
    public class SubscriberNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriberNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SubscriberNotFoundException(string message)
            : base(message)
        {
        }
    }
}
