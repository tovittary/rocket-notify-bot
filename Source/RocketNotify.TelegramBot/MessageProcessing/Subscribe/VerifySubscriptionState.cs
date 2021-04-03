namespace RocketNotify.TelegramBot.MessageProcessing.Subscribe
{
    using System;
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Services;
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// The state in which the subscription key provided in the message is verified.
    /// </summary>
    public class VerifySubscriptionState : IConfigurableMessageProcessingState
    {
        /// <summary>
        /// Notifications subscriptions management service.
        /// </summary>
        private readonly ISubscriptionService _subscriptionService;

        /// <summary>
        /// A delegate used for obtaining an instance of the <see cref="SubscriptionCompleteState"/> class.
        /// </summary>
        private readonly Func<SubscriptionCompleteState> _getSubscriptionCompleteState;

        /// <summary>
        /// The message processor.
        /// </summary>
        private IStatefulMessageProcessor _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifySubscriptionState"/> class.
        /// </summary>
        /// <param name="subscriptionService">Notifications subscriptions management service.</param>
        /// <param name="getSubscriptionCompleteState">A delegate used for obtaining an instance of the <see cref="SubscriptionCompleteState"/> class.</param>
        public VerifySubscriptionState(ISubscriptionService subscriptionService, Func<SubscriptionCompleteState> getSubscriptionCompleteState)
        {
            _subscriptionService = subscriptionService;
            _getSubscriptionCompleteState = getSubscriptionCompleteState;
        }

        /// <inheritdoc/>
        public bool IsFinal => false;

        /// <inheritdoc/>
        public bool IsRelevant(BotMessage message)
        {
            var lastResponseFromProcessor = _processor.Context.LastResponse;
            if (lastResponseFromProcessor == null)
                throw new InvalidOperationException("The state of the message processor is invalid");

            var currentMessageReply = message.ReplyInfo;
            if (currentMessageReply == null)
                return false;

            var isReplyToLastResponse = lastResponseFromProcessor.MessageId == currentMessageReply.SourceMessageId;

            return isReplyToLastResponse;
        }

        /// <inheritdoc/>
        public async Task<BotMessage> ProcessAsync(BotMessage message)
        {
            try
            {
                return await SubscribeSenderAsync(message).ConfigureAwait(false);
            }
            catch (SubscriberAlreadyExistsException)
            {
                return new BotMessage { Text = "This chat already subscribed." };
            }
            catch (SubscriptionNotAllowedException)
            {
                return new BotMessage { Text = "Failed to subscribe. Invalid subscription key." };
            }
            catch (SubscriberOperationException)
            {
                return new BotMessage { Text = "Failed to subscribe. Try again later." };
            }
            finally
            {
                SetSubscriptionCompletedState();
            }
        }

        /// <inheritdoc/>
        public void SetProcessor(IStatefulMessageProcessor processor) => _processor = processor;

        /// <summary>
        /// Adds the message sender as a subscriber.
        /// </summary>
        /// <param name="message">The message being processed.</param>
        /// <returns>The response to the message.</returns>
        private async Task<BotMessage> SubscribeSenderAsync(BotMessage message)
        {
            var subscriptionKey = message.Text;
            var senderId = message.Sender.Id;
            await _subscriptionService.AddSubscriptionAsync(senderId, subscriptionKey).ConfigureAwait(false);

            return new BotMessage { Text = "Successfully subscribed." };
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
