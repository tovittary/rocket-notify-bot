namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    /// <summary>
    /// Describes a message processing result.
    /// </summary>
    /// <seealso cref="BotMessage"/>.
    public class ProcessResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether processing of the message has completed and no more messages are expected.
        /// </summary>
        public bool IsFinal { get; set; }

        /// <summary>
        /// Creates an instance of the <see cref="ProcessResult"/> class signaling the completion of message processing.
        /// </summary>
        /// <returns>Instance of the <see cref="ProcessResult"/> class.</returns>
        public static ProcessResult Final()
        {
            return new ProcessResult { IsFinal = true };
        }
    }
}
