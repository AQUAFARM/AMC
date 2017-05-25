using System.ComponentModel;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Defines the available event types for batches.
    /// </summary>
    public enum BatchEventType
    {
        /// <summary>
        /// Defines the event that is raised when a batch is about to be added to the queue.
        /// </summary>
        [Description("A batch is about to be added to the queue")]
        Adding,

        /// <summary>
        /// Defines the event that is raised when a batch has just been added to the queue.
        /// </summary>
        [Description("A batch has just been added to the queue")]
        Added,

        /// <summary>
        /// Defines the event that is raised when a batch is about to be removed from the queue.
        /// </summary>
        [Description("A batch is about to be removed from the queue")]
        Removing,

        /// <summary>
        /// Defines the event that is raised when a batch has just been removed from the queue.
        /// </summary>
        [Description("A batch has just been removed from the queue")]
        Removed
    }
}
