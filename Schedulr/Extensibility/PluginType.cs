using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Defines a plugin.
    /// </summary>
    public class PluginType
    {
        #region Properties

        /// <summary>
        /// Gets the actual type of the plugin instances.
        /// </summary>
        public Type PluginInstanceType { get; private set; }

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the purpose of the plugin (i.e. the short description).
        /// </summary>
        public string Purpose { get; private set; }

        /// <summary>
        /// Gets the description of the plugin (i.e. the full explanation of what it does).
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the optional author of the plugin.
        /// </summary>
        public string Author { get; private set; }

        /// <summary>
        /// Gets the optional version of the plugin.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Gets the optional URL of the plugin.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the instantiation policy of the plugin.
        /// </summary>
        public PluginInstantiationPolicy InstantiationPolicy { get; private set; }

        /// <summary>
        /// Gets the specific account events that are supported by this plugin.
        /// </summary>
        public IList<AccountEventType> SupportedAccountEvents { get; private set; }

        /// <summary>
        /// Gets the application events that are supported by this plugin.
        /// </summary>
        public IList<ApplicationEventType> SupportedApplicationEvents { get; private set; }

        /// <summary>
        /// Gets the batch events that are supported by this plugin.
        /// </summary>
        public IList<BatchEventType> SupportedBatchEvents { get; private set; }

        /// <summary>
        /// Gets the configuration events that are supported by this plugin.
        /// </summary>
        public IList<ConfigurationEventType> SupportedConfigurationEvents { get; private set; }

        /// <summary>
        /// Gets the general account events that are supported by this plugin.
        /// </summary>
        public IList<GeneralAccountEventType> SupportedGeneralAccountEvents { get; private set; }

        /// <summary>
        /// Gets the picture events that are supported by this plugin.
        /// </summary>
        public IList<PictureEventType> SupportedPictureEvents { get; private set; }

        /// <summary>
        /// Gets the scheduled task events that are supported by this plugin.
        /// </summary>
        public IList<ScheduledTaskEventType> SupportedScheduledTaskEvents { get; private set; }

        /// <summary>
        /// Gets the rendering types that are supported by this plugin.
        /// </summary>
        public IList<RenderingType> SupportedRenderingTypes { get; private set; }

        /// <summary>
        /// Gets a unique identifier of this plugin type.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the available plugin collections that are supported by this plugin type.
        /// </summary>
        public ObservableCollection<PluginCollection> AvailablePluginCollections { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginType"/> class.
        /// </summary>
        /// <param name="pluginInstanceType">The actual type of the plugin instances.</param>
        /// <param name="metadata">The plugin type's metadata.</param>
        public PluginType(Type pluginInstanceType, IDictionary<string, object> metadata)
        {
            this.PluginInstanceType = pluginInstanceType;
            this.Id = PluginManager.GetPluginTypeId(pluginInstanceType);
            if (!typeof(IPlugin).IsAssignableFrom(pluginInstanceType))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The discovered plugin type \"{0}\" is not of the required type \"{1}\".", pluginInstanceType.FullName, typeof(IPlugin).FullName));
            }
            this.Name = GetMetadataEntry<string>(metadata, "Name");
            this.Purpose = GetMetadataEntry<string>(metadata, "Purpose");
            this.Description = GetMetadataEntry<string>(metadata, "Description");
            this.Author = GetMetadataEntry<string>(metadata, "Author");
            this.Version = GetMetadataEntry<string>(metadata, "Version");
            this.Url = GetMetadataEntry<string>(metadata, "Url");
            this.InstantiationPolicy = GetMetadataEntry<PluginInstantiationPolicy>(metadata, "InstantiationPolicy");
            this.SupportedAccountEvents = GetMetadataEntry<IList<AccountEventType>>(metadata, "SupportedAccountEvent") ?? new AccountEventType[0];
            this.SupportedApplicationEvents = GetMetadataEntry<IList<ApplicationEventType>>(metadata, "SupportedApplicationEvent") ?? new ApplicationEventType[0];
            this.SupportedBatchEvents = GetMetadataEntry<IList<BatchEventType>>(metadata, "SupportedBatchEvent") ?? new BatchEventType[0];
            this.SupportedConfigurationEvents = GetMetadataEntry<IList<ConfigurationEventType>>(metadata, "SupportedConfigurationEvent") ?? new ConfigurationEventType[0];
            this.SupportedGeneralAccountEvents = GetMetadataEntry<IList<GeneralAccountEventType>>(metadata, "SupportedGeneralAccountEvent") ?? new GeneralAccountEventType[0];
            this.SupportedPictureEvents = GetMetadataEntry<IList<PictureEventType>>(metadata, "SupportedPictureEvent") ?? new PictureEventType[0];
            this.SupportedScheduledTaskEvents = GetMetadataEntry<IList<ScheduledTaskEventType>>(metadata, "SupportedScheduledTaskEvent") ?? new ScheduledTaskEventType[0];
            this.SupportedRenderingTypes = GetMetadataEntry<IList<RenderingType>>(metadata, "SupportedRendering") ?? new RenderingType[0];
            this.AvailablePluginCollections = new ObservableCollection<PluginCollection>();
        }

        #endregion

        #region Helper Methods

        private static T GetMetadataEntry<T>(IDictionary<string, object> metadata, string key)
        {
            if (metadata == null || !metadata.ContainsKey(key))
            {
                return default(T);
            }
            return (T)metadata[key];
        }

        #endregion
    }
}