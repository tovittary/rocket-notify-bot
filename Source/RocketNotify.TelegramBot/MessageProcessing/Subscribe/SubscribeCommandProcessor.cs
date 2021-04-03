namespace RocketNotify.TelegramBot.MessageProcessing.Subscribe
{
    using System;
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Processes a sequence of messages related to the notifications subscription process.
    /// </summary>
    public class SubscribeCommandProcessor : IStatefulMessageProcessor
    {
        /// <summary>
        /// The client used to send responses to messages.
        /// </summary>
        private readonly ITelegramMessageSender _responder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeCommandProcessor"/> class.
        /// </summary>
        /// <param name="responder">The client used to send responses to messages.</param>
        /// <param name="getInitialState">The delegate for acquiring initial subscription command processing state.</param>
        public SubscribeCommandProcessor(ITelegramMessageSender responder, Func<InitialSubscribeState> getInitialState)
        {
            _responder = responder;
            ChangeCurrentState(getInitialState());
        }

        /// <inheritdoc/>
        public MessageContext Context { get; } = new MessageContext();

        /// <inheritdoc/>
        public IMessageProcessingState CurrentState { get; private set; }

        /// <inheritdoc/>
        public void ChangeCurrentState(IConfigurableMessageProcessingState state)
        {
            state.SetProcessor(this);
            CurrentState = state;
        }

        /// <inheritdoc/>
        public bool IsRelevant(BotMessage message) => CurrentState.IsRelevant(message);

        /// <inheritdoc/>
        public async Task<ProcessResult> ProcessAsync(BotMessage message)
        {
            var response = await CurrentState.ProcessAsync(message).ConfigureAwait(false);
            response.Sender = message.Sender;

            if (response.Markup != null)
                await SendResponseWithMarkup(response).ConfigureAwait(false);
            else
                await SendResponse(response).ConfigureAwait(false);

            SaveContext(message, response);
            return new ProcessResult { IsFinal = CurrentState.IsFinal };
        }

        /// <summary>
        /// Sends the response message.
        /// </summary>
        /// <param name="response">The response to send.</param>
        /// <returns>The task that represents the process of sending the response.</returns>
        private async Task SendResponse(BotMessage response)
        {
            var senderId = response.Sender.Id;
            response.MessageId = await _responder.SendMessageAsync(senderId, response.Text).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends the response message with a special markup.
        /// </summary>
        /// <param name="response">The response to send.</param>
        /// <returns>The task that represents the process of sending the response.</returns>
        private async Task SendResponseWithMarkup(BotMessage response)
        {
            var senderId = response.Sender.Id;
            var responseMarkup = MessageConverter.ConvertMarkup(response.Markup);
            response.MessageId = await _responder.SendMessageAsync(senderId, response.Text, responseMarkup).ConfigureAwait(false);
        }

        /// <summary>
        /// Saves the current state of the message processing context.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="response">The generated response message.</param>
        private void SaveContext(BotMessage message, BotMessage response)
        {
            Context.LastMessage = message;
            Context.LastResponse = response;
        }
    }
}
