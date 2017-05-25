using System.ComponentModel;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Defines the available event types for the configuration.
    /// </summary>
    public enum ConfigurationEventType
    {
        /// <summary>
        /// Defines the event that is raised when the configuration has just been loaded.
        /// </summary>
        [Description("The configuration has just been loaded")]
        Loaded,

        /// <summary>
        /// Defines the event that is raised when the configuration is about to be saved.
        /// </summary>
        [Description("The configuration is about to be saved")]
        Saving,

        /// <summary>
        /// Defines the event that is raised when the configuration has just been saved.
        /// </summary>
        [Description("The configuration has just been saved")]
        Saved,

        /// <summary>
        /// Defines the event that is raised when the configuration is about to be exported.
        /// </summary>
        [Description("The configuration is about to be exported")]
        Exporting,

        /// <summary>
        /// Defines the event that is raised when the configuration has just been exported.
        /// </summary>
        [Description("The configuration has just been exported")]
        Exported,
    }
}