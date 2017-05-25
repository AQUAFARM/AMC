using System.ComponentModel;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.SaveCopy
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class SaveCopyPluginSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the folder to store the copy of the file in.
        /// </summary>
        [DataMember]
        public string DestinationFolder
        {
            get { return this.GetValue(DestinationFolderProperty); }
            set { this.SetValue(DestinationFolderProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="DestinationFolder"/> observable property.
        /// </summary>
        public static ObservableProperty<string> DestinationFolderProperty = new ObservableProperty<string, SaveCopyPluginSettings>(o => o.DestinationFolder);

        /// <summary>
        /// Gets or sets a value that determines if the program should be run on pictures.
        /// </summary>
        [DataMember]
        [DefaultValue(true)]
        public bool RunOnPictures
        {
            get { return this.GetValue(RunOnPicturesProperty); }
            set { this.SetValue(RunOnPicturesProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="RunOnPictures"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> RunOnPicturesProperty = new ObservableProperty<bool, SaveCopyPluginSettings>(o => o.RunOnPictures, true);

        /// <summary>
        /// Gets or sets a value that determines if the program should be run on videos.
        /// </summary>
        [DataMember]
        [DefaultValue(true)]
        public bool RunOnVideos
        {
            get { return this.GetValue(RunOnVideosProperty); }
            set { this.SetValue(RunOnVideosProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="RunOnVideos"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> RunOnVideosProperty = new ObservableProperty<bool, SaveCopyPluginSettings>(o => o.RunOnVideos, true);

        /// <summary>
        /// Gets or sets the file extension to use for pictures.
        /// </summary>
        [DataMember]
        public string PictureFileExtension
        {
            get { return this.GetValue(PictureFileExtensionProperty); }
            set { this.SetValue(PictureFileExtensionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PictureFileExtension"/> observable property.
        /// </summary>
        public static ObservableProperty<string> PictureFileExtensionProperty = new ObservableProperty<string, SaveCopyPluginSettings>(o => o.PictureFileExtension);

        /// <summary>
        /// Gets or sets the file extension to use for videos.
        /// </summary>
        [DataMember]
        public string VideoFileExtension
        {
            get { return this.GetValue(VideoFileExtensionProperty); }
            set { this.SetValue(VideoFileExtensionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="VideoFileExtension"/> observable property.
        /// </summary>
        public static ObservableProperty<string> VideoFileExtensionProperty = new ObservableProperty<string, SaveCopyPluginSettings>(o => o.VideoFileExtension);

        #endregion
    }
}