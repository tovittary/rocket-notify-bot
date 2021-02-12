namespace RocketNotify.ChatClient.Exceptions
{
    using System;

    /// <inheritdoc />
    public class RocketChatApiException : Exception
    {
        /// <inheritdoc />
        public RocketChatApiException(string message) : base(message)
        {
        }
    }
}
