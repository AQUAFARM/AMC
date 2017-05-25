using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Represents a manipulation that can be performed on an image.
    /// </summary>
    public abstract class ImageManipulation : IImageManipulation
    {
        #region Virtual Methods

        /// <summary>
        /// Determines if the manipulation should be applied.
        /// </summary>
        /// <returns><see langword="true"/> if the manipulation should be applied, <see langword="false"/> otherwise.</returns>
        public virtual bool ShouldApply()
        {
            return true;
        }

        /// <summary>
        /// Determines if the manipulation should be applied.
        /// </summary>
        /// <param name="image">The image to process.</param>
        /// <returns><see langword="true"/> if the manipulation should be applied, <see langword="false"/> otherwise.</returns>
        public virtual bool ShouldApply(BitmapFrame image)
        {
            return ShouldApply();
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Applies the image manipulation.
        /// </summary>
        /// <param name="context">The drawing context.</param>
        /// <param name="image">The image being manipulated.</param>
        /// <param name="currentSize">The current size of the manipulated image.</param>
        /// <returns>The new size of the manipulated image.</returns>
        public abstract Rect Apply(DrawingContext context, BitmapFrame image, Rect currentSize);

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="ImageManipulation"/> is reclaimed by garbage collection.
        /// </summary>
        ~ImageManipulation()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion}
    }
}