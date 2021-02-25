namespace RocketNotify.TelegramBot.MessageProcessing
{
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Provides functionality of storing message processors.
    /// </summary>
    internal interface IMessageProcessorStorage
    {
        /// <summary>
        /// Adds the message processor to the storage.
        /// </summary>
        /// <param name="processor">The processor to store.</param>
        void StoreProcessor(IMessageProcessor processor);

        /// <summary>
        /// Gets an existing processor for handling the provided message.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>An instance of the existing processor to handle the message. <c>null</c> if no such processor exists.</returns>
        IMessageProcessor GetExisting(Message message);
    }
}
