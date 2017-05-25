using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// Contains all UI settings for picture details.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class PictureDetailsUISettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets a value that determines if the entire picture details panel is expanded.
        /// </summary>
        [DataMember]
        public bool PictureDetailsPanelIsExpanded
        {
            get { return this.GetValue(PictureDetailsPanelIsExpandedProperty); }
            set { this.SetValue(PictureDetailsPanelIsExpandedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PictureDetailsPanelIsExpanded"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> PictureDetailsPanelIsExpandedProperty = new ObservableProperty<bool, PictureDetailsUISettings>(o => o.PictureDetailsPanelIsExpanded);

        /// <summary>
        /// Gets or sets a value that determines if the entire picture details panel is locked.
        /// </summary>
        [DataMember]
        public bool PictureDetailsPanelIsLocked
        {
            get { return this.GetValue(PictureDetailsPanelIsLockedProperty); }
            set { this.SetValue(PictureDetailsPanelIsLockedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PictureDetailsPanelIsLocked"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> PictureDetailsPanelIsLockedProperty = new ObservableProperty<bool, PictureDetailsUISettings>(o => o.PictureDetailsPanelIsLocked);

        /// <summary>
        /// Gets or sets a value that determines if the picture details Online panel is expanded.
        /// </summary>
        [DataMember]
        public bool OnlinePanelIsExpanded
        {
            get { return this.GetValue(OnlinePanelIsExpandedProperty); }
            set { this.SetValue(OnlinePanelIsExpandedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="OnlinePanelIsExpanded"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> OnlinePanelIsExpandedProperty = new ObservableProperty<bool, PictureDetailsUISettings>(o => o.OnlinePanelIsExpanded, true);

        /// <summary>
        /// Gets or sets a value that determines if the picture details Details panel is expanded.
        /// </summary>
        [DataMember]
        public bool DetailsPanelIsExpanded
        {
            get { return this.GetValue(DetailsPanelIsExpandedProperty); }
            set { this.SetValue(DetailsPanelIsExpandedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="DetailsPanelIsExpanded"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> DetailsPanelIsExpandedProperty = new ObservableProperty<bool, PictureDetailsUISettings>(o => o.DetailsPanelIsExpanded, true);

        /// <summary>
        /// Gets or sets a value that determines if the picture details Settings panel is expanded.
        /// </summary>
        [DataMember]
        public bool SettingsPanelIsExpanded
        {
            get { return this.GetValue(SettingsPanelIsExpandedProperty); }
            set { this.SetValue(SettingsPanelIsExpandedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="SettingsPanelIsExpanded"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> SettingsPanelIsExpandedProperty = new ObservableProperty<bool, PictureDetailsUISettings>(o => o.SettingsPanelIsExpanded, true);

        /// <summary>
        /// Gets or sets a value that determines if the picture details Sets panel is expanded.
        /// </summary>
        [DataMember]
        public bool SetsPanelIsExpanded
        {
            get { return this.GetValue(SetsPanelIsExpandedProperty); }
            set { this.SetValue(SetsPanelIsExpandedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="SetsPanelIsExpanded"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> SetsPanelIsExpandedProperty = new ObservableProperty<bool, PictureDetailsUISettings>(o => o.SetsPanelIsExpanded, true);

        /// <summary>
        /// Gets or sets a value that determines if the picture details Groups panel is expanded.
        /// </summary>
        [DataMember]
        public bool GroupsPanelIsExpanded
        {
            get { return this.GetValue(GroupsPanelIsExpandedProperty); }
            set { this.SetValue(GroupsPanelIsExpandedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="GroupsPanelIsExpanded"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> GroupsPanelIsExpandedProperty = new ObservableProperty<bool, PictureDetailsUISettings>(o => o.GroupsPanelIsExpanded, true);

        /// <summary>
        /// Gets or sets a value that determines if the picture details Location panel is expanded.
        /// </summary>
        [DataMember]
        public bool LocationPanelIsExpanded
        {
            get { return this.GetValue(LocationPanelIsExpandedProperty); }
            set { this.SetValue(LocationPanelIsExpandedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="LocationPanelIsExpanded"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> LocationPanelIsExpandedProperty = new ObservableProperty<bool, PictureDetailsUISettings>(o => o.LocationPanelIsExpanded, true);

        /// <summary>
        /// Gets or sets a value that determines if the picture details Batch panel is expanded.
        /// </summary>
        [DataMember]
        public bool BatchPanelIsExpanded
        {
            get { return this.GetValue(BatchPanelIsExpandedProperty); }
            set { this.SetValue(BatchPanelIsExpandedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="BatchPanelIsExpanded"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> BatchPanelIsExpandedProperty = new ObservableProperty<bool, PictureDetailsUISettings>(o => o.BatchPanelIsExpanded, true);

        #endregion
    }
}
