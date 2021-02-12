namespace RocketNotify.TelegramBot.Commands
{
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Services;

    using Telegram.Bot.Types;

    /// <summary>
    /// Subscribe to notifications command.
    /// </summary>
    public class SubscribeCommand : ICommand
    {
        /// <summary>
        /// Notifications subscriptions managing service.
        /// </summary>
        private readonly ISubscriptionService _subscriptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeCommand"/> class.
        /// </summary>
        /// <param name="subscriptionService">Notifications subscriptions managing service.</param>
        public SubscribeCommand(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        /// <inheritdoc />
        public CommandName Name => CommandName.Subscribe;

        /// <inheritdoc />
        public async Task<CommandResult> ExecuteAsync(Message message)
        {
            var chatId = message.Chat.Id;

            try
            {
                await _subscriptionService.AddSubscriptionAsync(chatId).ConfigureAwait(false);
            }
            catch (SubscriberAlreadyExistsException)
            {
                return new CommandResult { Text = "This chat already subscribed." };
            }
            catch (SubscriberOperationException)
            {
                return new CommandResult { Text = "Failed to subscribe. Try again later." };
            }

            return new CommandResult { Text = "Successfully subscribed." };
        }
    }
}
