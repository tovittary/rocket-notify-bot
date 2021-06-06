namespace RocketNotify.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    using RocketNotify.Notification;

    /// <summary>
    /// Registers notification services.
    /// </summary>
    public class NotificationServiceRegistration : IServiceRegistration
    {
        /// <inheritdoc />
        public void Register(IServiceCollection services)
        {
            services.AddTransient<INotificationProvider, NotificationProvider>();
        }
    }
}
