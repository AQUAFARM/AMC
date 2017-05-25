
namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides data for events handled by plugins.
    /// </summary>
    /// <typeparam name="TEventType">The type of the event handled by the plugin.</typeparam>
    public class EventPluginEventArgs<TEventType> : PluginEventArgs where TEventType : struct
    {
        /// <summary>
        /// Gets or sets the plugin event that occurred.
        /// </summary>
        public TEventType Event { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPluginEventArgs&lt;TEventType&gt;"/> class.
        /// </summary>
        /// <param name="event">The plugin event that occurred.</param>
        /// <param name="applicationInfo">The information about the application.</param>
        public EventPluginEventArgs(TEventType @event, ApplicationInfo applicationInfo)
            : base(applicationInfo)
        {
            this.Event = @event;
        }
    }
}