namespace RocketNotify.Subscription.Exceptions
{
    using System;

    /// <summary>
    /// Represents an error that occurs while managing subscribers.
    /// </summary>
    public class SubscriberOperationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriberOperationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SubscriberOperationException(string message)
            : base(message)
        {
        }
    }
}
