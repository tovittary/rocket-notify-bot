namespace RocketNotify.TelegramBot.MessageProcessing.Unsupported
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Processes the message containing an unsupported command name.
    /// </summary>
    public class UnsupportedCommandProcessor : IUnsupportedCommandProcessor
    {
        /// <summary>
        /// The client used to send responses to messages.
        /// </summary>
        private readonly ITelegramMessageSender _responder;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedCommandProcessor"/> class.
        /// </summary>
        /// <param name="responder">The client used to send responses to messages.</param>
        public UnsupportedCommandProcessor(ITelegramMessageSender responder)
        {
            _responder = responder;
        }

        /// <inheritdoc/>
        public bool IsRelevant(BotMessage message) =>
            message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Any(w => w.StartsWith('/'));

        /// <inheritdoc/>
        public async Task<ProcessResult> ProcessAsync(BotMessage message)
        {
            await _responder.SendMessageAsync(message.Sender.Id, "Command is not supported.");
            return ProcessResult.Final();
        }
    }
}
