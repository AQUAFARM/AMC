using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// Contains settings for plugin collections.
    /// </summary>
    public class PluginCollectionSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the unique identifier of the plugin collection.
        /// </summary>
        [DataMember]
        public string PluginCollectionId
        {
            get { return this.GetValue(PluginCollectionIdProperty); }
            set { this.SetValue(PluginCollectionIdProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PluginCollectionId"/> observable property.
        /// </summary>
        public static ObservableProperty<string> PluginCollectionIdProperty = new ObservableProperty<string, PluginCollectionSettings>(o => o.PluginCollectionId);

        /// <summary>
        /// Gets or sets a value that determines if the collection is expanded.
        /// </summary>
        [DataMember]
        public bool IsExpanded
        {
            get { return this.GetValue(IsExpandedProperty); }
            set { this.SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="IsExpanded"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> IsExpandedProperty = new ObservableProperty<bool, PluginCollectionSettings>(o => o.IsExpanded, true);

        #endregion
    }
}