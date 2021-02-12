namespace RocketNotify.Subscription.Services
{
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Model;

    /// <summary>
    /// Provides a functionality for managing notifications subscriptions.
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Adds a chat subscription to notifications.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns>A task that represents the adding the subscription process.</returns>
        Task AddSubscriptionAsync(long chatId);

        /// <summary>
        /// Removes a chat subscription to notifications.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns>A task that represents the removing the subscription process.</returns>
        Task RemoveSubscriptionAsync(long chatId);

        /// <summary>
        /// Gets all subscriptions.
        /// </summary>
        /// <returns>Subscriptions data.</returns>
        Task<Subscriber[]> GetAllSubscriptionsAsync();
    }
}