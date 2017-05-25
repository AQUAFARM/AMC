using System.ComponentModel;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Defines the available event types for general accounts.
    /// </summary>
    public enum GeneralAccountEventType
    {
        /// <summary>
        /// Defines the event that is raised when an account is about to be added.
        /// </summary>
        [Description("An account is about to be added")]
        Adding,

        /// <summary>
        /// Defines the event that is raised when an account has just been added.
        /// </summary>
        [Description("An account has just been added")]
        Added,

        /// <summary>
        /// Defines the event that is raised when an account is about to be removed.
        /// </summary>
        [Description("An account is about to be removed")]
        Removing,

        /// <summary>
        /// Defines the event that is raised when an account has just been removed.
        /// </summary>
        [Description("An account has just been removed")]
        Removed,
    }
}