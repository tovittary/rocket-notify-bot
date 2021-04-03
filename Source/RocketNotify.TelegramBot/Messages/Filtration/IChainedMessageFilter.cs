namespace RocketNotify.TelegramBot.Messages.Filtration
{
    /// <summary>
    /// An interface that provides operations for configuring the message filters chain.
    /// </summary>
    public interface IChainedMessageFilter : IMessageFilter
    {
        /// <summary>
        /// Sets the next message filter in the chain.
        /// </summary>
        /// <param name="nextFilter">The next message filter in the chain.</param>
        void SetNextFilter(IMessageFilter nextFilter);
    }
}
