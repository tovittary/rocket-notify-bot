namespace RocketNotify.TelegramBot.MessageProcessing
{
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Functionality of a message processor that stores data about processed messages.
    /// Able to process a chain of related messages.
    /// </summary>
    internal interface IStatefulMessageProcessor : IMessageProcessor
    {
        /// <summary>
        /// Gets the context of message processing.
        /// </summary>
        MessageContext Context { get; }

        /// <summary>
        /// Gets the current message processing state.
        /// </summary>
        IMessageProcessingState CurrentState { get; }

        /// <summary>
        /// Sets the new state of message processing.
        /// </summary>
        /// <param name="newState">The new processing state.</param>
        void ChangeCurrentState(IConfigurableMessageProcessingState newState);
    }
}
