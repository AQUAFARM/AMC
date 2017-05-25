using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Converts an image to grayscale (black and white).
    /// </summary>
    public class BlackAndWhiteManipulation : ImageManipulation
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlackAndWhiteManipulation"/> class.
        /// </summary>
        public BlackAndWhiteManipulation()
        {
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
            // Draw the original image as grayscale.
            var group = new DrawingGroup();
            group.Children.Add(new ImageDrawing(new FormatConvertedBitmap(image, PixelFormats.Gray32Float, null, 0), currentSize));
            context.DrawDrawing(group);

            // Return the original size.
            return currentSize;
        }

        #endregion
    }
}