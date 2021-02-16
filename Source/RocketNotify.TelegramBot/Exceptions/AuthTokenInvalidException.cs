namespace RocketNotify.TelegramBot.Exceptions
{
    using System;

    /// <summary>
    /// Represents an error that occurs when an invalid auth token is provided to the telegram client.
    /// </summary>
    public class AuthTokenInvalidException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthTokenInvalidException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AuthTokenInvalidException(string message)
            : base(message)
        {
        }
    }
}
