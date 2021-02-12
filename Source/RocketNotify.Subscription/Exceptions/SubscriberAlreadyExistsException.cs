namespace RocketNotify.Subscription.Exceptions
{
    using System;

    /// <inheritdoc />
    public class SubscriberAlreadyExistsException : Exception
    {
        /// <inheritdoc />
        public SubscriberAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
