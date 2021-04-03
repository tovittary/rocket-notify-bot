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
    public class InitialSubscribeState : IConfigurableMessageProcessingState
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
        /// A delegate used for obtaining an instance of the <see cref="SubscriptionCompleteState"/> class.
        /// </summary>
        private readonly Func<SubscriptionCompleteState> _getSubscriptionCompleteState;

        /// <summary>
        /// The message processor.
        /// </summary>
        private IStatefulMessageProcessor _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitialSubscribeState"/> class.
        /// </summary>
        /// <param name="subscriptionService">Notifications subscriptions management service.</param>
        /// <param name="getVerifySubscriptionState">A delegate used for obtaining an instance of the <see cref="VerifySubscriptionState"/> class.</param>
        /// <param name="getSubscriptionCompleteState">A delegate used for obtaining an instance of the <see cref="SubscriptionCompleteState"/> class.</param>
        public InitialSubscribeState(ISubscriptionService subscriptionService, Func<VerifySubscriptionState> getVerifySubscriptionState, Func<SubscriptionCompleteState> getSubscriptionCompleteState)
        {
            _subscriptionService = subscriptionService;
            _getVerifySubscriptionState = getVerifySubscriptionState;
            _getSubscriptionCompleteState = getSubscriptionCompleteState;
        }

        /// <inheritdoc/>
        public bool IsFinal => false;

        /// <inheritdoc/>
        public bool IsRelevant(BotMessage message) => message.Text.Contains(CommandText, StringComparison.InvariantCultureIgnoreCase);

        /// <inheritdoc/>
        public async Task<BotMessage> ProcessAsync(BotMessage message)
        {
            try
            {
                return await DoProcessAsync(message).ConfigureAwait(false);
            }
            catch (SubscriberAlreadyExistsException)
            {
                SetSubscriptionCompletedState();
                return new BotMessage { Text = "This chat already subscribed." };
            }
            catch (SubscriberOperationException)
            {
                SetSubscriptionCompletedState();
                return new BotMessage { Text = "Failed to subscribe. Try again later." };
            }
        }

        /// <inheritdoc/>
        public void SetProcessor(IStatefulMessageProcessor processor) => _processor = processor;

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>The response to the message.</returns>
        private Task<BotMessage> DoProcessAsync(BotMessage message)
        {
            var secretIsNeeded = _subscriptionService.CheckSubscriptionKeyNeeded();
            return secretIsNeeded ? RequestSubscriptionKeyAsync() : SubscribeSenderAsync(message);
        }

        /// <summary>
        /// Generates a response message requesting a subscription key.
        /// </summary>
        /// <returns>The response to the message.</returns>
        private Task<BotMessage> RequestSubscriptionKeyAsync()
        {
            var replyMarkup = new ForceReplyMarkup();
            var result = new BotMessage { Text = "Provide the subscription key in the reply message to complete the process.", Markup = replyMarkup };

            SetVerifySubscriptionState();
            return Task.FromResult(result);
        }

        /// <summary>
        /// Adds the message sender as a subscriber.
        /// </summary>
        /// <param name="message">The message being processed.</param>
        /// <returns>The response to the message.</returns>
        private async Task<BotMessage> SubscribeSenderAsync(BotMessage message)
        {
            var senderId = message.Sender.Id;
            await _subscriptionService.AddSubscriptionAsync(senderId, string.Empty).ConfigureAwait(false);

            SetSubscriptionCompletedState();
            return new BotMessage { Text = "Successfully subscribed." };
        }

        /// <summary>
        /// Sets the message processor to a state that signals about the necessity of the subscription verification.
        /// </summary>
        private void SetVerifySubscriptionState()
        {
            var newState = _getVerifySubscriptionState();
            _processor.ChangeCurrentState(newState);
        }

        /// <summary>
        /// Sets the message processor to a state that signals about the completion of the subscription process.
        /// </summary>
        private void SetSubscriptionCompletedState()
        {
            var newState = _getSubscriptionCompleteState();
            _processor.ChangeCurrentState(newState);
        }
    }
}
