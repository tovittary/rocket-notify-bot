namespace RocketNotify.TelegramBot.MessageProcessing
{
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Provides functionality of invoking the message processor.
    /// </summary>
    internal interface IMessageProcessorInvoker
    {
        /// <summary>
        /// Gets the processor of handling the provided message.
        /// </summary>
        /// <param name="message">The message that should be processed.</param>
        /// <returns>The message processor.</returns>
        IMessageProcessor GetProcessor(Message message);
    }
}
