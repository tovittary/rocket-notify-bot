namespace RocketNotify.Subscription.Exceptions
{
    using System;

    /// <inheritdoc />
    public class SubscriberNotFoundException : Exception
    {
        /// <inheritdoc />
        public SubscriberNotFoundException(string message) : base(message)
        {
        }
    }
}
