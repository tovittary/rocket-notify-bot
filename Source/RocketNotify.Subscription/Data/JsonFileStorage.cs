namespace RocketNotify.Subscription.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;

    using RocketNotify.Subscription.Model;

    /// <summary>
    /// Subscribers data storage, represented by a JSON file.
    /// </summary>
    public class JsonFileStorage : IFileStorage
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
        /// Subscribers storage file path.
        /// </summary>
        private string _storageFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileStorage"/> class.
        /// </summary>
        /// <param name="configuration">Application settings.</param>
        public JsonFileStorage(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        public void Initialize()
        {
            var storageFolder = _configuration.GetSection("Subscriptions")?["StorageFolder"];
            if (string.IsNullOrWhiteSpace(storageFolder))
                throw new InvalidOperationException("The storage folder path cannot be empty (Model:StorageFolder). Check the appsettings.json file.");

            if (!Directory.Exists(storageFolder))
                Directory.CreateDirectory(storageFolder);

            _storageFilePath = Path.Combine(storageFolder, StorageFileName);
        }

        /// <inheritdoc />
        public async Task<ICollection<Subscriber>> LoadSubscribersDataAsync()
        {
            if (!File.Exists(_storageFilePath))
                return Array.Empty<Subscriber>();

            var fileContents = await File.ReadAllTextAsync(_storageFilePath).ConfigureAwait(false);
            return JsonSerializer.Deserialize<Subscriber[]>(fileContents);
        }

        /// <inheritdoc />
        public void SaveSubscribersData(ICollection<Subscriber> subscribers)
        {
            var serialized = JsonSerializer.Serialize(subscribers, new JsonSerializerOptions { WriteIndented = true });

            lock (StorageFileName)
                File.WriteAllText(_storageFilePath, serialized, Encoding.UTF8);
        }
    }
}
