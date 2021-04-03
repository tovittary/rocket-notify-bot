namespace RocketNotify.TelegramBot.MessageProcessing.Commands
{
    /// <summary>
    /// Aggregates information about commands supported by the bot.
    /// </summary>
    public interface ICommandInfoAggregator
    {
        /// <summary>
        /// Returns description of all commands supported by the bot.
        /// </summary>
        /// <returns>Description of supported commands.</returns>
        CommandDescription[] GetDescriptions();
    }
}
