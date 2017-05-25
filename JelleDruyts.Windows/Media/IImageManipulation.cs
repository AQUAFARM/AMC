using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Represents a manipulation that can be performed on an image.
    /// </summary>
    public interface IImageManipulation : IDisposable
    {
        /// <summary>
        /// Determines if the manipulation should be applied.
        /// </summary>
        /// <returns><see langword="true"/> if the manipulation should be applied, <see langword="false"/> otherwise.</returns>
        bool ShouldApply();

        /// <summary>
        /// Determines if the manipulation should be applied.
        /// </summary>
        /// <param name="image">The image to process.</param>
        /// <returns><see langword="true"/> if the manipulation should be applied, <see langword="false"/> otherwise.</returns>
        bool ShouldApply(BitmapFrame image);

        /// <summary>
        /// Applies the image manipulation.
        /// </summary>
        /// <param name="context">The drawing context.</param>
        /// <param name="image">The image being manipulated.</param>
        /// <param name="currentSize">The current size of the manipulated image.</param>
        /// <returns>The new size of the manipulated image.</returns>
        Rect Apply(DrawingContext context, BitmapFrame image, Rect currentSize);
    }
}