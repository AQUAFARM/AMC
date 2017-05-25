using System.IO;
using JelleDruyts.Windows;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides a convenient base class for rendering plugins, so that they only need to override the methods they are interested in.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public abstract class RenderingPlugin : Plugin, IRenderingPlugin
    {
        #region Virtual Methods For IRenderingPlugin

        /// <summary>
        /// Called when a picture or video needs to be rendered before it is uploaded.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        /// <param name="fileToRender">The current stream that contains the file to render.</param>
        /// <returns>The stream that contains the modified file.</returns>
        public virtual Stream OnRenderingFile(RenderingEventArgs args, Stream fileToRender)
        {
            return fileToRender;
        }

        /// <summary>
        /// Called when a file has been completely rendered. Use this to perform cleanup if needed.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public virtual void OnRenderingFileCompleted(RenderingEventArgs args)
        {
        }

        #endregion
    }

    /// <summary>
    /// Provides a convenient base class for rendering plugins that have settings, so that they only need to override the methods they are interested in.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings for this plugin.</typeparam>
    /// <typeparam name="TSettingsControl">The type of the control that is used to edit the settings for this plugin.</typeparam>
    public abstract class RenderingPlugin<TSettings, TSettingsControl> : RenderingPlugin, ISupportSettings where TSettings : class, new()
    {
        #region Properties

        /// <summary>
        /// Gets the settings for this plugin.
        /// </summary>
        protected TSettings Settings { get; private set; }

        /// <summary>
        /// Gets the control that is used to edit the settings for this plugin.
        /// </summary>
        protected TSettingsControl SettingsControl { get; private set; }

        #endregion

        #region ISupportSettings Implementation

        /// <summary>
        /// Initializes the plugin with its settings object serialized as a string.
        /// </summary>
        /// <param name="serializedSettings">The serialized settings object.</param>
        void ISupportSettings.InitializeSettings(string serializedSettings)
        {
            if (string.IsNullOrEmpty(serializedSettings))
            {
                this.Settings = new TSettings();
            }
            else
            {
                this.Settings = SerializationProvider.ReadFromString<TSettings>(serializedSettings);
            }
            this.SettingsControl = GetSettingsControl(this.Settings);
        }

        /// <summary>
        /// Returns the plugin's settings object serialized as a string.
        /// </summary>
        /// <returns>The plugin's settings object serialized as a string.</returns>
        string ISupportSettings.GetSerializedSettings()
        {
            return SerializationProvider.WriteToString<TSettings>(this.Settings, false);
        }

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
        object ISupportSettings.GetSettingsControl()
        {
            return this.SettingsControl;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Returns the control that is used to edit the settings for this plugin.
        /// </summary>
        /// <param name="settings">The settings for this plugin.</param>
        /// <returns>The control that is used to edit the settings for this plugin.</returns>
        /// <remarks>
        /// <para>
        /// The settings control is rendered by WPF so in theory it can be any object, but
        /// typically it will be a WPF User Control.
        /// </para>
        /// <para>
        /// You are responsible for making sure the settings control is bound to its
        /// <paramref name="settings"/> instance.
        /// </para>
        /// </remarks>
        protected abstract TSettingsControl GetSettingsControl(TSettings settings);

        #endregion
    }
}