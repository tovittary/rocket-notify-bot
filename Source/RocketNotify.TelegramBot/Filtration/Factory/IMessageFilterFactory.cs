namespace RocketNotify.TelegramBot.Filtration.Factory
{
    using RocketNotify.TelegramBot.Filtration;

    /// <summary>
    /// Provides functionality for obtaining message filtration units.
    /// </summary>
    public interface IMessageFilterFactory
    {
        /// <summary>
        /// Gets an instance of a message filter.
        /// </summary>
        /// <returns>Message filter.</returns>
        IMessageFilter GetFilter();
    }
}
