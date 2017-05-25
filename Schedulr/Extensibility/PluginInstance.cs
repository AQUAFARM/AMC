using System;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Represents a loaded instance of a plugin.
    /// </summary>
    public sealed class PluginInstance : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the ID of this plugin instance.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the collection that this plugin instance is part of.
        /// </summary>
        public PluginCollection Collection { get; private set; }

        /// <summary>
        /// Gets the plugin type that defines this plugin instance.
        /// </summary>
        public PluginType Type { get; private set; }

        /// <summary>
        /// Gets the actual plugin instance.
        /// </summary>
        private IPlugin Plugin { get; set; }

        /// <summary>
        /// Gets a value that determines if this plugin instance is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the control that is used to edit the settings for this plugin instance.
        /// </summary>
        public object SettingsControl { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInstance"/> class.
        /// </summary>
        /// <param name="id">The ID of this plugin instance.</param>
        /// <param name="collection">The collection that this plugin instance is part of.</param>
        /// <param name="type">The plugin type that defines this plugin instance.</param>
        /// <param name="serializedSettings">The serialized settings object for the plugin.</param>
        /// <param name="isEnabled">A value that determines if this plugin instance is enabled.</param>
        public PluginInstance(string id, PluginCollection collection, PluginType type, string serializedSettings, bool isEnabled)
        {
            this.Id = id;
            this.Collection = collection;
            this.Type = type;
            this.IsEnabled = isEnabled;

            // Create the plugin instance.
            try
            {
                this.Plugin = (IPlugin)Activator.CreateInstance(type.PluginInstanceType);
            }
            catch (Exception exc)
            {
                throw new PluginException(this, "An exception occurred while creating an instance of the specified plugin type (make sure it has a public default constructor).", exc);
            }

            // Initialize the plugin.
            try
            {
                this.Plugin.Initialize(new PluginHost(this));
            }
            catch (Exception exc)
            {
                throw new PluginException(this, "An exception occurred while initializing the plugin.", exc);
            }

            // Initialize the plugin settings.
            var pluginWithSettings = this.Plugin as ISupportSettings;
            if (pluginWithSettings != null)
            {
                try
                {
                    pluginWithSettings.InitializeSettings(serializedSettings);
                }
                catch (Exception exc)
                {
                    throw new PluginException(this, "An exception occurred while initializing the plugin with its settings.", exc);
                }
                try
                {
                    this.SettingsControl = pluginWithSettings.GetSettingsControl();
                }
                catch (Exception exc)
                {
                    throw new PluginException(this, "An exception occurred while retrieving the plugin's settings control.", exc);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the plugin's settings object serialized as a string.
        /// </summary>
        /// <returns>The plugin's settings object serialized as a string.</returns>
        public string GetSerializedSettings()
        {
            var pluginWithSettings = this.Plugin as ISupportSettings;
            if (pluginWithSettings != null)
            {
                try
                {
                    return pluginWithSettings.GetSerializedSettings();
                }
                catch (Exception exc)
                {
                    throw new PluginException(this, "An exception occurred while retrieving the plugin's settings.", exc);
                }
            }
            return null;
        }

        /// <summary>
        /// Determines if the plugin instance is of the specified plugin type.
        /// </summary>
        /// <typeparam name="TPlugin">The type to check for on the plugin instance.</typeparam>
        /// <returns><see langword="true"/> if the plugin instance is of the specified type, <see langword="false"/> otherwise.</returns>
        public bool PluginIsOfType<TPlugin>() where TPlugin : IPlugin
        {
            return this.Plugin is TPlugin;
        }

        /// <summary>
        /// Executes the specified action on the plugin instance.
        /// </summary>
        /// <typeparam name="TPlugin">The plugin type to cast the plugin instance to.</typeparam>
        /// <param name="action">The action to execute on the plugin instance.</param>
        public void Execute<TPlugin>(Action<TPlugin> action) where TPlugin : class, IPlugin
        {
            var instance = this.Plugin as TPlugin;
            if (instance == null)
            {
                throw new ArgumentException("The plugin instance is not of the specified type: " + typeof(TPlugin).FullName);
            }
            try
            {
                action(instance);
            }
            catch (Exception exc)
            {
                throw new PluginException(this, exc.Message, exc);
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.Plugin.Dispose();
            }
            catch (Exception exc)
            {
                throw new PluginException(this, "An exception occurred while disposing the plugin.", exc);
            }
            this.Plugin = null;
        }

        #endregion
    }
}