namespace RocketNotify.Subscription.Exceptions
{
    using System;

    /// <summary>
    /// Represents an error that occurs when the subscriber with the same chat identifier already exists.
    /// </summary>
    public class SubscriberAlreadyExistsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriberAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SubscriberAlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}
