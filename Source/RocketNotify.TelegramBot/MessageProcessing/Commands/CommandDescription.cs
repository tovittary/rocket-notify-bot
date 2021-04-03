namespace RocketNotify.TelegramBot.MessageProcessing.Commands
{
    /// <summary>
    /// Describes a command supported by the bot.
    /// </summary>
    public class CommandDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDescription"/> class.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="description">The description of the command.</param>
        public CommandDescription(string commandName, string description)
        {
            CommandName = commandName;
            Description = description;
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        public string Description { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{CommandName}: {Description}";
        }
    }
}
