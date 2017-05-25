using Schedulr.Models;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides data for scheduled task events handled by plugins.
    /// </summary>
    public class ScheduledTaskEventArgs : EventPluginEventArgs<ScheduledTaskEventType>
    {
        #region Properties

        /// <summary>
        /// Gets the account related to the event.
        /// </summary>
        public Account Account { get; private set; }

        /// <summary>
        /// Gets the name of the related Windows Scheduled Task.
        /// </summary>
        public string ScheduledTaskName { get; private set; }

        /// <summary>
        /// Gets a value that determines if the related Windows Scheduled Task is enabled.
        /// </summary>
        public bool ScheduledTaskIsEnabled { get; private set; }

        /// <summary>
        /// Gets the upload schedule related to the event.
        /// </summary>
        public UploadSchedule UploadSchedule { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTaskEventArgs"/> class.
        /// </summary>
        /// <param name="event">The plugin event that occurred.</param>
        /// <param name="applicationInfo">The information about the application.</param>
        /// <param name="account">The account related to the event.</param>
        /// <param name="scheduledTaskName">Name of the scheduled task.</param>
        /// <param name="scheduledTaskIsEnabled">A value that determines if the related Windows Scheduled Task is enabled.</param>
        /// <param name="uploadSchedule">The upload schedule related to the event.</param>
        public ScheduledTaskEventArgs(ScheduledTaskEventType @event, ApplicationInfo applicationInfo, Account account, string scheduledTaskName, bool scheduledTaskIsEnabled, UploadSchedule uploadSchedule)
            : base(@event, applicationInfo)
        {
            this.Account = account;
            this.ScheduledTaskName = scheduledTaskName;
            this.ScheduledTaskIsEnabled = scheduledTaskIsEnabled;
            this.UploadSchedule = uploadSchedule;
        }

        #endregion
    }
}