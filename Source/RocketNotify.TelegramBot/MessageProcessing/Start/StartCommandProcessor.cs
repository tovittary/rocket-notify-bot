namespace RocketNotify.TelegramBot.MessageProcessing.Start
{
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Processes the message that contains the "Start" command.
    /// </summary>
    internal class StartCommandProcessor : IMessageProcessor
    {
        /// <inheritdoc/>
        public IMessageProcessingState CurrentState => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public void ChangeCurrentState(IMessageProcessingState state)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsRelevant(Message message)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ProcessResult> ProcessAsync(Message message) => CurrentState.ProcessAsync(message);
    }
}
