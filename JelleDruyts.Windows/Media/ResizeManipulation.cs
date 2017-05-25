using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Resizes an image.
    /// </summary>
    public class ResizeManipulation : ImageManipulation
    {
        #region Constants

        private const BitmapScalingMode DefaultScalingMode = BitmapScalingMode.HighQuality;
        private const AllowedResizeDirections DefaultAllowedDirections = AllowedResizeDirections.All;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the resized image width, or <see langword="null"/> to base it on other properties.
        /// </summary>
        public int? Width { get; private set; }

        /// <summary>
        /// Gets the resized image height, or <see langword="null"/> to base it on other properties.
        /// </summary>
        public int? Height { get; private set; }

        /// <summary>
        /// Gets the percentage to resize the image with, or <see langword="null"/> to base it on other properties.
        /// </summary>
        public double? Percentage { get; private set; }

        /// <summary>
        /// Gets the size of the longest side of the resized image, or <see langword="null"/> to base it on other properties.
        /// </summary>
        public int? LongestSide { get; private set; }

        /// <summary>
        /// Gets or sets the scaling mode to use when resizing.
        /// </summary>
        [DefaultValue(DefaultScalingMode)]
        public BitmapScalingMode ScalingMode { get; set; }

        /// <summary>
        /// Gets or sets the allowed resize directions.
        /// </summary>
        [DefaultValue(DefaultAllowedDirections)]
        public AllowedResizeDirections AllowedDirections { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeManipulation"/> class.
        /// </summary>
        /// <param name="width">The resized image width, or <see langword="null"/> to base it on the <paramref name="height"/>.</param>
        /// <param name="height">The resized image height, or <see langword="null"/> to base it on the <paramref name="width"/>.</param>
        public ResizeManipulation(int? width, int? height)
            : this(width, height, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeManipulation"/> class.
        /// </summary>
        /// <param name="percentage">The percentage to resize the image with.</param>
        public ResizeManipulation(double percentage)
            : this(null, null, percentage, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeManipulation"/> class.
        /// </summary>
        /// <param name="longestSide">The size of the longest side of the resized image.</param>
        public ResizeManipulation(int longestSide)
            : this(null, null, null, longestSide)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeManipulation"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="percentage">The percentage.</param>
        /// <param name="longestSide">The longest side.</param>
        private ResizeManipulation(int? width, int? height, double? percentage, int? longestSide)
        {
            this.Width = width;
            this.Height = height;
            this.Percentage = percentage;
            this.LongestSide = longestSide;
            this.ScalingMode = DefaultScalingMode;
            this.AllowedDirections = DefaultAllowedDirections;
        }

        #endregion

        #region ShouldApply

        /// <summary>
        /// Determines if the manipulation should be applied.
        /// </summary>
        /// <returns><see langword="true"/> if the manipulation should be applied, <see langword="false"/> otherwise.</returns>
        public override bool ShouldApply()
        {
            return !(this.Width == null && this.Height == null && (this.Percentage == null || this.Percentage.Value == 1) && this.LongestSide == null);
        }

        /// <summary>
        /// Determines if the manipulation should be applied.
        /// </summary>
        /// <param name="image">The image to process.</param>
        /// <returns><see langword="true"/> if the manipulation should be applied, <see langword="false"/> otherwise.</returns>
        public override bool ShouldApply(BitmapFrame image)
        {
            if (!ShouldApply())
            {
                return false;
            }
            var rect = GetFinalSize(image);
            return (rect.Width != image.PixelWidth && rect.Height != image.PixelHeight);
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
            // First clear out the current image entirely.
            context.DrawRectangle(Brushes.White, null, currentSize);

            // Then draw the resized image.
            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, this.ScalingMode);
            var finalSize = GetFinalSize(image);
            group.Children.Add(new ImageDrawing(image, finalSize));
            context.DrawDrawing(group);

            // And return the new size.
            return finalSize;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the final size.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <returns></returns>
        private Rect GetFinalSize(BitmapFrame image)
        {
            var canResizeUp = ((this.AllowedDirections & AllowedResizeDirections.Up) == AllowedResizeDirections.Up);
            var canResizeDown = ((this.AllowedDirections & AllowedResizeDirections.Down) == AllowedResizeDirections.Down);
            var finalWidth = image.PixelWidth;
            var finalHeight = image.PixelHeight;
            if (this.Percentage != null)
            {
                var percentage = this.Percentage.Value;
                if ((percentage > 1 && !canResizeUp) || percentage < 1 && !canResizeDown)
                {
                    percentage = 1;
                }
                finalWidth = (int)(image.PixelWidth * percentage);
                finalHeight = (int)(image.PixelHeight * percentage);
            }
            else if (this.LongestSide != null)
            {
                if (image.PixelWidth > image.PixelHeight)
                {
                    // Landscape mode.
                    finalWidth = this.LongestSide.Value;
                    if ((finalWidth > image.PixelWidth && !canResizeUp) || (finalWidth < image.PixelWidth && !canResizeDown))
                    {
                        finalWidth = image.PixelWidth;
                    }
                    finalHeight = (image.PixelHeight * finalWidth) / image.PixelWidth;
                }
                else
                {
                    // Portrait mode.
                    finalHeight = this.LongestSide.Value;
                    if ((finalHeight > image.PixelHeight && !canResizeUp) || (finalHeight < image.PixelHeight && !canResizeDown))
                    {
                        finalHeight = image.PixelHeight;
                    }
                    finalWidth = (image.PixelWidth * finalHeight) / image.PixelHeight;
                }
            }
            else if (this.Width == null && this.Height != null)
            {
                finalHeight = this.Height.Value;
                if ((finalHeight > image.PixelHeight && !canResizeUp) || (finalHeight < image.PixelHeight && !canResizeDown))
                {
                    finalHeight = image.PixelHeight;
                }
                finalWidth = (image.PixelWidth * finalHeight) / image.PixelHeight;
            }
            else if (this.Height == null && this.Width != null)
            {
                finalWidth = this.Width.Value;
                if ((finalWidth > image.PixelWidth && !canResizeUp) || (finalWidth < image.PixelWidth && !canResizeDown))
                {
                    finalWidth = image.PixelWidth;
                }
                finalHeight = (image.PixelHeight * finalWidth) / image.PixelWidth;
            }
            else
            {
                finalHeight = this.Height.Value;
                finalWidth = this.Width.Value;
                if ((finalHeight > image.PixelHeight && !canResizeUp) || (finalHeight < image.PixelHeight && !canResizeDown) || (finalWidth > image.PixelWidth && !canResizeUp) || (finalWidth < image.PixelWidth && !canResizeDown))
                {
                    finalHeight = image.PixelHeight;
                    finalWidth = image.PixelWidth;
                }
            }
            return new Rect(0, 0, finalWidth, finalHeight);
        }

        #endregion
    }
}