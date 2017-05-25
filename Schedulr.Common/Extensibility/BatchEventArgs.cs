using Schedulr.Models;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides data for batch events handled by plugins.
    /// </summary>
    public class BatchEventArgs : EventPluginEventArgs<BatchEventType>
    {
        #region Properties

        /// <summary>
        /// Gets the account related to the event.
        /// </summary>
        public Account Account { get; private set; }

        /// <summary>
        /// Gets the batch related to the event.
        /// </summary>
        public Batch Batch { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureEventArgs"/> class.
        /// </summary>
        /// <param name="event">The plugin event that occurred.</param>
        /// <param name="applicationInfo">The information about the application.</param>
        /// <param name="account">The account related to the event.</param>
        /// <param name="batch">The batch related to the event.</param>
        public BatchEventArgs(BatchEventType @event, ApplicationInfo applicationInfo, Account account, Batch batch)
            : base(@event, applicationInfo)
        {
            this.Account = account;
            this.Batch = batch;
        }

        #endregion
    }
}