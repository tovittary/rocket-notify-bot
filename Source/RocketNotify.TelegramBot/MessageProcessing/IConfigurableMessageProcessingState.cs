namespace RocketNotify.TelegramBot.MessageProcessing
{
    /// <summary>
    /// Provides operations for configuring the message processing state.
    /// </summary>
    internal interface IConfigurableMessageProcessingState : IMessageProcessingState
    {
        /// <summary>
        /// Sets the message processor associated with the state.
        /// </summary>
        /// <param name="processor">The message processor.</param>
        void SetProcessor(IStatefulMessageProcessor processor);
    }
}
