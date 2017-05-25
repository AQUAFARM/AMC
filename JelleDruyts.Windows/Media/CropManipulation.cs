using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Crops an image.
    /// </summary>
    public class CropManipulation : ImageManipulation
    {
        #region Properties

        /// <summary>
        /// Gets or sets the thickness around the border to crop.
        /// </summary>
        public Thickness CropThickness { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CropManipulation"/> class.
        /// </summary>
        /// <param name="cropThickness">The thickness around the border to crop.</param>
        public CropManipulation(Thickness cropThickness)
        {
            this.CropThickness = cropThickness;
        }

        #endregion

        #region ShouldApply

        /// <summary>
        /// Determines if the manipulation should be applied.
        /// </summary>
        /// <returns><see langword="true"/> if the manipulation should be applied, <see langword="false"/> otherwise.</returns>
        public override bool ShouldApply()
        {
            return (this.CropThickness.Bottom > 0 || this.CropThickness.Left > 0 || this.CropThickness.Right > 0 || this.CropThickness.Top > 0);
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
            // Draw the original image with the upper left corner negatively placed at the crop thickness.
            var group = new DrawingGroup();
            var rect = new Rect(-this.CropThickness.Left, -this.CropThickness.Top, currentSize.Width, currentSize.Height);
            group.Children.Add(new ImageDrawing(image, rect));
            context.DrawDrawing(group);

            // And return the modified size.
            return new Rect(0, 0, currentSize.Width - this.CropThickness.Left - this.CropThickness.Right, currentSize.Height - this.CropThickness.Top - this.CropThickness.Bottom);
        }

        #endregion
    }
}