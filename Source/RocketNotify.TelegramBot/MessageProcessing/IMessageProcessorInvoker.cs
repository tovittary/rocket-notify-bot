namespace RocketNotify.TelegramBot.MessageProcessing
{
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Provides functionality of invoking the message processor.
    /// </summary>
    public interface IMessageProcessorInvoker
    {
        /// <summary>
        /// Invokes message processing with an appropriate processor.
        /// </summary>
        /// <param name="message">The message that should be processed.</param>
        /// <returns>A task representing the message processing operation.</returns>
        Task InvokeAsync(BotMessage message);
    }
}
