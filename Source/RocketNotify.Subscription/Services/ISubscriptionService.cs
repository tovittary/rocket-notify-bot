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
        /// <param name="secretKey">The secret subscription key.</param>
        /// <returns>A task that represents the adding the subscription process.</returns>
        Task AddSubscriptionAsync(long chatId, string secretKey);

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

        /// <summary>
        /// Checks if the potential subscriber needs to provide the secret key to subscribe.
        /// </summary>
        /// <returns><c>true</c> if the secret key is needed, <c>false</c> otherwise.</returns>
        bool CheckSubscriptionKeyNeeded();
    }
}
