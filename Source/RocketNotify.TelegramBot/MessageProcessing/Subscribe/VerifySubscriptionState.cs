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
    internal class VerifySubscriptionState : IConfigurableMessageProcessingState
    {
        /// <summary>
        /// Notifications subscriptions management service.
        /// </summary>
        private readonly ISubscriptionService _subscriptionService;

        /// <summary>
        /// The message processor.
        /// </summary>
        private IStatefulMessageProcessor _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifySubscriptionState"/> class.
        /// </summary>
        /// <param name="subscriptionService">Notifications subscriptions management service.</param>
        public VerifySubscriptionState(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        /// <inheritdoc/>
        public bool IsFinal => true;

        /// <inheritdoc/>
        public bool IsRelevant(Message message)
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
        public async Task<Message> ProcessAsync(Message message)
        {
            try
            {
                return await SubscribeSenderAsync(message).ConfigureAwait(false);
            }
            catch (SubscriberAlreadyExistsException)
            {
                return new Message { Text = "This chat already subscribed." };
            }
            catch (SubscriptionNotAllowedException)
            {
                return new Message { Text = "Failed to subscribe. Invalid subscription key." };
            }
            catch (SubscriberOperationException)
            {
                return new Message { Text = "Failed to subscribe. Try again later." };
            }
        }

        /// <inheritdoc/>
        public void SetProcessor(IStatefulMessageProcessor processor) => _processor = processor;

        /// <summary>
        /// Adds the message sender as a subscriber.
        /// </summary>
        /// <param name="message">The message being processed.</param>
        /// <returns>The response to the message.</returns>
        private async Task<Message> SubscribeSenderAsync(Message message)
        {
            var subscriptionKey = message.Text;
            var senderId = message.Sender.Id;
            await _subscriptionService.AddSubscriptionAsync(senderId, subscriptionKey).ConfigureAwait(false);

            return new Message { Text = "Successfully subscribed." };
        }
    }
}
