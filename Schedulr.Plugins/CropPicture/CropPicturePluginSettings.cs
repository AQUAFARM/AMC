using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.CropPicture
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class CropPicturePluginSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the crop margin from the left side.
        /// </summary>
        [DataMember]
        public int CropMarginLeft
        {
            get { return this.GetValue(CropMarginLeftProperty); }
            set { this.SetValue(CropMarginLeftProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="CropMarginLeft"/> observable property.
        /// </summary>
        public static ObservableProperty<int> CropMarginLeftProperty = new ObservableProperty<int, CropPicturePluginSettings>(o => o.CropMarginLeft);

        /// <summary>
        /// Gets or sets the crop margin from the top.
        /// </summary>
        [DataMember]
        public int CropMarginTop
        {
            get { return this.GetValue(CropMarginTopProperty); }
            set { this.SetValue(CropMarginTopProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="CropMarginTop"/> observable property.
        /// </summary>
        public static ObservableProperty<int> CropMarginTopProperty = new ObservableProperty<int, CropPicturePluginSettings>(o => o.CropMarginTop);

        /// <summary>
        /// Gets or sets the crop margin from the right.
        /// </summary>
        [DataMember]
        public int CropMarginRight
        {
            get { return this.GetValue(CropMarginRightProperty); }
            set { this.SetValue(CropMarginRightProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="CropMarginRight"/> observable property.
        /// </summary>
        public static ObservableProperty<int> CropMarginRightProperty = new ObservableProperty<int, CropPicturePluginSettings>(o => o.CropMarginRight);

        /// <summary>
        /// Gets or sets the crop margin from the bottom.
        /// </summary>
        [DataMember]
        public int CropMarginBottom
        {
            get { return this.GetValue(CropMarginBottomProperty); }
            set { this.SetValue(CropMarginBottomProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="CropMarginBottom"/> observable property.
        /// </summary>
        public static ObservableProperty<int> CropMarginBottomProperty = new ObservableProperty<int, CropPicturePluginSettings>(o => o.CropMarginBottom);

        #endregion
    }
}