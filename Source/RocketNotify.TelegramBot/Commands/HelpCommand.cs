namespace RocketNotify.TelegramBot.Commands
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Telegram.Bot.Types;

    /// <summary>
    /// Help command. Displays available commands list.
    /// </summary>
    public class HelpCommand : ICommand
    {
        /// <inheritdoc />
        public CommandName Name => CommandName.Help;

        /// <inheritdoc />
        public Task<CommandResult> ExecuteAsync(Message message)
        {
            var commandNames = Enum.GetValues(typeof(CommandName)).Cast<CommandName>();
            var commandNamesString = string.Join(", ", commandNames.Select(c => $"/{c.ToString().ToLower()}"));

            var result = new CommandResult { ReplyText = $"Available commands: {commandNamesString}" };
            return Task.FromResult(result);
        }
    }
}
