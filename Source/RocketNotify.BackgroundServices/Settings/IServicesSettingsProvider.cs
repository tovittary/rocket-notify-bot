namespace RocketNotify.BackgroundServices.Settings
{
    using System;

    /// <summary>
    /// Background services settings provider interface.
    /// </summary>
    public interface IServicesSettingsProvider
    {
        /// <summary>
        /// Gets the messages check interval.
        /// </summary>
        /// <returns>The interval for checking messages.</returns>
        TimeSpan GetMessageCheckInterval();
    }
}
