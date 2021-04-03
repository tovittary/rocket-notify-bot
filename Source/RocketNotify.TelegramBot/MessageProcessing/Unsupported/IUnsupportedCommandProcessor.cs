namespace RocketNotify.TelegramBot.MessageProcessing.Unsupported
{
    /// <summary>
    /// A marker interface to indicate a default processor for a message containing an unsupported command.
    /// </summary>
    public interface IUnsupportedCommandProcessor : IMessageProcessor
    {
    }
}
