namespace RocketNotify.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    using RocketNotify.BackgroundServices;
    using RocketNotify.BackgroundServices.Settings;

    /// <summary>
    /// Registers background services.
    /// </summary>
    public class BackgroundServiceRegistration : IServiceRegistration
    {
        /// <inheritdoc />
        public void Register(IServiceCollection services)
        {
            services.AddTransient<IServicesSettingsProvider, ServicesSettingsProvider>();
            services.AddHostedService<TelegramBotBackgroundService>();
            services.AddHostedService<NotifierBackgroundService>();
        }
    }
}
