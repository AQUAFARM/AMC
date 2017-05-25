
namespace Schedulr.Extensibility
{
    /// <summary>
    /// Represents the possible plugin instantiation policies.
    /// </summary>
    public enum PluginInstantiationPolicy
    {
        /// <summary>
        /// Indicates that there can be multiple instances of the plugin in the same scope (e.g. per event for an event plugin).
        /// </summary>
        MultipleInstancesPerScope,

        /// <summary>
        /// Indicates that there can only be a single instance of the plugin in the same scope (e.g. per event for an event plugin).
        /// </summary>
        SingleInstancePerScope,

        /// <summary>
        /// Indicates that there can only be a single instance of the plugin in the entire application.
        /// </summary>
        SingleInstancePerApplication
    }
}
