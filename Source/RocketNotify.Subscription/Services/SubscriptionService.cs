namespace RocketNotify.Subscription.Services
{
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Data;
    using RocketNotify.Subscription.Model;

    /// <summary>
    /// Notifications subscriptions managing service.
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        /// <summary>
        /// Notifications subscribers repository.
        /// </summary>
        private readonly ISubscribersRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionService"/> class.
        /// </summary>
        /// <param name="repository">Notifications subscribers repository.</param>
        public SubscriptionService(ISubscribersRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc />
        public Task AddSubscriptionAsync(long chatId) =>
            _repository.CreateSubscriberAsync(chatId);

        /// <inheritdoc />
        public Task RemoveSubscriptionAsync(long chatId) =>
            _repository.DeleteSubscriberAsync(chatId);

        /// <inheritdoc />
        public Task<Subscriber[]> GetAllSubscriptionsAsync() =>
            _repository.GetAllSubscribersAsync();
    }
}
