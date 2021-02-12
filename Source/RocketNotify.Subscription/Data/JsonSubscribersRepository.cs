namespace RocketNotify.Subscription.Data
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;

    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Model;

    /// <summary>
    /// Notifications subscribers repository that uses JSON file as storage.
    /// </summary>
    public class JsonSubscribersRepository : ISubscribersRepository
    {
        /// <summary>
        /// The name of the subscribers storage file.
        /// </summary>
        private const string StorageFileName = "subscribers.rnb";

        /// <summary>
        /// Application settings.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// A value indicating whether the repository has been initialized.
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// Subscribers.
        /// </summary>
        private ConcurrentDictionary<long, Subscriber> _subscribers;

        /// <summary>
        /// Subscribers storage file path;
        /// </summary>
        private string _storageFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSubscribersRepository" /> class.
        /// </summary>
        /// <param name="configuration">Application settings.</param>
        public JsonSubscribersRepository(IConfiguration configuration)
        {
            _configuration = configuration;
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

            SaveSubsToFile();
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

            SaveSubsToFile();
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

            var storageFolder = _configuration.GetSection("Model")?["StorageFolder"];
            if (string.IsNullOrWhiteSpace(storageFolder))
                throw new InvalidOperationException("The storage folder path cannot be empty (Model:StorageFolder). Check the appsettings.json file.");

            _storageFilePath = Path.Combine(storageFolder, StorageFileName);
            if (!File.Exists(_storageFilePath))
            {
                _subscribers = new ConcurrentDictionary<long, Subscriber>();
                _initialized = true;

                return;
            }

            var fileContents = await File.ReadAllTextAsync(_storageFilePath).ConfigureAwait(false);
            var subs = JsonSerializer.Deserialize<Subscriber[]>(fileContents).ToDictionary(sub => sub.ChatId);

            _subscribers = new ConcurrentDictionary<long, Subscriber>(subs);
            _initialized = true;
        }

        /// <summary>
        /// Saves subscribers data to the storage file.
        /// </summary>
        private void SaveSubsToFile()
        {
            var serialized = JsonSerializer.Serialize(_subscribers.Values, new JsonSerializerOptions { WriteIndented = true });

            lock (_subscribers)
                File.WriteAllText(_storageFilePath, serialized, Encoding.UTF8);
        }
    }
}
