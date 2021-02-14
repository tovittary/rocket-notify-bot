namespace RocketNotify.TelegramBot.Commands
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;

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
        /// Application settings.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeCommand"/> class.
        /// </summary>
        /// <param name="subscriptionService">Notifications subscriptions managing service.</param>
        /// <param name="configuration">Application settings.</param>
        public SubscribeCommand(ISubscriptionService subscriptionService, IConfiguration configuration)
        {
            _subscriptionService = subscriptionService;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public CommandName Name => CommandName.Subscribe;

        /// <summary>
        /// Gets the name of the command, as it appears in the chat.
        /// </summary>
        private string CommandNameInChat => $"/{Name.ToString().ToLower()}";

        /// <inheritdoc />
        public async Task<CommandResult> ExecuteAsync(Message message)
        {
            var chatId = message.Chat.Id;

            try
            {
                VerifySubscriptionAllowed(message);
                await _subscriptionService.AddSubscriptionAsync(chatId).ConfigureAwait(false);
            }
            catch (SubscriberAlreadyExistsException)
            {
                return new CommandResult { Text = "This chat already subscribed." };
            }
            catch (SubscriptionNotAllowedException)
            {
                return new CommandResult { Text = "No subscription allowed for this chat. Enter your subscription secret after the command name." };
            }
            catch (SubscriberOperationException)
            {
                return new CommandResult { Text = "Failed to subscribe. Try again later." };
            }

            return new CommandResult { Text = "Successfully subscribed." };
        }

        /// <summary>
        /// Checks whether the chat is allowed to subscribe to notifications or not.
        /// </summary>
        /// <param name="message">The message instance.</param>
        public void VerifySubscriptionAllowed(Message message)
        {
            var secret = _configuration.GetSection("Subscriptions")?["Secret"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(secret))
                throw new SubscriptionNotAllowedException("Can't find subscription secret in the configuration file");

            var messageWords = message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var commandAndArguments = messageWords.SkipWhile(word => !word.StartsWith(CommandNameInChat, StringComparison.InvariantCultureIgnoreCase)).ToArray();
            if (commandAndArguments.Length < 2)
                throw new SubscriptionNotAllowedException("Can't get subscription secret from the message");

            var messageSecret = commandAndArguments[1];
            if (messageSecret.Equals(secret))
                return;

            throw new SubscriptionNotAllowedException("The subscription secret in the message is invalid");
        }
    }
}
