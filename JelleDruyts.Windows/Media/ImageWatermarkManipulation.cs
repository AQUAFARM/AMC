using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Adds an image-based watermark to an image.
    /// </summary>
    public class ImageWatermarkManipulation : WatermarkManipulation
    {
        #region Properties

        /// <summary>
        /// Gets or sets the image to use as the watermark.
        /// </summary>
        public Stream Image { get; set; }

        /// <summary>
        /// Gets a value that determines if the image should be disposed when this instance is disposed.
        /// </summary>
        private bool DisposeImage { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageWatermarkManipulation"/> class.
        /// </summary>
        /// <param name="imageFileName">The path to the image file to use as the watermark.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public ImageWatermarkManipulation(string imageFileName)
            : this(File.OpenRead(imageFileName), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageWatermarkManipulation"/> class.
        /// </summary>
        /// <param name="image">The image to use as the watermark.</param>
        public ImageWatermarkManipulation(Stream image)
            : this(image, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageWatermarkManipulation"/> class.
        /// </summary>
        /// <param name="image">The image to use as the watermark.</param>
        /// <param name="disposeImage">Determines if the image should be disposed when this instance is disposed.</param>
        public ImageWatermarkManipulation(Stream image, bool disposeImage)
        {
            this.Image = image;
            this.DisposeImage = disposeImage;
        }

        #endregion

        #region ShouldApply

        /// <summary>
        /// Determines if the manipulation should be applied.
        /// </summary>
        /// <returns><see langword="true"/> if the manipulation should be applied, <see langword="false"/> otherwise.</returns>
        public override bool ShouldApply()
        {
            return this.Image != null;
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
            var frame = BitmapFrame.Create(this.Image);
            var origin = GetPlacementOrigin(frame.PixelWidth, frame.PixelHeight, currentSize.Width, currentSize.Height);
            var rect = new Rect(origin.X, origin.Y, frame.PixelWidth, frame.PixelHeight);
            context.DrawImage(frame, rect);
            return currentSize;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.DisposeImage && this.Image != null)
                {
                    this.Image.Dispose();
                    this.Image = null;
                }
            }
        }

        #endregion
    }
}