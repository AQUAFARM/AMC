using Schedulr.Models;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides data for configuration events handled by plugins.
    /// </summary>
    public class ConfigurationEventArgs : EventPluginEventArgs<ConfigurationEventType>
    {
        /// <summary>
        /// Gets the configuration related to the event.
        /// </summary>
        public SchedulrConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets the full path of the configuration file.
        /// </summary>
        public string ConfigurationFileName { get; private set; }

        /// <summary>
        /// When loading: gets a value that determines if the configuration was just created, i.e. not loaded from an existing file.
        /// </summary>
        public bool IsNew { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationEventArgs"/> class.
        /// </summary>
        /// <param name="event">The plugin event that occurred.</param>
        /// <param name="applicationInfo">The information about the application.</param>
        /// <param name="configuration">The configuration related to the event.</param>
        /// <param name="configurationFileName">The full path of the configuration file.</param>
        /// <param name="isNew">When loading: a value that determines if the configuration was just created, i.e. not loaded from an existing file.</param>
        public ConfigurationEventArgs(ConfigurationEventType @event, ApplicationInfo applicationInfo, SchedulrConfiguration configuration, string configurationFileName, bool isNew)
            : base(@event, applicationInfo)
        {
            this.Configuration = configuration;
            this.ConfigurationFileName = configurationFileName;
            this.IsNew = isNew;
        }
    }
}