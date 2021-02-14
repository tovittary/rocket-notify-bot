namespace RocketNotify.TelegramBot.Messages.Filtration
{
    /// <summary>
    /// An action to be taken based on message filtration result.
    /// </summary>
    public enum FiltrationAction
    {
        /// <summary>
        /// Further filtering required.
        /// </summary>
        NextFilter,

        /// <summary>
        /// Message processing allowed.
        /// </summary>
        Process,

        /// <summary>
        /// Message should be ignored.
        /// </summary>
        Ignore
    }
}
