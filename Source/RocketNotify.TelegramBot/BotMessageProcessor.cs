namespace RocketNotify.TelegramBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using RocketNotify.TelegramBot.Commands;
    using RocketNotify.TelegramBot.Interfaces;

    using Telegram.Bot.Types;

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
        /// Logger.
        /// </summary>
        private readonly ILogger<BotMessageProcessor> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotMessageProcessor"/> class.
        /// </summary>
        /// <param name="commands">Commands available for execution.</param>
        /// <param name="logger">Logger.</param>
        public BotMessageProcessor(IEnumerable<ICommand> commands, ILogger<BotMessageProcessor> logger)
        {
            _commands = commands.ToArray();
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task ProcessMessageAsync(Message message, ITelegramBotMessageSender client)
        {
            LogMessage(message);

            var chatId = message.Chat.Id;
            var commandName = ParseCommandName(message.Text);

            var command = _commands.FirstOrDefault(c => c.Name == commandName);
            if (command == null)
                throw new NotSupportedException($"The command '{commandName}' is not supported at the moment.");

            var result = await command.ExecuteAsync(message).ConfigureAwait(false);
            await client.SendMessageAsync(chatId, result.Text).ConfigureAwait(false);
        }

        /// <summary>
        /// Parses the command name from the message text. If no command is found, returns the name of the help command.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <returns>The command name.</returns>
        private CommandName ParseCommandName(string messageText)
        {
            var firstWord = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (string.IsNullOrEmpty(firstWord))
                return CommandName.Help;

            return Enum.TryParse(firstWord.Trim('/'), true, out CommandName commandName)
                ? commandName
                : CommandName.Help;
        }

        /// <summary>
        /// Logs the message received from the bot.
        /// </summary>
        /// <param name="message">The message instance.</param>
        private void LogMessage(Message message)
        {
            var msg = $"[{DateTime.Now}] User: '{message.From.Username}' ({message.Chat.Id}). Message: '{message.Text}'";
            _logger.LogInformation(msg);
        }
    }
}
