namespace RocketNotify.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Provides operations for registering services in <see cref="IServiceCollection"/>.
    /// </summary>
    public interface IServiceRegistration
    {
        /// <summary>
        /// Adds services to <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        void Register(IServiceCollection services);
    }
}
