namespace RocketNotify.TelegramBot.Messages
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using RocketNotify.TelegramBot.MessageProcessing;
    using RocketNotify.TelegramBot.MessageProcessing.Model;
    using RocketNotify.TelegramBot.Messages.Filtration.Factory;

    using Telegram.Bot.Types;

    /// <summary>
    /// Handles the event of a message received by the bot.
    /// </summary>
    public class BotMessageHandler : IBotMessageHandler
    {
        /// <summary>
        /// The factory used for obtaining the message filters.
        /// </summary>
        private readonly IMessageFilterFactory _messageFilterFactory;

        /// <summary>
        /// The invoker of message processing.
        /// </summary>
        private readonly IMessageProcessorInvoker _processor;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<BotMessageHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotMessageHandler"/> class.
        /// </summary>
        /// <param name="messageFilterFactory">The factory used for obtaining the message filters.</param>
        /// <param name="processor">The invoker of message processing.</param>
        /// <param name="logger">Logger.</param>
        public BotMessageHandler(IMessageFilterFactory messageFilterFactory, IMessageProcessorInvoker processor, ILogger<BotMessageHandler> logger)
        {
            _messageFilterFactory = messageFilterFactory;
            _processor = processor;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task HandleAsync(Message message)
        {
            LogMessage(message);

            var shouldHandleMessage = FilterMessage(message);
            if (!shouldHandleMessage)
                return Task.CompletedTask;

            var converted = MessageConverter.Convert(message);
            return _processor.InvokeAsync(converted);
        }

        /// <summary>
        /// Filters the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if the message should be handled, <c>false</c> otherwise.</returns>
        private bool FilterMessage(Message message)
        {
            var shouldHandleMessage = false;
            try
            {
                var messageFilter = _messageFilterFactory.GetFilter();
                shouldHandleMessage = messageFilter.Filter(message);
            }
            catch (Exception ex)
            {
                LogException(message, ex);
            }

            LogFiltrationResult(message, shouldHandleMessage);
            return shouldHandleMessage;
        }

        /// <summary>
        /// Logs the message received from the bot.
        /// </summary>
        /// <param name="message">The message instance.</param>
        private void LogMessage(Message message)
        {
            var msg = $"MessageId: {message.MessageId}; User: '{message.From.Username}' ({message.Chat.Title ?? message.Chat.FirstName}). Message: '{message.Text}'";
            _logger.LogInformation(msg);
        }

        /// <summary>
        /// Logs the message filtration error.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <param name="ex">The exception that describes the error that occurred during filtration.</param>
        private void LogException(Message message, Exception ex)
        {
            var msg = $"MessageId: {message.MessageId}; Error: {ex.Message}";
            _logger.LogError(msg);
        }

        /// <summary>
        /// Logs the message filtration result.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <param name="shouldHandleMessage">Specifies whether the message should be handled.</param>
        private void LogFiltrationResult(Message message, bool shouldHandleMessage)
        {
            var filtrationResult = shouldHandleMessage ? "Process" : "Ignore";

            var msg = $"MessageId: {message.MessageId}; Filtration result: {filtrationResult}";
            _logger.LogInformation(msg);
        }
    }
}
