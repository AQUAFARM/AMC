using System.ComponentModel;
using System.Drawing;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Adds a watermark to an image.
    /// </summary>
    public abstract class WatermarkManipulation : ImageManipulation
    {
        #region Constants

        private const int DefaultMargin = 5;
        private const ContentAlignment DefaultPosition = ContentAlignment.TopLeft;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the margin to respect (in pixels) from the outside border when placing the watermark.
        /// </summary>
        [DefaultValue(DefaultMargin)]
        public int Margin { get; set; }

        /// <summary>
        /// Gets or sets the position of the watermark.
        /// </summary>
        [DefaultValue(DefaultPosition)]
        public ContentAlignment Position { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WatermarkManipulation"/> class.
        /// </summary>
        public WatermarkManipulation()
        {
            this.Margin = DefaultMargin;
            this.Position = DefaultPosition;
        }

        #endregion

        #region GetPlacementOrigin

        /// <summary>
        /// Gets the placement origin.
        /// </summary>
        /// <param name="itemWidth">The width of the item to place.</param>
        /// <param name="itemHeight">The height of the item to place.</param>
        /// <param name="canvasWidth">The width of the canvas in which to place the item.</param>
        /// <param name="canvasHeight">The height of the canvas in which to place the item.</param>
        protected System.Windows.Point GetPlacementOrigin(double itemWidth, double itemHeight, double canvasWidth, double canvasHeight)
        {
            switch (this.Position)
            {
                case ContentAlignment.BottomCenter:
                    return new System.Windows.Point((canvasWidth - itemWidth) / 2, canvasHeight - itemHeight - this.Margin);
                case ContentAlignment.BottomLeft:
                    return new System.Windows.Point(this.Margin, canvasHeight - itemHeight - this.Margin);
                case ContentAlignment.BottomRight:
                    return new System.Windows.Point(canvasWidth - itemWidth - this.Margin, canvasHeight - itemHeight - this.Margin);
                case ContentAlignment.MiddleCenter:
                    return new System.Windows.Point((canvasWidth - itemWidth) / 2, (canvasHeight - itemHeight) / 2);
                case ContentAlignment.MiddleLeft:
                    return new System.Windows.Point(this.Margin, (canvasHeight - itemHeight) / 2);
                case ContentAlignment.MiddleRight:
                    return new System.Windows.Point(canvasWidth - itemWidth - this.Margin, (canvasHeight - itemHeight) / 2);
                case ContentAlignment.TopCenter:
                    return new System.Windows.Point((canvasWidth - itemWidth) / 2, this.Margin);
                case ContentAlignment.TopLeft:
                    return new System.Windows.Point(this.Margin, this.Margin);
                case ContentAlignment.TopRight:
                    return new System.Windows.Point(canvasWidth - itemWidth - this.Margin, this.Margin);
                default:
                    return new System.Windows.Point(this.Margin, this.Margin);
            }
        }

        #endregion
    }
}