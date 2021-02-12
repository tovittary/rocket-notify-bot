namespace RocketNotify.TelegramBot.Commands
{
    using System.Threading.Tasks;

    using Telegram.Bot.Types;

    /// <summary>
    /// Bot command interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        CommandName Name { get; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="message">The message from the bot.</param>
        /// <returns>Command execution result.</returns>
        Task<CommandResult> ExecuteAsync(Message message);
    }
}
