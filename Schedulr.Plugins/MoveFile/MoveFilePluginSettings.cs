using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.MoveFile
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class MoveFilePluginSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the folder to move the file to.
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
        public static ObservableProperty<string> DestinationFolderProperty = new ObservableProperty<string, MoveFilePluginSettings>(o => o.DestinationFolder);

        #endregion
    }
}