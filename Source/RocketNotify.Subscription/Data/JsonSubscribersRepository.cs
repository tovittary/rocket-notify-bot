namespace RocketNotify.Subscription.Data
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Model;

    /// <summary>
    /// Notifications subscribers repository that uses JSON file as storage.
    /// </summary>
    public class JsonSubscribersRepository : ISubscribersRepository
    {
        /// <summary>
        /// Subscribers data storage.
        /// </summary>
        private readonly IFileStorage _storage;

        /// <summary>
        /// A value indicating whether the repository has been initialized.
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// Subscribers.
        /// </summary>
        private ConcurrentDictionary<long, Subscriber> _subscribers;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSubscribersRepository" /> class.
        /// </summary>
        /// <param name="storage">Subscribers data storage.</param>
        public JsonSubscribersRepository(IFileStorage storage)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task CreateSubscriberAsync(long chatId)
        {
            await InitializeAsync().ConfigureAwait(false);

            if (_subscribers.ContainsKey(chatId))
                throw new SubscriberAlreadyExistsException($"Subscriber with the same chatId already exists ({chatId})");

            var newSub = new Subscriber { ChatId = chatId };
            var subscriberAdded = _subscribers.TryAdd(chatId, newSub);

            if (!subscriberAdded)
                throw new SubscriberOperationException($"There was an error while attempting to create subscriber (chatId = {chatId})");

            _storage.SaveSubscribersData(_subscribers.Values);
        }

        /// <inheritdoc />
        public async Task DeleteSubscriberAsync(long chatId)
        {
            await InitializeAsync().ConfigureAwait(false);

            if (!_subscribers.ContainsKey(chatId))
                throw new SubscriberNotFoundException($"Subscriber with chatId = {chatId} not found.");

            var subscriberRemoved = _subscribers.TryRemove(chatId, out var subscriber);
            if (!subscriberRemoved)
                throw new SubscriberOperationException($"There was an error while attempting to delete subscriber with chatId = {chatId}");

            _storage.SaveSubscribersData(_subscribers.Values);
        }

        /// <inheritdoc />
        public async Task<Subscriber[]> GetAllSubscribersAsync()
        {
            await InitializeAsync().ConfigureAwait(false);
            return _subscribers.Values.ToArray();
        }

        /// <summary>
        /// Initializes repository with subscribers data.
        /// </summary>
        /// <returns>A task that represents the repository initialization process.</returns>
        private async Task InitializeAsync()
        {
            if (_initialized)
                return;

            _storage.Initialize();

            var subscribers = await _storage.LoadSubscribersDataAsync().ConfigureAwait(false);
            var subscribesrDictionary = subscribers.ToDictionary(s => s.ChatId);

            _subscribers = new ConcurrentDictionary<long, Subscriber>();
            _initialized = true;
        }
    }
}
