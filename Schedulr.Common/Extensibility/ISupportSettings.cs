
namespace Schedulr.Extensibility
{
    /// <summary>
    /// Represents a plugin that supports settings.
    /// </summary>
    public interface ISupportSettings
    {
        /// <summary>
        /// Initializes the plugin with its settings object serialized as a string.
        /// </summary>
        /// <param name="serializedSettings">The serialized settings object.</param>
        void InitializeSettings(string serializedSettings);

        /// <summary>
        /// Returns the plugin's settings object serialized as a string.
        /// </summary>
        /// <returns>The plugin's settings object serialized as a string.</returns>
        string GetSerializedSettings();

        /// <summary>
        /// Returns the control that is used to edit the settings for this plugin.
        /// </summary>
        /// <returns>The control that is used to edit the settings for this plugin.</returns>
        /// <remarks>
        /// <para>
        /// The settings control is rendered by WPF so in theory it can be any object, but
        /// typically it will be a WPF User Control.
        /// </para>
        /// <para>
        /// You are responsible for making sure the settings control is bound to its settings
        /// instance.
        /// </para>
        /// </remarks>
        object GetSettingsControl();
    }
}