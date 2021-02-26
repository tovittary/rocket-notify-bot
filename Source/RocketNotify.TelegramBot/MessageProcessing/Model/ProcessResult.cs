namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    /// <summary>
    /// Describes a message processing result.
    /// </summary>
    /// <seealso cref="Message"/>.
    internal class ProcessResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether processing of the message has completed and no more messages are expected.
        /// </summary>
        public bool IsFinal { get; set; }
    }
}
