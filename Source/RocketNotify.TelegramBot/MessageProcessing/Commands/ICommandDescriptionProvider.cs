namespace RocketNotify.TelegramBot.MessageProcessing.Commands
{
    /// <summary>
    /// Provides description of supported command.
    /// </summary>
    public interface ICommandDescriptionProvider
    {
        /// <summary>
        /// Gets a description for a command.
        /// </summary>
        /// <returns>A command description.</returns>
        CommandDescription GetDescription();
    }
}
