using System.ComponentModel;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Defines the available event types for specific accounts.
    /// </summary>
    public enum AccountEventType
    {
        /// <summary>
        /// Defines the event that is raised when an account has just been activated.
        /// </summary>
        [Description("An account has just been activated")]
        Activated,

        /// <summary>
        /// Defines the event that is raised when an account has just been deactivated.
        /// </summary>
        [Description("An account has just been deactivated")]
        Deactivated,

        /// <summary>
        /// Defines the event that is raised when an account is about to have its information refreshed from Flickr.
        /// </summary>
        [Description("An account is about to have its information refreshed from Flickr")]
        Refreshing,

        /// <summary>
        /// Defines the event that is raised when an account has just refreshed its information from Flickr.
        /// </summary>
        [Description("An account has just refreshed its information from Flickr")]
        Refreshed
    }
}