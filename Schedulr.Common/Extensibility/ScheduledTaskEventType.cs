using System.ComponentModel;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Defines the available event types for scheduled tasks.
    /// </summary>
    public enum ScheduledTaskEventType
    {
        /// <summary>
        /// Defines the event that is raised when the scheduled task is about to be created.
        /// </summary>
        [Description("The scheduled task is about to be created")]
        Creating,

        /// <summary>
        /// Defines the event that is raised when the scheduled task has just been created.
        /// </summary>
        [Description("The scheduled task has just been created")]
        Created,

        /// <summary>
        /// Defines the event that is raised when the scheduled task is about to be updated.
        /// </summary>
        [Description("The scheduled task is about to be updated")]
        Updating,

        /// <summary>
        /// Defines the event that is raised when the scheduled task has just been updated.
        /// </summary>
        [Description("The scheduled task has just been updated")]
        Updated,

        /// <summary>
        /// Defines the event that is raised when the scheduled task is about to be enabled or disabled.
        /// </summary>
        [Description("The scheduled task is about to be enabled or disabled")]
        EnabledChanging,

        /// <summary>
        /// Defines the event that is raised when the scheduled task has just been enabled or disabled.
        /// </summary>
        [Description("The scheduled task has just been enabled or disabled")]
        EnabledChanged,

        /// <summary>
        /// Defines the event that is raised when the scheduled task is about to be deleted.
        /// </summary>
        [Description("The scheduled task is about to be deleted")]
        Deleting,

        /// <summary>
        /// Defines the event that is raised when the scheduled task has just been deleted.
        /// </summary>
        [Description("The scheduled task has just been deleted")]
        Deleted,
    }
}