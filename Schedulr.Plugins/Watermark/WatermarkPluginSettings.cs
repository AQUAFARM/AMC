using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.Watermark
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class WatermarkPluginSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the type of watermark.
        /// </summary>
        [DataMember]
        public WatermarkType Type
        {
            get { return this.GetValue(TypeProperty); }
            set { this.SetValue(TypeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Type"/> observable property.
        /// </summary>
        public static ObservableProperty<WatermarkType> TypeProperty = new ObservableProperty<WatermarkType, WatermarkPluginSettings>(o => o.Type, WatermarkType.Text);

        /// <summary>
        /// Gets or sets the text to use as the watermark.
        /// </summary>
        [DataMember]
        [DefaultValue("$(PictureTitle)")]
        public string TextWatermark
        {
            get { return this.GetValue(TextWatermarkProperty); }
            set { this.SetValue(TextWatermarkProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="TextWatermark"/> observable property.
        /// </summary>
        public static ObservableProperty<string> TextWatermarkProperty = new ObservableProperty<string, WatermarkPluginSettings>(o => o.TextWatermark, "$(PictureTitle)");

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        [DataMember]
        public Color TextColor
        {
            get { return this.GetValue(TextColorProperty); }
            set { this.SetValue(TextColorProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="TextColor"/> observable property.
        /// </summary>
        public static ObservableProperty<Color> TextColorProperty = new ObservableProperty<Color, WatermarkPluginSettings>(o => o.TextColor, Color.Black);

        /// <summary>
        /// Gets or sets the text opacity.
        /// </summary>
        [DataMember]
        [DefaultValue(1.0)]
        public double TextOpacity
        {
            get { return this.GetValue(TextOpacityProperty); }
            set { this.SetValue(TextOpacityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="TextOpacity"/> observable property.
        /// </summary>
        public static ObservableProperty<double> TextOpacityProperty = new ObservableProperty<double, WatermarkPluginSettings>(o => o.TextOpacity, 1.0);

        /// <summary>
        /// Gets or sets the text size.
        /// </summary>
        [DataMember]
        [DefaultValue(20)]
        public int FontSize
        {
            get { return this.GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="FontSize"/> observable property.
        /// </summary>
        public static ObservableProperty<int> FontSizeProperty = new ObservableProperty<int, WatermarkPluginSettings>(o => o.FontSize, 20);

        /// <summary>
        /// Gets or sets the font name.
        /// </summary>
        [DataMember]
        [DefaultValue("Verdana")]
        public string FontName
        {
            get { return this.GetValue(FontNameProperty); }
            set { this.SetValue(FontNameProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="FontName"/> observable property.
        /// </summary>
        public static ObservableProperty<string> FontNameProperty = new ObservableProperty<string, WatermarkPluginSettings>(o => o.FontName, "Verdana");

        /// <summary>
        /// Gets or sets the file name of the image to use as the watermark.
        /// </summary>
        [DataMember]
        public string ImageWatermarkFileName
        {
            get { return this.GetValue(ImageWatermarkFileNameProperty); }
            set { this.SetValue(ImageWatermarkFileNameProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ImageWatermarkFileName"/> observable property.
        /// </summary>
        public static ObservableProperty<string> ImageWatermarkFileNameProperty = new ObservableProperty<string, WatermarkPluginSettings>(o => o.ImageWatermarkFileName);

        /// <summary>
        /// Gets or sets the margin to respect (in pixels) from the outside border when placing the watermark.
        /// </summary>
        [DataMember]
        [DefaultValue(5)]
        public int Margin
        {
            get { return this.GetValue(MarginProperty); }
            set { this.SetValue(MarginProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Margin"/> observable property.
        /// </summary>
        public static ObservableProperty<int> MarginProperty = new ObservableProperty<int, WatermarkPluginSettings>(o => o.Margin, 5);

        /// <summary>
        /// Gets or sets the position of the watermark.
        /// </summary>
        [DataMember]
        [DefaultValue(ContentAlignment.TopLeft)]
        public ContentAlignment Position
        {
            get { return this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Position"/> observable property.
        /// </summary>
        public static ObservableProperty<ContentAlignment> PositionProperty = new ObservableProperty<ContentAlignment, WatermarkPluginSettings>(o => o.Position, ContentAlignment.TopLeft);

        #endregion
    }
}