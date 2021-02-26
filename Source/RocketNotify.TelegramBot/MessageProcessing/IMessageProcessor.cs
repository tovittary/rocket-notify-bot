namespace RocketNotify.TelegramBot.MessageProcessing
{
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// An interface that describes a message processor that doesn't have any states and can be used for processing a single message.
    /// </summary>
    internal interface IMessageProcessor
    {
        /// <summary>
        /// Checks if the provided message is relevant in the current state of message processor.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <returns><c>true</c> if the message can be processed in this state, <c>false</c> otherwise.</returns>
        bool IsRelevant(Message message);

        /// <summary>
        /// Processes the provided message as an async operation.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>A task object containing the result of message processing.</returns>
        Task<ProcessResult> ProcessAsync(Message message);
    }
}
