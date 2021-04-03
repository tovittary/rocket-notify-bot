namespace RocketNotify.TelegramBot.MessageProcessing.Subscribe
{
    using System;
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// The state of subscription command processor indicating successful subscription to notifications.
    /// </summary>
    public class SubscriptionCompleteState : IConfigurableMessageProcessingState
    {
        /// <inheritdoc/>
        public bool IsFinal => true;

        /// <inheritdoc/>
        public bool IsRelevant(BotMessage message) => false;

        /// <inheritdoc/>
        public Task<BotMessage> ProcessAsync(BotMessage message)
        {
            throw new InvalidOperationException("The subscribe command could not be processed in this state");
        }

        /// <inheritdoc/>
        public void SetProcessor(IStatefulMessageProcessor processor)
        {
        }
    }
}
