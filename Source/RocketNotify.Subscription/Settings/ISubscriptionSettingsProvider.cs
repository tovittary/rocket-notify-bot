namespace RocketNotify.Subscription.Settings
{
    /// <summary>
    /// Provides subscription settings.
    /// </summary>
    public interface ISubscriptionSettingsProvider
    {
        /// <summary>
        /// Gets the secret key used to subscribe.
        /// </summary>
        /// <returns>The subscription secret key.</returns>
        string GetSubscriptionKey();
    }
}
