using Schedulr.Models;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides data for specific account events handled by plugins.
    /// </summary>
    public class AccountEventArgs : EventPluginEventArgs<AccountEventType>
    {
        /// <summary>
        /// Gets the account related to the event.
        /// </summary>
        public Account Account { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountEventArgs"/> class.
        /// </summary>
        /// <param name="event">The plugin event that occurred.</param>
        /// <param name="applicationInfo">The information about the application.</param>
        /// <param name="account">The account related to the event.</param>
        public AccountEventArgs(AccountEventType @event, ApplicationInfo applicationInfo, Account account)
            : base(@event, applicationInfo)
        {
            this.Account = account;
        }
    }
}