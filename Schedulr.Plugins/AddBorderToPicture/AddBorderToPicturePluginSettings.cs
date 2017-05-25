using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.AddBorderToPicture
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class AddBorderToPicturePluginSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the border margin from the left side.
        /// </summary>
        [DataMember]
        [DefaultValue(5)]
        public int BorderMarginLeft
        {
            get { return this.GetValue(BorderMarginLeftProperty); }
            set { this.SetValue(BorderMarginLeftProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="BorderMarginLeft"/> observable property.
        /// </summary>
        public static ObservableProperty<int> BorderMarginLeftProperty = new ObservableProperty<int, AddBorderToPicturePluginSettings>(o => o.BorderMarginLeft, 5);

        /// <summary>
        /// Gets or sets the border margin from the top.
        /// </summary>
        [DataMember]
        [DefaultValue(5)]
        public int BorderMarginTop
        {
            get { return this.GetValue(BorderMarginTopProperty); }
            set { this.SetValue(BorderMarginTopProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="BorderMarginTop"/> observable property.
        /// </summary>
        public static ObservableProperty<int> BorderMarginTopProperty = new ObservableProperty<int, AddBorderToPicturePluginSettings>(o => o.BorderMarginTop, 5);

        /// <summary>
        /// Gets or sets the border margin from the right.
        /// </summary>
        [DataMember]
        [DefaultValue(5)]
        public int BorderMarginRight
        {
            get { return this.GetValue(BorderMarginRightProperty); }
            set { this.SetValue(BorderMarginRightProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="BorderMarginRight"/> observable property.
        /// </summary>
        public static ObservableProperty<int> BorderMarginRightProperty = new ObservableProperty<int, AddBorderToPicturePluginSettings>(o => o.BorderMarginRight, 5);

        /// <summary>
        /// Gets or sets the border margin from the bottom.
        /// </summary>
        [DataMember]
        [DefaultValue(5)]
        public int BorderMarginBottom
        {
            get { return this.GetValue(BorderMarginBottomProperty); }
            set { this.SetValue(BorderMarginBottomProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="BorderMarginBottom"/> observable property.
        /// </summary>
        public static ObservableProperty<int> BorderMarginBottomProperty = new ObservableProperty<int, AddBorderToPicturePluginSettings>(o => o.BorderMarginBottom, 5);

        /// <summary>
        /// Gets or sets the border color.
        /// </summary>
        [DataMember]
        public Color BorderColor
        {
            get { return this.GetValue(BorderColorProperty); }
            set { this.SetValue(BorderColorProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="BorderColor"/> observable property.
        /// </summary>
        public static ObservableProperty<Color> BorderColorProperty = new ObservableProperty<Color, AddBorderToPicturePluginSettings>(o => o.BorderColor, Color.White);

        #endregion
    }
}