
namespace Schedulr.Models
{
    /// <summary>
    /// Determines the type of a scheduled task.
    /// </summary>
    public enum ScheduleType
    {
        /// <summary>
        /// The task executes on a daily basis.
        /// </summary>
        Daily,

        /// <summary>
        /// The task executes on a weekly basis.
        /// </summary>
        Weekly
    }
}