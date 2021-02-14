namespace RocketNotify.Subscription.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RocketNotify.Subscription.Model;

    /// <summary>
    /// Provides subscribers storage file management operations.
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Initializes file storage.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Loads existing subscribers data from the storage file as an asynchronous operation.
        /// </summary>
        /// <returns>Subscribers data.</returns>
        Task<ICollection<Subscriber>> LoadSubscribersDataAsync();

        /// <summary>
        /// Saves subscribers data to the storage file.
        /// </summary>
        /// <param name="subscribers">Subscribers data to save.</param>
        void SaveSubscribersData(ICollection<Subscriber> subscribers);
    }
}
