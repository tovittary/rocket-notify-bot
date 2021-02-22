namespace RocketNotify.TelegramBot.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.Commands;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    /// <summary>
    /// Bot messages processing module.
    /// </summary>
    public class BotMessageProcessor : IBotMessageProcessor
    {
        /// <summary>
        /// Commands available for execution.
        /// </summary>
        private readonly ICommand[] _commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotMessageProcessor"/> class.
        /// </summary>
        /// <param name="commands">Commands available for execution.</param>
        public BotMessageProcessor(IEnumerable<ICommand> commands)
        {
            _commands = commands.ToArray();
        }

        /// <inheritdoc />
        public async Task ProcessMessageAsync(Message message, ITelegramMessageSender messageSender)
        {
            var chatId = message.Chat.Id;
            var commandName = ParseCommandName(message);

            var command = _commands.FirstOrDefault(c => c.Name == commandName);
            if (command == null)
                throw new NotSupportedException($"The command '{commandName}' is not supported at the moment.");

            var result = await command.ExecuteAsync(message).ConfigureAwait(false);
            await messageSender.SendMessageAsync(chatId, result.ResponseText, result.ReplyMarkup).ConfigureAwait(false);
        }

        /// <summary>
        /// Parses the command name from the message text. If no command is found, returns the name of the help command.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <returns>The command name.</returns>
        private CommandName ParseCommandName(Message message)
        {
            var commands = message.GetEntities().Where(ent => ent.Type == MessageEntityType.BotCommand);
            var firstCommand = commands.FirstOrDefault();
            if (firstCommand == default)
                throw new ArgumentException("Message processing error: could not find command name");

            var commandText = firstCommand.Value;
            if (commandText.Contains('@'))
                commandText = commandText.Split('@').First();

            return Enum.TryParse(commandText.Trim('/'), true, out CommandName commandName)
                ? commandName
                : CommandName.Help;
        }
    }
}
