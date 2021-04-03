namespace RocketNotify.TelegramBot.MessageProcessing.Help
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.MessageProcessing.Commands;
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Processes the message that contains "Help" command.
    /// </summary>
    public class HelpCommandProcessor : IMessageProcessor
    {
        /// <summary>
        /// The text of the command.
        /// </summary>
        private const string CommandText = "/help";

        /// <summary>
        /// Available commands descriptions.
        /// </summary>
        private readonly ICommandInfoAggregator _commandsDescriptions;

        /// <summary>
        /// The client used to send responses to messages.
        /// </summary>
        private readonly ITelegramMessageSender _responder;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommandProcessor"/> class.
        /// </summary>
        /// <param name="commandsDescriptions">Available commands descriptions.</param>
        /// <param name="responder">The client used to send responses to messages.</param>
        public HelpCommandProcessor(ICommandInfoAggregator commandsDescriptions, ITelegramMessageSender responder)
        {
            _commandsDescriptions = commandsDescriptions;
            _responder = responder;
        }

        /// <inheritdoc/>
        public bool IsRelevant(BotMessage message) => message.Text.Contains(CommandText, StringComparison.InvariantCultureIgnoreCase);

        /// <inheritdoc/>
        public async Task<ProcessResult> ProcessAsync(BotMessage message)
        {
            var senderId = message.Sender.Id;

            var allCommandsDescriptions = _commandsDescriptions.GetDescriptions();
            var responseText = string.Join(Environment.NewLine, allCommandsDescriptions.Select(cd => cd.ToString()));

            await _responder.SendMessageAsync(senderId, responseText).ConfigureAwait(false);

            return ProcessResult.Final();
        }
    }
}
