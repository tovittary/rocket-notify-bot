namespace RocketNotify.TelegramBot.MessageProcessing
{
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Functionality of a message processor.
    /// Able to process a chain of related messages.
    /// </summary>
    internal interface IMessageProcessor : IMessageProcessingState
    {
        /// <summary>
        /// Gets the current message processing state.
        /// </summary>
        IMessageProcessingState CurrentState { get; }

        /// <summary>
        /// Sets the new state of message processing.
        /// </summary>
        /// <param name="state">The new processing state.</param>
        void ChangeCurrentState(IMessageProcessingState state);

        /// <summary>
        /// Checks if the provided message is relevant to the processor, i.e. can be processed by it.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <returns><c>true</c> if the message can be processed by the processor, <c>false</c> otherwise.</returns>
        bool IsRelevant(Message message);
    }
}
