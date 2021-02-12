namespace RocketNotify.Subscription.Exceptions
{
    using System;

    /// <inheritdoc />
    public class SubscriberOperationException : Exception
    {
        /// <inheritdoc />
        public SubscriberOperationException(string message) : base(message)
        {
        }
    }
}
