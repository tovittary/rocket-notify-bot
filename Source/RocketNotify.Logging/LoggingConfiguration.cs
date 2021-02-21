namespace RocketNotify.Logging
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using RocketNotify.Logging.CustomConsole;

    /// <summary>
    /// Sets up logging for the application.
    /// </summary>
    public class LoggingConfiguration : ILoggingConfiguration
    {
        /// <inheritdoc/>
        public void Configure(HostBuilderContext hostContext, ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleFormatterOptions>();
            logging.AddConsole(opt => opt.FormatterName = CustomConsoleFormatter.FormatterName);

            var serilogConfiguration = hostContext.Configuration.GetSection("Logging").GetSection("Serilog");
            if (serilogConfiguration != null)
                logging.AddFile(serilogConfiguration);
        }
    }
}
