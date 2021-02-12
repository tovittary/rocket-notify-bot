namespace RocketNotify.Subscription.Data
{
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Model;

    /// <summary>
    /// Notifications subscribers repository interface.
    /// </summary>
    public interface ISubscribersRepository
    {
        /// <summary>
        /// Creates the subscriber to notifications with the specified chat id.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns>A task that represents the subscriber creation process.</returns>
        Task CreateSubscriberAsync(long chatId);

        /// <summary>
        /// Deletes the subscriber with the specified chat id.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns>A task that represents the subscriber deletion process.</returns>
        Task DeleteSubscriberAsync(long chatId);

        /// <summary>
        /// Gets all subscribers.
        /// </summary>
        /// <returns>Subscribers data.</returns>
        Task<Subscriber[]> GetAllSubscribersAsync();
    }
}
