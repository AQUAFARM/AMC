using System.ComponentModel;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Defines the available event types for the application.
    /// </summary>
    public enum ApplicationEventType
    {
        /// <summary>
        /// Defines the event that is raised when the application has started.
        /// </summary>
        [Description("The application has started")]
        Started,

        /// <summary>
        /// Defines the event that is raised when the application is closing.
        /// </summary>
        [Description("The application is closing")]
        Closing
    }
}