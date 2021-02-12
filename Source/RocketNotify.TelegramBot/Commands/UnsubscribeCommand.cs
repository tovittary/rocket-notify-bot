namespace RocketNotify.TelegramBot.Commands
{
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Services;

    using Telegram.Bot.Types;

    /// <summary>
    /// Unsubscribe from notifications command.
    /// </summary>
    public class UnsubscribeCommand : ICommand
    {
        /// <summary>
        /// Notifications subscriptions managing service.
        /// </summary>
        private readonly ISubscriptionService _subscriptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsubscribeCommand"/> class.
        /// </summary>
        /// <param name="subscriptionService">Notifications subscriptions managing service.</param>
        public UnsubscribeCommand(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        /// <inheritdoc />
        public CommandName Name => CommandName.Unsubscribe;

        /// <inheritdoc />
        public async Task<CommandResult> ExecuteAsync(Message message)
        {
            var chatId = message.Chat.Id;

            try
            {
                await _subscriptionService.RemoveSubscriptionAsync(chatId).ConfigureAwait(false);
            }
            catch (SubscriberNotFoundException)
            {
                return new CommandResult { Text = "This chat is not yet subscribed." };
            }
            catch (SubscriberOperationException)
            {
                return new CommandResult { Text = "Failed to unsubscribe. Try again later." };
            }

            return new CommandResult { Text = "Successfully unsubscribed." };
        }
    }
}
