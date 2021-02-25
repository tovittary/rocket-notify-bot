namespace RocketNotify.TelegramBot.MessageProcessing.Start
{
    using System.Threading.Tasks;

    using RocketNotify.TelegramBot.MessageProcessing.Model;

    /// <summary>
    /// Represents the processing state of the message containing the "Start" command.
    /// </summary>
    internal class StartCommandState : IMessageProcessingState
    {
        /// <inheritdoc/>
        public Task<ProcessResult> ProcessAsync(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}
