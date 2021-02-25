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
        /// Processes the provided message as an async operation.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>A task object that represents the message processing operation.</returns>
        Task<ProcessResult> ProcessAsync(Message message);
    }
}
