namespace RocketNotify.Subscription.Exceptions
{
    using System;

    /// <summary>
    /// Represents an error that occurs when the chat is not allowed to subscribe to notifications.
    /// </summary>
    public class SubscriptionNotAllowedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionNotAllowedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SubscriptionNotAllowedException(string message)
            : base(message)
        {
        }
    }
}
