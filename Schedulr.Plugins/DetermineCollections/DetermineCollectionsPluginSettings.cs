using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.DetermineCollections
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class DetermineCollectionsPluginSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets a value that determines if sets should be determined from folders.
        /// </summary>
        [DataMember]
        public bool DetermineSets
        {
            get { return this.GetValue(DetermineSetsProperty); }
            set { this.SetValue(DetermineSetsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="DetermineSets"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> DetermineSetsProperty = new ObservableProperty<bool, DetermineCollectionsPluginSettings>(o => o.DetermineSets, true);

        /// <summary>
        /// Gets or sets a value that determines if groups should be determined from folders.
        /// </summary>
        [DataMember]
        public bool DetermineGroups
        {
            get { return this.GetValue(DetermineGroupsProperty); }
            set { this.SetValue(DetermineGroupsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="DetermineGroups"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> DetermineGroupsProperty = new ObservableProperty<bool, DetermineCollectionsPluginSettings>(o => o.DetermineGroups, true);

        /// <summary>
        /// Gets or sets a value that determines if only the folder that the file is in should be taken into account, or all folders recursively up to the root.
        /// </summary>
        [DataMember]
        public bool DetermineRecursively
        {
            get { return this.GetValue(DetermineRecursivelyProperty); }
            set { this.SetValue(DetermineRecursivelyProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="DetermineRecursively"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> DetermineRecursivelyProperty = new ObservableProperty<bool, DetermineCollectionsPluginSettings>(o => o.DetermineRecursively, true);

        #endregion
    }
}
