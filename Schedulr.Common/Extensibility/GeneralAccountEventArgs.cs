using Schedulr.Models;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides data for general account events handled by plugins.
    /// </summary>
    public class GeneralAccountEventArgs : EventPluginEventArgs<GeneralAccountEventType>
    {
        /// <summary>
        /// Gets the account related to the event.
        /// </summary>
        public Account Account { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralAccountEventArgs"/> class.
        /// </summary>
        /// <param name="event">The plugin event that occurred.</param>
        /// <param name="applicationInfo">The information about the application.</param>
        /// <param name="account">The account related to the event.</param>
        public GeneralAccountEventArgs(GeneralAccountEventType @event, ApplicationInfo applicationInfo, Account account)
            : base(@event, applicationInfo)
        {
            this.Account = account;
        }
    }
}