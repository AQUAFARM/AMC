using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// Represents the configuration for Schedulr.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class SchedulrConfiguration : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the configured accounts.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public ObservableCollection<Account> Accounts
        {
            get { return this.GetValue(AccountsProperty); }
            set { this.SetValue(AccountsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Accounts"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<Account>> AccountsProperty = new ObservableProperty<ObservableCollection<Account>, SchedulrConfiguration>(o => o.Accounts);

        /// <summary>
        /// Gets or sets the UI settings.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public UISettings UISettings
        {
            get { return this.GetValue(UISettingsProperty); }
            set { this.SetValue(UISettingsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UISettings"/> observable property.
        /// </summary>
        public static ObservableProperty<UISettings> UISettingsProperty = new ObservableProperty<UISettings, SchedulrConfiguration>(o => o.UISettings);

        /// <summary>
        /// Gets or sets the application-level plugins.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public ObservableCollection<PluginConfiguration> Plugins
        {
            get { return this.GetValue(PluginsProperty); }
            set { this.SetValue(PluginsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Plugins"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<PluginConfiguration>> PluginsProperty = new ObservableProperty<ObservableCollection<PluginConfiguration>, SchedulrConfiguration>(o => o.Plugins);

        /// <summary>
        /// Gets or sets the settings for the plugin collections.
        /// </summary>
        [DataMember]
        public ObservableCollection<PluginCollectionSettings> PluginCollectionSettings
        {
            get { return this.GetValue(PluginCollectionSettingsProperty); }
            set { this.SetValue(PluginCollectionSettingsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PluginCollectionSettings"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<PluginCollectionSettings>> PluginCollectionSettingsProperty = new ObservableProperty<ObservableCollection<PluginCollectionSettings>, SchedulrConfiguration>(o => o.PluginCollectionSettings);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulrConfiguration"/> class.
        /// </summary>
        public SchedulrConfiguration()
        {
            this.Accounts = new ObservableCollection<Account>();
            this.UISettings = new UISettings();
            this.Plugins = new ObservableCollection<PluginConfiguration>();
            this.PluginCollectionSettings = new ObservableCollection<PluginCollectionSettings>();
        }

        #endregion
    }
}