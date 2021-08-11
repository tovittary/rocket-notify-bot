namespace RocketNotify.ChatClient.Exceptions
{
    using System;

    /// <summary>
    /// Represents an error that occured during Rocket.Chat REST API request.
    /// </summary>
    public class RocketChatApiException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RocketChatApiException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RocketChatApiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketChatApiException"/> class.
        /// </summary>
        /// <param name="innerException">Source exception instance.</param>
        public RocketChatApiException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }
    }
}
