namespace RocketNotify.TelegramBot.Messages.Filtration
{
    using System;

    /// <summary>
    /// Describes the result of filtering a message.
    /// </summary>
    public class FiltrationResult
    {
        /// <summary>
        /// Gets or sets the type of action to be taken on the message.
        /// </summary>
        public FiltrationAction SuggestedAction { get; set; }

        /// <summary>
        /// Gets or sets the type of the next message filter.
        /// </summary>
        public Type NextSuggestedFilterType { get; set; }

        /// <summary>
        /// Creates the filtration result with the <see cref="FiltrationAction.Process"/> action.
        /// </summary>
        /// <returns>The filtration result with the <see cref="FiltrationAction.Process"/> action.</returns>
        public static FiltrationResult Process()
        {
            return new FiltrationResult { SuggestedAction = FiltrationAction.Process };
        }

        /// <summary>
        /// Creates the filtration result with the <see cref="FiltrationAction.Ignore"/> action.
        /// </summary>
        /// <returns>The filtration result with the <see cref="FiltrationAction.Ignore"/> action.</returns>
        public static FiltrationResult Ignore()
        {
            return new FiltrationResult { SuggestedAction = FiltrationAction.Ignore };
        }

        /// <summary>
        /// Creates the filtration result with the <see cref="FiltrationAction.NextFilter"/> action.
        /// </summary>
        /// <param name="typeOfNextFilter">The type of the next filter.</param>
        /// <returns>The filtration result with the <see cref="FiltrationAction.NextFilter"/> action.</returns>
        public static FiltrationResult NextFilter(Type typeOfNextFilter)
        {
            return new FiltrationResult { SuggestedAction = FiltrationAction.NextFilter, NextSuggestedFilterType = typeOfNextFilter };
        }
    }
}
