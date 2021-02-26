namespace RocketNotify.TelegramBot.MessageProcessing.Subscribe
{
    using System;
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Services;
    using RocketNotify.TelegramBot.MessageProcessing.Model;
    using RocketNotify.TelegramBot.MessageProcessing.Model.Markups;

    /// <summary>
    /// The state in which the subscription availability is checked.
    /// If it is available, then the message sender will be subscribed.
    /// If not - then the response message will be sent with a request for a subscription key.
    /// </summary>
    internal class InitialSubscribeState : IConfigurableMessageProcessingState
    {
        /// <summary>
        /// The text of the command.
        /// </summary>
        private const string CommandText = "/subscribe";

        /// <summary>
        /// Notifications subscriptions management service.
        /// </summary>
        private readonly ISubscriptionService _subscriptionService;

        /// <summary>
        /// A delegate used for obtaining an instance of the <see cref="VerifySubscriptionState"/> class.
        /// </summary>
        private readonly Func<VerifySubscriptionState> _getVerifySubscriptionState;

        /// <summary>
        /// The message processor.
        /// </summary>
        private IStatefulMessageProcessor _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitialSubscribeState"/> class.
        /// </summary>
        /// <param name="subscriptionService">Notifications subscriptions management service.</param>
        /// <param name="getVerifySubscriptionState">A delegate used for obtaining an instance of the <see cref="VerifySubscriptionState"/> class.</param>
        public InitialSubscribeState(ISubscriptionService subscriptionService, Func<VerifySubscriptionState> getVerifySubscriptionState)
        {
            _subscriptionService = subscriptionService;
            _getVerifySubscriptionState = getVerifySubscriptionState;
        }

        /// <inheritdoc/>
        public bool IsFinal => false;

        /// <inheritdoc/>
        public bool IsRelevant(Message message) => message.Text.Contains(CommandText, StringComparison.InvariantCultureIgnoreCase);

        /// <inheritdoc/>
        public async Task<Message> ProcessAsync(Message message)
        {
            try
            {
                return await DoProcessAsync(message).ConfigureAwait(false);
            }
            catch (SubscriberAlreadyExistsException)
            {
                return new Message { Text = "This chat already subscribed." };
            }
            catch (SubscriberOperationException)
            {
                return new Message { Text = "Failed to subscribe. Try again later." };
            }
        }

        /// <inheritdoc/>
        public void SetProcessor(IStatefulMessageProcessor processor) => _processor = processor;

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>The response to the message.</returns>
        private Task<Message> DoProcessAsync(Message message)
        {
            var secretIsNeeded = _subscriptionService.CheckSubscriptionKeyNeeded();
            return secretIsNeeded ? RequestSubscriptionKeyAsync() : SubscribeSenderAsync(message);
        }

        /// <summary>
        /// Generates a response message requesting a subscription key.
        /// </summary>
        /// <returns>The response to the message.</returns>
        private Task<Message> RequestSubscriptionKeyAsync()
        {
            var replyMarkup = new ForceReplyMarkup();
            var result = new Message { Text = "Provide the subscription key in the reply message to complete the process.", Markup = replyMarkup };

            UpdateProcessorState();

            return Task.FromResult(result);
        }

        /// <summary>
        /// Updates the current message processor state.
        /// </summary>
        private void UpdateProcessorState()
        {
            var newState = _getVerifySubscriptionState();
            _processor.ChangeCurrentState(newState);
        }

        /// <summary>
        /// Adds the message sender as a subscriber.
        /// </summary>
        /// <param name="message">The message being processed.</param>
        /// <returns>The response to the message.</returns>
        private async Task<Message> SubscribeSenderAsync(Message message)
        {
            var senderId = message.Sender.Id;
            await _subscriptionService.AddSubscriptionAsync(senderId, string.Empty).ConfigureAwait(false);

            return new Message { Text = "Successfully subscribed." };
        }
    }
}
