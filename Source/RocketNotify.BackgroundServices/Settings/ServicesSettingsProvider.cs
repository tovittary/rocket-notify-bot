namespace RocketNotify.BackgroundServices.Settings
{
    using System;

    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Provides background services settings.
    /// </summary>
    public class ServicesSettingsProvider : IServicesSettingsProvider
    {
        /// <summary>
        /// Default new messages check interval.
        /// </summary>
        private static readonly TimeSpan _defaultMessageCheckInterval = TimeSpan.FromSeconds(6);

        /// <summary>
        /// Application settings.
        /// </summary>
        private IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesSettingsProvider"/> class.
        /// </summary>
        /// <param name="configuration">Application settings.</param>
        public ServicesSettingsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public TimeSpan GetMessageCheckInterval()
        {
            var intervalSecondsStr = _configuration.GetSection("Notifications")?["MessageCheckIntervalSec"];
            if (string.IsNullOrWhiteSpace(intervalSecondsStr))
                return _defaultMessageCheckInterval;

            if (int.TryParse(intervalSecondsStr, out var seconds))
                return TimeSpan.FromSeconds(seconds);

            return _defaultMessageCheckInterval;
        }
    }
}
