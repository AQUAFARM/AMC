using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// Contains the configuration of a plugin.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class PluginConfiguration : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the ID of this plugin instance.
        /// </summary>
        [DataMember]
        public string Id
        {
            get { return this.GetValue(IdProperty); }
            set { this.SetValue(IdProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Id"/> observable property.
        /// </summary>
        public static ObservableProperty<string> IdProperty = new ObservableProperty<string, PluginConfiguration>(o => o.Id);

        /// <summary>
        /// Gets or sets a value that determines if the plugin is enabled.
        /// </summary>
        [DataMember]
        public bool IsEnabled
        {
            get { return this.GetValue(IsEnabledProperty); }
            set { this.SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="IsEnabled"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> IsEnabledProperty = new ObservableProperty<bool, PluginConfiguration>(o => o.IsEnabled);

        /// <summary>
        /// Gets or sets the serialized plugin settings.
        /// </summary>
        [DataMember]
        public string Settings
        {
            get { return this.GetValue(SettingsProperty); }
            set { this.SetValue(SettingsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Settings"/> observable property.
        /// </summary>
        public static ObservableProperty<string> SettingsProperty = new ObservableProperty<string, PluginConfiguration>(o => o.Settings);

        /// <summary>
        /// Gets or sets the ID of the collection to which this plugin belongs.
        /// </summary>
        [DataMember]
        public string CollectionId
        {
            get { return this.GetValue(CollectionIdProperty); }
            set { this.SetValue(CollectionIdProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="CollectionId"/> observable property.
        /// </summary>
        public static ObservableProperty<string> CollectionIdProperty = new ObservableProperty<string, PluginConfiguration>(o => o.CollectionId);

        /// <summary>
        /// Gets or sets the ID of the plugin type from which the plugin is constructed.
        /// </summary>
        [DataMember]
        public string PluginTypeId
        {
            get { return this.GetValue(PluginTypeIdProperty); }
            set { this.SetValue(PluginTypeIdProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PluginTypeId"/> observable property.
        /// </summary>
        public static ObservableProperty<string> PluginTypeIdProperty = new ObservableProperty<string, PluginConfiguration>(o => o.PluginTypeId);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        /// <param name="id">The ID of this plugin instance.</param>
        /// <param name="isEnabled">A value that determines if the plugin is enabled.</param>
        /// <param name="settings">The serialized plugin settings.</param>
        /// <param name="collectionId">The ID of the collection to which this plugin belongs.</param>
        /// <param name="pluginTypeId">The ID of the plugin type from which the plugin is constructed.</param>
        public PluginConfiguration(string id, bool isEnabled, string settings, string collectionId, string pluginTypeId)
        {
            this.Id = id;
            this.IsEnabled = isEnabled;
            this.Settings = settings;
            this.CollectionId = collectionId;
            this.PluginTypeId = pluginTypeId;
        }

        #endregion
    }
}