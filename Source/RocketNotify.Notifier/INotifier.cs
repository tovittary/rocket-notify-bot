namespace RocketNotify.Notifier
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Messages notification service interface.
    /// </summary>
    public interface INotifier
    {
        /// <summary>
        /// Starts the Rocket.Chat messages monitoring process.
        /// </summary>
        /// <param name="token">The token for stopping new messages monitoring.</param>
        /// <returns>A task that represents the messages monitoring process.</returns>
        Task StartAsync(CancellationToken token);
    }
}
