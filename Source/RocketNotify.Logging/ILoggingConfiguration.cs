namespace RocketNotify.Logging
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Provides operations for setting up logging for an application.
    /// </summary>
    public interface ILoggingConfiguration
    {
        /// <summary>
        /// Configures logging for an application.
        /// </summary>
        /// <param name="hostContext">Context containing the common services on the <see cref="IHost"/>.</param>
        /// <param name="logging">An interface for configuring logging providers.</param>
        void Configure(HostBuilderContext hostContext, ILoggingBuilder logging);
    }
}
