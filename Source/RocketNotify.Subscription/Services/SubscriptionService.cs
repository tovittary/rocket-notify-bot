namespace RocketNotify.Subscription.Services
{
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Data;
    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Model;
    using RocketNotify.Subscription.Settings;

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
        /// Provides subscription settings.
        /// </summary>
        private readonly ISubscriptionSettingsProvider _settingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionService"/> class.
        /// </summary>
        /// <param name="repository">Notifications subscribers repository.</param>
        /// <param name="settingsProvider">Provides subscription settings.</param>
        public SubscriptionService(ISubscribersRepository repository, ISubscriptionSettingsProvider settingsProvider)
        {
            _repository = repository;
            _settingsProvider = settingsProvider;
        }

        /// <inheritdoc />
        public Task AddSubscriptionAsync(long chatId, string subscriptionKey)
        {
            var needValidateSecretKey = CheckSubscriptionKeyNeeded();
            if (needValidateSecretKey)
                ValidateSubscriptionKey(subscriptionKey);

            return _repository.CreateSubscriberAsync(chatId);
        }

        /// <inheritdoc />
        public Task RemoveSubscriptionAsync(long chatId) =>
            _repository.DeleteSubscriberAsync(chatId);

        /// <inheritdoc />
        public Task<Subscriber[]> GetAllSubscriptionsAsync() =>
            _repository.GetAllSubscribersAsync();

        /// <inheritdoc/>
        public bool CheckSubscriptionKeyNeeded()
        {
            var subscriptionKey = _settingsProvider.GetSubscriptionKey();
            return !string.IsNullOrWhiteSpace(subscriptionKey);
        }

        /// <summary>
        /// Validates the provided subscription key.
        /// </summary>
        /// <param name="subscriptionKey">The provided subscription key.</param>
        private void ValidateSubscriptionKey(string subscriptionKey)
        {
            var validKey = _settingsProvider.GetSubscriptionKey();
            var secretKeyIsValid = validKey.Equals(subscriptionKey);

            if (!secretKeyIsValid)
                throw new SubscriptionNotAllowedException("Invalid subscription key");
        }
    }
}
