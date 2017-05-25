using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// Contains all settings for an <see cref="Account"/>.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class AccountSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the default values for new pictures.
        /// </summary>
        [DataMember]
        public Picture PictureDefaults
        {
            get { return this.GetValue(PictureDefaultsProperty); }
            set { this.SetValue(PictureDefaultsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PictureDefaults"/> observable property.
        /// </summary>
        public static ObservableProperty<Picture> PictureDefaultsProperty = new ObservableProperty<Picture, AccountSettings>(o => o.PictureDefaults);

        /// <summary>
        /// Gets or sets the display mode of picture collections.
        /// </summary>
        [DataMember]
        public PictureCollectionDisplayMode PictureCollectionDisplayMode
        {
            get { return this.GetValue(PictureCollectionDisplayModeProperty); }
            set { this.SetValue(PictureCollectionDisplayModeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PictureCollectionDisplayMode"/> observable property.
        /// </summary>
        public static ObservableProperty<PictureCollectionDisplayMode> PictureCollectionDisplayModeProperty = new ObservableProperty<PictureCollectionDisplayMode, AccountSettings>(o => o.PictureCollectionDisplayMode, PictureCollectionDisplayMode.TextAndIcon);

        /// <summary>
        /// Gets or sets the sort mode of sets.
        /// </summary>
        [DataMember]
        public PictureCollectionSortMode SetsSortMode
        {
            get { return this.GetValue(SetsSortModeProperty); }
            set { this.SetValue(SetsSortModeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="SetsSortMode"/> observable property.
        /// </summary>
        public static ObservableProperty<PictureCollectionSortMode> SetsSortModeProperty = new ObservableProperty<PictureCollectionSortMode, AccountSettings>(o => o.SetsSortMode, PictureCollectionSortMode.UserOrderAscending);

        /// <summary>
        /// Gets or sets the sort mode of groups.
        /// </summary>
        [DataMember]
        public PictureCollectionSortMode GroupsSortMode
        {
            get { return this.GetValue(GroupsSortModeProperty); }
            set { this.SetValue(GroupsSortModeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="GroupsSortMode"/> observable property.
        /// </summary>
        public static ObservableProperty<PictureCollectionSortMode> GroupsSortModeProperty = new ObservableProperty<PictureCollectionSortMode, AccountSettings>(o => o.GroupsSortMode, PictureCollectionSortMode.NameAscending);

        /// <summary>
        /// Gets or sets the display mode of the picture queue.
        /// </summary>
        [DataMember]
        public PictureQueueDisplayMode PictureQueueDisplayMode
        {
            get { return this.GetValue(PictureQueueDisplayModeProperty); }
            set { this.SetValue(PictureQueueDisplayModeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PictureQueueDisplayMode"/> observable property.
        /// </summary>
        public static ObservableProperty<PictureQueueDisplayMode> PictureQueueDisplayModeProperty = new ObservableProperty<PictureQueueDisplayMode, AccountSettings>(o => o.PictureQueueDisplayMode, PictureQueueDisplayMode.Vertical);

        /// <summary>
        /// Gets or sets the display mode of the picture previews.
        /// </summary>
        [DataMember]
        public PicturePreviewDisplayMode PicturePreviewDisplayMode
        {
            get { return this.GetValue(PicturePreviewDisplayModeProperty); }
            set { this.SetValue(PicturePreviewDisplayModeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PicturePreviewDisplayMode"/> observable property.
        /// </summary>
        public static ObservableProperty<PicturePreviewDisplayMode> PicturePreviewDisplayModeProperty = new ObservableProperty<PicturePreviewDisplayMode, AccountSettings>(o => o.PicturePreviewDisplayMode, PicturePreviewDisplayMode.Thumbnail);

        /// <summary>
        /// Gets or sets the last selected geographic location.
        /// </summary>
        [DataMember]
        public GeoLocation LastLocation
        {
            get { return this.GetValue(LastLocationProperty); }
            set { this.SetValue(LastLocationProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="LastLocation"/> observable property.
        /// </summary>
        public static ObservableProperty<GeoLocation> LastLocationProperty = new ObservableProperty<GeoLocation, AccountSettings>(o => o.LastLocation);

        /// <summary>
        /// Gets or sets the UI settings for the queued picture details.
        /// </summary>
        [DataMember]
        public PictureDetailsUISettings QueuedPictureDetailsUISettings
        {
            get { return this.GetValue(QueuedPictureDetailsUISettingsProperty); }
            set { this.SetValue(QueuedPictureDetailsUISettingsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="QueuedPictureDetailsUISettings"/> observable property.
        /// </summary>
        public static ObservableProperty<PictureDetailsUISettings> QueuedPictureDetailsUISettingsProperty = new ObservableProperty<PictureDetailsUISettings, AccountSettings>(o => o.QueuedPictureDetailsUISettings);

        /// <summary>
        /// Gets or sets the UI settings for the uploaded picture details.
        /// </summary>
        [DataMember]
        public PictureDetailsUISettings UploadedPictureDetailsUISettings
        {
            get { return this.GetValue(UploadedPictureDetailsUISettingsProperty); }
            set { this.SetValue(UploadedPictureDetailsUISettingsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UploadedPictureDetailsUISettings"/> observable property.
        /// </summary>
        public static ObservableProperty<PictureDetailsUISettings> UploadedPictureDetailsUISettingsProperty = new ObservableProperty<PictureDetailsUISettings, AccountSettings>(o => o.UploadedPictureDetailsUISettings);

        /// <summary>
        /// Gets or sets the UI settings for the picture defaults details.
        /// </summary>
        [DataMember]
        public PictureDetailsUISettings PictureDefaultsDetailsUISettings
        {
            get { return this.GetValue(PictureDefaultsDetailsUISettingsProperty); }
            set { this.SetValue(PictureDefaultsDetailsUISettingsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PictureDefaultsDetailsUISettings"/> observable property.
        /// </summary>
        public static ObservableProperty<PictureDetailsUISettings> PictureDefaultsDetailsUISettingsProperty = new ObservableProperty<PictureDetailsUISettings, AccountSettings>(o => o.PictureDefaultsDetailsUISettings);

        /// <summary>
        /// Gets or sets the type of map to use for geographic locations.
        /// </summary>
        [DataMember]
        public GeoLocationMapProvider GeoLocationMapProvider
        {
            get { return this.GetValue(GeoLocationMapProviderProperty); }
            set { this.SetValue(GeoLocationMapProviderProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="GeoLocationMapProvider"/> observable property.
        /// </summary>
        public static ObservableProperty<GeoLocationMapProvider> GeoLocationMapProviderProperty = new ObservableProperty<GeoLocationMapProvider, AccountSettings>(o => o.GeoLocationMapProvider);

        /// <summary>
        /// The number of attemp to upload a file.
        /// </summary>
        [DataMember]
        public short UploadRetryAttempts
        {
            get { return this.GetValue(UploadRetryAttemptsProperty); }
            set { this.SetValue(UploadRetryAttemptsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UploadRetryAttempts"/> observable property.
        /// </summary>
        public static ObservableProperty<short> UploadRetryAttemptsProperty = new ObservableProperty<short, AccountSettings>(o => o.UploadRetryAttempts, 2);

            #endregion

        #region Obsolete Properties

#pragma warning disable 618 // Disable warning about obsolete members

        /// <summary>
        /// Gets or sets a value that determines if picture metadata should be retrieved for new pictures.
        /// </summary>
        [DataMember]
        [Obsolete("This property is obsolete and has been replaced by a plugin.")]
        public bool RetrieveMetadata
        {
            get { return this.GetValue(RetrieveMetadataProperty); }
            set { this.SetValue(RetrieveMetadataProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="RetrieveMetadata"/> observable property.
        /// </summary>
        [Obsolete("This property is obsolete and has been replaced by a plugin.")]
        public static ObservableProperty<bool> RetrieveMetadataProperty = new ObservableProperty<bool, AccountSettings>(o => o.RetrieveMetadata);

        /// <summary>
        /// Gets or sets a value that determines if pictures should be deleted after they have been uploaded.
        /// </summary>
        [DataMember]
        [Obsolete("This property is obsolete and has been replaced by a plugin.")]
        public bool DeleteFileAfterUpload
        {
            get { return this.GetValue(DeleteFileAfterUploadProperty); }
            set { this.SetValue(DeleteFileAfterUploadProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="DeleteFileAfterUpload"/> observable property.
        /// </summary>
        [Obsolete("This property is obsolete and has been replaced by a plugin.")]
        public static ObservableProperty<bool> DeleteFileAfterUploadProperty = new ObservableProperty<bool, AccountSettings>(o => o.DeleteFileAfterUpload);

        /// <summary>
        /// Gets or sets the folders to monitor for new pictures.
        /// </summary>
        [DataMember]
        [Obsolete("This property is obsolete and has been replaced by a plugin.")]
        public ObservableCollection<string> FoldersToMonitor
        {
            get { return this.GetValue(FoldersToMonitorProperty); }
            set { this.SetValue(FoldersToMonitorProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="FoldersToMonitor"/> observable property.
        /// </summary>
        [Obsolete("This property is obsolete and has been replaced by a plugin.")]
        public static ObservableProperty<ObservableCollection<string>> FoldersToMonitorProperty = new ObservableProperty<ObservableCollection<string>, AccountSettings>(o => o.FoldersToMonitor);

#pragma warning restore 618 // Restore warning about obsolete members

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountSettings"/> class.
        /// </summary>
        public AccountSettings()
        {
            this.PictureDefaults = new Picture();
#pragma warning disable 618 // Disable warning about obsolete members
            this.FoldersToMonitor = new ObservableCollection<string>();
#pragma warning restore 618 // Restore warning about obsolete members
            this.QueuedPictureDetailsUISettings = new PictureDetailsUISettings();
            this.UploadedPictureDetailsUISettings = new PictureDetailsUISettings();
            this.PictureDefaultsDetailsUISettings = new PictureDetailsUISettings();
        }

        #endregion
    }
}