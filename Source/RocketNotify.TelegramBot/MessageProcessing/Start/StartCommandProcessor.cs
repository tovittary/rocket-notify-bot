namespace RocketNotify.TelegramBot.MessageProcessing.Start
{
    using System;
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.MessageProcessing.Commands;
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Processes the message that contains the "Start" command.
    /// </summary>
    public class StartCommandProcessor : IMessageProcessor, ICommandDescriptionProvider
    {
        /// <summary>
        /// The text of the command.
        /// </summary>
        private const string CommandText = "/start";

        /// <summary>
        /// The client used to send responses to messages.
        /// </summary>
        private readonly ITelegramMessageSender _responder;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartCommandProcessor"/> class.
        /// </summary>
        /// <param name="responder">The client used to send responses to messages.</param>
        public StartCommandProcessor(ITelegramMessageSender responder)
        {
            _responder = responder;
        }

        /// <inheritdoc/>
        public bool IsRelevant(BotMessage message) => message.Text.Contains(CommandText, StringComparison.InvariantCultureIgnoreCase);

        /// <inheritdoc/>
        public async Task<ProcessResult> ProcessAsync(BotMessage message)
        {
            var senderId = message.Sender.Id;
            var responseText = $"Hello there, friend! Now I know that your name is {message.Sender.Name}! I know everything about you...";

            await _responder.SendMessageAsync(senderId, responseText).ConfigureAwait(false);

            return ProcessResult.Final();
        }

        /// <inheritdoc/>
        public CommandDescription GetDescription() =>
            new CommandDescription(CommandText, "Get started with the bot");
    }
}
