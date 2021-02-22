namespace RocketNotify.TelegramBot.Commands
{
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Services;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.ReplyMarkups;

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
            try
            {
                var secretIsNeeded = _subscriptionService.CheckSubscriptionKeyNeeded();
                if (secretIsNeeded)
                {
                    var replyMarkup = new ForceReplyMarkup();
                    return new CommandResult { ResponseText = "Provide the subscription key to complete the process.", ReplyMarkup = replyMarkup };
                }

                var senderId = message.Chat.Id;
                await _subscriptionService.AddSubscriptionAsync(senderId, string.Empty).ConfigureAwait(false);

                return new CommandResult { ResponseText = "Successfully subscribed." };
            }
            catch (SubscriberAlreadyExistsException)
            {
                return new CommandResult { ResponseText = "This chat already subscribed." };
            }
            catch (SubscriberOperationException)
            {
                return new CommandResult { ResponseText = "Failed to subscribe. Try again later." };
            }
        }
    }
}
