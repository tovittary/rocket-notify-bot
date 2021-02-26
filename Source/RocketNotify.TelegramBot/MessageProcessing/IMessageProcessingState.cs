namespace RocketNotify.TelegramBot.MessageProcessing
{
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Provides operations for processing the message.
    /// </summary>
    internal interface IMessageProcessingState
    {
        /// <summary>
        /// Gets a value indicating whether the state is final.
        /// </summary>
        bool IsFinal { get; }

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
        /// <returns>A task object containing the response to the message.</returns>
        Task<Message> ProcessAsync(Message message);
    }
}
