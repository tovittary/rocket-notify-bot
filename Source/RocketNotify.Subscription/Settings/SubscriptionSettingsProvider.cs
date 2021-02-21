namespace RocketNotify.Subscription.Settings
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Provides subscription settings.
    /// </summary>
    public class SubscriptionSettingsProvider : ISubscriptionSettingsProvider
    {
        /// <summary>
        /// Application settings.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionSettingsProvider"/> class.
        /// </summary>
        /// <param name="configuration">Application settings.</param>
        public SubscriptionSettingsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public string GetSubscriptionKey() =>
            _configuration.GetSection("Subscriptions")?["Secret"] ?? string.Empty;
    }
}
