using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Adds a text-based watermark to an image.
    /// </summary>
    public class TextWatermarkManipulation : WatermarkManipulation
    {
        #region Constants

        private static readonly Typeface DefaultTypeface = new Typeface("Verdana");
        private static readonly int DefaultFontSize = 20;
        private static readonly Brush DefaultBrush = Brushes.Black;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text for the watermark.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// Gets or sets the brush to use to draw the text.
        /// </summary>
        public Brush Brush { get; set; }

        /// <summary>
        /// Gets or sets the font typeface.
        /// </summary>
        public Typeface Typeface { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWatermarkManipulation"/> class.
        /// </summary>
        /// <param name="text">The text for the watermark.</param>
        public TextWatermarkManipulation(string text)
        {
            this.Text = text;
            this.FontSize = DefaultFontSize;
            this.Brush = DefaultBrush;
            this.Typeface = DefaultTypeface;
        }

        #endregion

        #region ShouldApply

        /// <summary>
        /// Determines if the manipulation should be applied.
        /// </summary>
        /// <returns><see langword="true"/> if the manipulation should be applied, <see langword="false"/> otherwise.</returns>
        public override bool ShouldApply()
        {
            return !string.IsNullOrEmpty(this.Text);
        }

        #endregion

        #region Apply

        /// <summary>
        /// Applies the image manipulation.
        /// </summary>
        /// <param name="context">The drawing context.</param>
        /// <param name="image">The image being manipulated.</param>
        /// <param name="currentSize">The current size of the manipulated image.</param>
        /// <returns>The new size of the manipulated image.</returns>
        public override Rect Apply(DrawingContext context, BitmapFrame image, Rect currentSize)
        {
            var culture = CultureInfo.CurrentCulture;
            var flowDirection = (culture.TextInfo.IsRightToLeft ? System.Windows.FlowDirection.RightToLeft : System.Windows.FlowDirection.LeftToRight);
            var formattedText = new FormattedText(this.Text, culture, flowDirection, this.Typeface, this.FontSize, this.Brush);

            var origin = GetPlacementOrigin(formattedText.Width, formattedText.Height, currentSize.Width, currentSize.Height);
            context.DrawText(formattedText, origin);

            return currentSize;
        }

        #endregion
    }
}