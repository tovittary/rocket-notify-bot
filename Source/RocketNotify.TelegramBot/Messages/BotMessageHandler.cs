namespace RocketNotify.TelegramBot.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.Messages.Filtration;

    using Telegram.Bot.Types;

    /// <summary>
    /// Handles the event of a message received by the bot.
    /// </summary>
    public class BotMessageHandler : IBotMessageHandler
    {
        /// <summary>
        /// A collection of message filtration units.
        /// </summary>
        private readonly Dictionary<Type, IMessageFilter> _filters;

        /// <summary>
        /// The message filter that starts the filtration process.
        /// </summary>
        private readonly IInitialMessageFilter _initialFilter;

        /// <summary>
        /// Messages processing module.
        /// </summary>
        private readonly IBotMessageProcessor _processor;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<BotMessageHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotMessageHandler"/> class.
        /// </summary>
        /// <param name="initialFilter">The message filter that starts the filtration process.</param>
        /// <param name="filters">A collection of message filtration units.</param>
        /// <param name="processor">Messages processing module.</param>
        /// <param name="logger">Logger.</param>
        public BotMessageHandler(IInitialMessageFilter initialFilter, IEnumerable<IMessageFilter> filters, IBotMessageProcessor processor, ILogger<BotMessageHandler> logger)
        {
            _filters = filters.ToDictionary(f => f.GetType());
            _initialFilter = initialFilter;
            _processor = processor;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task HandleAsync(Message message, ITelegramMessageSender messageSender)
        {
            LogMessage(message);

            FiltrationAction filterResultAction;
            try
            {
                filterResultAction = FilterMessage(message);
            }
            catch (Exception ex)
            {
                LogException(message, ex);
                return Task.CompletedTask;
            }

            LogFiltrationResult(message, filterResultAction);
            if (filterResultAction == FiltrationAction.Process)
                return _processor.ProcessMessageAsync(message, messageSender);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Filters the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The message filtration result.</returns>
        private FiltrationAction FilterMessage(Message message)
        {
            IMessageFilter filter = _initialFilter;
            while (true)
            {
                var filtrationResult = filter.Filter(message);
                if (filtrationResult.SuggestedAction != FiltrationAction.NextFilter)
                    return filtrationResult.SuggestedAction;

                var filterType = filtrationResult.NextSuggestedFilterType;
                if (filterType == null)
                    throw new InvalidOperationException("Invalid filtration result: next filter type is not set");

                if (!_filters.ContainsKey(filterType))
                    throw new InvalidOperationException("Invalid filtration result: next filter type is not found");

                filter = _filters[filterType];
            }
        }

        /// <summary>
        /// Logs the message received from the bot.
        /// </summary>
        /// <param name="message">The message instance.</param>
        private void LogMessage(Message message)
        {
            var msg = $"[{DateTime.Now}] MessageId: {message.MessageId} User: '{message.From.Username}' ({message.Chat.Title ?? message.Chat.FirstName}). Message: '{message.Text}'";
            _logger.LogInformation(msg);
        }

        /// <summary>
        /// Logs the message filtration error.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <param name="ex">The exception that describes the error that occurred during filtration.</param>
        private void LogException(Message message, Exception ex)
        {
            var msg = $"[{DateTime.Now}] MessageId: {message.MessageId} Error: {ex.Message}";
            _logger.LogError(msg);
        }

        /// <summary>
        /// Logs the message filtration result.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <param name="filtrationResult">The message filtration result.</param>
        private void LogFiltrationResult(Message message, FiltrationAction filtrationResult)
        {
            var msg = $"[{DateTime.Now}] MessageId: {message.MessageId} Result: {filtrationResult}";
            _logger.LogInformation(msg);
        }
    }
}
