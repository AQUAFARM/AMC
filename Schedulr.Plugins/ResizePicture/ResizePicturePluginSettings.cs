using System.ComponentModel;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.ResizePicture
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class ResizePicturePluginSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the length in pixels to resize the longest side to.
        /// </summary>
        [DataMember]
        [DefaultValue(1280)]
        public int LongestSide
        {
            get { return this.GetValue(LongestSideProperty); }
            set { this.SetValue(LongestSideProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="LongestSide"/> observable property.
        /// </summary>
        public static ObservableProperty<int> LongestSideProperty = new ObservableProperty<int, ResizePicturePluginSettings>(o => o.LongestSide, 1280);

        #endregion
    }
}