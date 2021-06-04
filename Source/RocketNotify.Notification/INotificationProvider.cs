namespace RocketNotify.Notification
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RocketNotify.Notification.Model;

    /// <summary>
    /// Provides functionality for generating notification messages based on data received from Rocket.Chat.
    /// </summary>
    public interface INotificationProvider
    {
        /// <summary>
        /// Generates the notification text based on messages received from Rocket.Chat and subscribers' configuration.
        /// </summary>
        /// <param name="data">Aggregated data on messages received from Rocket.Chat.</param>
        /// <returns>Notification messages.</returns>
        Task<IEnumerable<Notification>> GenerateNotificationsAsync(MessagesData data);
    }
}
