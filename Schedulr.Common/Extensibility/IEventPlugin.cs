
namespace Schedulr.Extensibility
{
    /// <summary>
    /// Represents a plugin that can handle events.
    /// </summary>
    public interface IEventPlugin : IPlugin
    {
        /// <summary>
        /// Called when a specific account plugin event occurred.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        void OnAccountEvent(AccountEventArgs args);

        /// <summary>
        /// Called when an application plugin event occurred.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        void OnApplicationEvent(ApplicationEventArgs args);

        /// <summary>
        /// Called when a batch plugin event occurred.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        void OnBatchEvent(BatchEventArgs args);

        /// <summary>
        /// Called when a configuration plugin event occurred.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        void OnConfigurationEvent(ConfigurationEventArgs args);

        /// <summary>
        /// Called when a general account plugin event occurred.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        void OnGeneralAccountEvent(GeneralAccountEventArgs args);

        /// <summary>
        /// Called when a picture plugin event occurred.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        void OnPictureEvent(PictureEventArgs args);

        /// <summary>
        /// Called when a scheduled task plugin event occurred.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        void OnScheduledTaskEvent(ScheduledTaskEventArgs args);
    }
}