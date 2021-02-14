namespace RocketNotify.TelegramBot.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.Client.Factory;
    using RocketNotify.TelegramBot.Messages;

    using Telegram.Bot;
    using Telegram.Bot.Args;

    /// <summary>
    /// Telegram bot client using a polling mechanism o receive messages.
    /// </summary>
    public class TelegramBotPollingClient : ITelegramBotPollingClient
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
        /// Messages polling process cancellation token.
        /// </summary>
        private CancellationToken _token;

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

        /// <summary>
        /// Finalizes an instance of the <see cref="TelegramBotPollingClient" /> class.
        /// </summary>
        ~TelegramBotPollingClient()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void Initialize()
        {
            if (_client != null)
                throw new InvalidOperationException("The client already initialized.");

            _client = _botClientFactory.GetClient();
        }

        /// <inheritdoc />
        public void StartPolling(CancellationToken token)
        {
            if (_client == null)
                throw new InvalidOperationException("The client is not yet initialized.");

            _token = token;
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

            _token = CancellationToken.None;
            _client.OnMessage -= ProcessMessageAsync;
        }

        /// <inheritdoc />
        public Task SendMessageAsync(long chatId, string text)
        {
            if (_client == null)
                throw new InvalidOperationException("The client is not yet initialized.");

            return _client.SendTextMessageAsync(chatId, text, cancellationToken: _token);
        }

        /// <summary>
        /// Releases unmanaged and managed resources.
        /// </summary>
        /// <param name="isDisposing">Specifies whether to release managed resources.</param>
        protected void Dispose(bool isDisposing)
        {
            if (!isDisposing)
                return;

            if (_client != null)
                StopPolling();
        }

        /// <summary>
        /// Handles the message received event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProcessMessageAsync(object sender, MessageEventArgs e) =>
            _messageHandler.HandleAsync(e.Message, this);
    }
}
