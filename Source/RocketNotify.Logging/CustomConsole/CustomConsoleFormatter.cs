namespace RocketNotify.Logging.CustomConsole
{
    using System;
    using System.IO;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Logging.Console;

    /// <summary>
    /// A custom console log message formatter.
    /// </summary>
    public class CustomConsoleFormatter : ConsoleFormatter
    {
        /// <summary>
        /// The name of the formatter.
        /// </summary>
        public const string FormatterName = "CustomConsole";

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomConsoleFormatter"/> class.
        /// </summary>
        public CustomConsoleFormatter()
            : base(FormatterName)
        {
        }

        /// <inheritdoc/>
        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            message = $"{DateTime.Now:s} [{logEntry.LogLevel}] {message}";

            var foregroundColor = GetForegroundColor(logEntry.LogLevel);

            WriteWithColor(message, foregroundColor, textWriter);
        }

        /// <summary>
        /// Defines the foreground console color for the log level.
        /// </summary>
        /// <param name="logLevel">The message log level.</param>
        /// <returns>The console foreground color.</returns>
        private ConsoleColor GetForegroundColor(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => ConsoleColor.Gray,
                LogLevel.Debug => ConsoleColor.Blue,
                LogLevel.Information => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.DarkRed,
                LogLevel.Critical => ConsoleColor.Red,
                LogLevel.None => ConsoleColor.White,

                _ => ConsoleColor.White
            };
        }

        /// <summary>
        /// Writes the colored message.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="foregroundColor">The message foreground color.</param>
        /// <param name="writer">Text writer.</param>
        private void WriteWithColor(string message, ConsoleColor foregroundColor, TextWriter writer)
        {
            var escapedColor = AnsiEscapedColors.GetForegroundColorEscapeCode(foregroundColor);
            var defaultColor = AnsiEscapedColors.DefaultForegroundColor;

            writer.Write(escapedColor);
            writer.WriteLine(message);
            writer.Write(defaultColor);
        }
    }
}
