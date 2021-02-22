namespace RocketNotify.TelegramBot.Commands
{
    using System.Threading.Tasks;

    using Telegram.Bot.Types;

    /// <summary>
    /// Start command.
    /// </summary>
    public class StartCommand : ICommand
    {
        /// <inheritdoc />
        public CommandName Name => CommandName.Start;

        /// <inheritdoc />
        public Task<CommandResult> ExecuteAsync(Message message)
        {
            var result = new CommandResult { ResponseText = $"Hello there, dear {message.From.FirstName}" };
            return Task.FromResult(result);
        }
    }
}
