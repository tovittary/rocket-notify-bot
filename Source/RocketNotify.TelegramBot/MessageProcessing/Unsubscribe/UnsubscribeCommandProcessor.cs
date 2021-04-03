namespace RocketNotify.TelegramBot.MessageProcessing.Unsubscribe
{
    using System;
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Services;
    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Processes the message that contains "Unsubscribe" command.
    /// </summary>
    public class UnsubscribeCommandProcessor : IMessageProcessor
    {
        /// <summary>
        /// The text of the command.
        /// </summary>
        private const string CommandText = "/unsubscribe";

        /// <summary>
        /// Notifications subscriptions management service.
        /// </summary>
        private readonly ISubscriptionService _subscriptionService;

        /// <summary>
        /// The client used to send responses to messages.
        /// </summary>
        private readonly ITelegramMessageSender _responder;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsubscribeCommandProcessor"/> class.
        /// </summary>
        /// <param name="subscriptionService">Notifications subscriptions management service.</param>
        /// <param name="responder">The client used to send responses to messages.</param>
        public UnsubscribeCommandProcessor(ISubscriptionService subscriptionService, ITelegramMessageSender responder)
        {
            _subscriptionService = subscriptionService;
            _responder = responder;
        }

        /// <inheritdoc/>
        public bool IsRelevant(BotMessage message) => message.Text.Contains(CommandText, StringComparison.InvariantCultureIgnoreCase);

        /// <inheritdoc/>
        public async Task<ProcessResult> ProcessAsync(BotMessage message)
        {
            var senderId = message.Sender.Id;

            var responseText = await DoUnsubscribeAsync(senderId).ConfigureAwait(false);
            await _responder.SendMessageAsync(senderId, responseText).ConfigureAwait(false);

            return new ProcessResult { IsFinal = true };
        }

        /// <summary>
        /// Unsubscribes the message sender from notifications.
        /// </summary>
        /// <param name="senderId">The sender identifier.</param>
        /// <returns>Response text.</returns>
        private async Task<string> DoUnsubscribeAsync(long senderId)
        {
            try
            {
                await _subscriptionService.RemoveSubscriptionAsync(senderId).ConfigureAwait(false);
                return "Successfully unsubscribed.";
            }
            catch (SubscriberNotFoundException)
            {
                return "This chat is not yet subscribed.";
            }
            catch (SubscriberOperationException)
            {
                return "Failed to unsubscribe. Try again later.";
            }
        }
    }
}
