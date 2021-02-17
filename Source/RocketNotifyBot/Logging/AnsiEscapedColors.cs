namespace RocketNotifyBot.Logging
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Matching of console colors to ansi escape codes.
    /// </summary>
    public static class AnsiEscapedColors
    {
        /// <summary>
        /// The ansi escape code that matches the default console foreground color.
        /// </summary>
        public const string DefaultForegroundColor = "\x1B[39m\x1B[22m";

        /// <summary>
        /// Matching of ansi escape codes to console foreground colors.
        /// </summary>
        private static readonly Dictionary<ConsoleColor, string> ForegroundColors =
            new Dictionary<ConsoleColor, string>
            {
                { ConsoleColor.Black, "\x1B[30m" },
                { ConsoleColor.DarkRed, "\x1B[31m" },
                { ConsoleColor.DarkGreen, "\x1B[32m" },
                { ConsoleColor.DarkYellow, "\x1B[33m" },
                { ConsoleColor.DarkBlue, "\x1B[34m" },
                { ConsoleColor.DarkMagenta, "\x1B[35m" },
                { ConsoleColor.DarkCyan, "\x1B[36m" },
                { ConsoleColor.Gray, "\x1B[37m" },
                { ConsoleColor.Red, "\x1B[1m\x1B[31m" },
                { ConsoleColor.Green, "\x1B[1m\x1B[32m" },
                { ConsoleColor.Yellow, "\x1B[1m\x1B[33m" },
                { ConsoleColor.Blue, "\x1B[1m\x1B[34m" },
                { ConsoleColor.Magenta, "\x1B[1m\x1B[35m" },
                { ConsoleColor.Cyan, "\x1B[1m\x1B[36m" },
                { ConsoleColor.White, "\x1B[1m\x1B[37m" }
            };

        /// <summary>
        /// Gets the ansi escape code that matches the console foreground color.
        /// </summary>
        /// <param name="foregroundColor">The console foreground color.</param>
        /// <returns>The ansi escape code.</returns>
        public static string GetForegroundColorEscapeCode(ConsoleColor foregroundColor) =>
            ForegroundColors.GetValueOrDefault(foregroundColor) ?? DefaultForegroundColor;
    }
}
