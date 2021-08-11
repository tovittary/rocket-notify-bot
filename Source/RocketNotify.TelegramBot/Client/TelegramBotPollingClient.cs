namespace RocketNotify.TelegramBot.Client
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.Client.Factory;
    using RocketNotify.TelegramBot.Exceptions;
    using RocketNotify.TelegramBot.Messages;

    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Exceptions;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Telegram bot client using a polling mechanism to receive messages.
    /// </summary>
    public class TelegramBotPollingClient : ITelegramMessagePollingClient
    {
        /// <summary>
        /// Handles the event of a message received by the bot.
        /// </summary>
        private readonly IBotMessageHandler _messageHandler;

        /// <summary>
        /// Factory for getting the <see cref="ITelegramBotClient"/> interface implementations.
        /// </summary>
        private readonly ITelegramBotClientFactory _botClientFactory;

        /// <summary>
        /// The underlying telegram bot client.
        /// </summary>
        private ITelegramBotClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotPollingClient" /> class.
        /// </summary>
        /// <param name="messageHandler">Handles the event of a message received by the bot.</param>
        /// <param name="botClientFactory">Factory for getting the <see cref="ITelegramBotClient"/> interface implementations.</param>
        public TelegramBotPollingClient(IBotMessageHandler messageHandler, ITelegramBotClientFactory botClientFactory)
        {
            _messageHandler = messageHandler;
            _botClientFactory = botClientFactory;
        }

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            lock (_botClientFactory)
            {
                if (_client != null)
                    return;

                _client = _botClientFactory.GetClient();
            }

            var tokenValid = await _client.TestApiAsync().ConfigureAwait(false);
            if (!tokenValid)
                throw new AuthTokenInvalidException("Invalid Telegram client auth token");
        }

        /// <inheritdoc />
        public void StartPolling(CancellationToken token)
        {
            if (_client == null)
                throw new InvalidOperationException("The client is not yet initialized.");

            _client.OnMessage += ProcessMessageAsync;
            _client.StartReceiving(cancellationToken: token);
        }

        /// <inheritdoc />
        public void StopPolling()
        {
            if (_client == null)
                throw new InvalidOperationException("The client is not yet initialized.");

            if (_client.IsReceiving)
                _client.StopReceiving();

            _client.OnMessage -= ProcessMessageAsync;
        }

        /// <inheritdoc />
        public Task<int> SendMessageAsync(long chatId, string text) =>
            SendMessageAsync(chatId, text, null);

        /// <inheritdoc/>
        public async Task<int> SendMessageAsync(long chatId, string text, IReplyMarkup markup)
        {
            if (_client == null)
                throw new InvalidOperationException("The client is not yet initialized.");

            var sentMessage = await SendTextMessageAsync(chatId, text, markup).ConfigureAwait(false);
            return sentMessage.MessageId;
        }

        /// <summary>
        /// Handles the message received event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProcessMessageAsync(object sender, MessageEventArgs e) =>
            _messageHandler.HandleAsync(e.Message);

        /// <summary>
        /// Sends text messages. On success, the sent Description is returned.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <param name="text">The message text.</param>
        /// <param name="markup">Markup of the message.</param>
        /// <returns>The description of the sent message.</returns>
        /// <remarks>Catches <see cref="HttpRequestException"/> instances, throws <see cref="ApiRequestException"/> instead.</remarks>
        private async Task<Message> SendTextMessageAsync(long chatId, string text, IReplyMarkup markup)
        {
            try
            {
                return await _client.SendTextMessageAsync(chatId, text, replyMarkup: markup).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                throw new ApiRequestException(ex.Message, ex);
            }
        }
    }
}
