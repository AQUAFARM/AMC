
namespace Schedulr.Extensibility
{
    /// <summary>
    /// Represents the available categories in which plugins can be loaded.
    /// </summary>
    public enum PluginCategory
    {
        /// <summary>
        /// Defines the application-level events plugin category.
        /// </summary>
        ApplicationEvents,

        /// <summary>
        /// Defines the account-level events plugin category.
        /// </summary>
        AccountEvents,

        /// <summary>
        /// Defines the rendering plugin category.
        /// </summary>
        Rendering
    }
}