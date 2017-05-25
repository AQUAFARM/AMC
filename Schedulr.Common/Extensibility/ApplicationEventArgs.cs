
namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides data for application events handled by plugins.
    /// </summary>
    public class ApplicationEventArgs : EventPluginEventArgs<ApplicationEventType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationEventArgs"/> class.
        /// </summary>
        /// <param name="event">The plugin event that occurred.</param>
        /// <param name="applicationInfo">The information about the application.</param>
        public ApplicationEventArgs(ApplicationEventType @event, ApplicationInfo applicationInfo)
            : base(@event, applicationInfo)
        {
        }
    }
}