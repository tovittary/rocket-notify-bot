namespace RocketNotify.TelegramBot.MessageProcessing
{
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Provides functionality for obtaining new instances of message processors.
    /// </summary>
    public interface IMessageProcessorFactory
    {
        /// <summary>
        /// Creates a new instance of message processor capable of handling the provided message.
        /// </summary>
        /// <param name="message">The message for which the processor is being created.</param>
        /// <returns>The message processor instance.</returns>
        IMessageProcessor CreateProcessor(BotMessage message);
    }
}
