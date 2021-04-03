namespace RocketNotify.TelegramBot.MessageProcessing.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc/>
    public class CommandInfoAggregator : ICommandInfoAggregator
    {
        /// <summary>
        /// Provides description of supported commands.
        /// </summary>
        private readonly IEnumerable<ICommandDescriptionProvider> _descriptionProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInfoAggregator"/> class.
        /// </summary>
        /// <param name="descriptionProviders">Provides description of supported commands.</param>
        public CommandInfoAggregator(IEnumerable<ICommandDescriptionProvider> descriptionProviders)
        {
            _descriptionProviders = descriptionProviders;
        }

        /// <inheritdoc/>
        public CommandDescription[] GetDescriptions()
        {
            return _descriptionProviders.Select(dp => dp.GetDescription()).ToArray();
        }
    }
}
