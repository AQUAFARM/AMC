using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Adds a border around an image.
    /// </summary>
    public class BorderManipulation : ImageManipulation
    {
        #region Constants

        private static readonly Brush DefaultBorderBrush = Brushes.White;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the border thickness.
        /// </summary>
        public Thickness BorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the border brush.
        /// </summary>
        public Brush BorderBrush { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderManipulation"/> class.
        /// </summary>
        /// <param name="borderThickness">The border thickness.</param>
        public BorderManipulation(Thickness borderThickness)
        {
            this.BorderThickness = borderThickness;
            this.BorderBrush = DefaultBorderBrush;
        }

        #endregion

        #region ShouldApply

        /// <summary>
        /// Determines if the manipulation should be applied.
        /// </summary>
        /// <returns><see langword="true"/> if the manipulation should be applied, <see langword="false"/> otherwise.</returns>
        public override bool ShouldApply()
        {
            return (this.BorderThickness.Bottom > 0 || this.BorderThickness.Left > 0 || this.BorderThickness.Right > 0 || this.BorderThickness.Top > 0);
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
            // First fill the current image entirely with the border brush.
            var finalSize = new Rect(0, 0, currentSize.Width + this.BorderThickness.Left + this.BorderThickness.Right, currentSize.Height + this.BorderThickness.Top + this.BorderThickness.Bottom);
            context.DrawRectangle(this.BorderBrush, null, finalSize);

            // Then draw the original image.
            var group = new DrawingGroup();
            var rect = new Rect(this.BorderThickness.Left, this.BorderThickness.Top, currentSize.Width, currentSize.Height);
            group.Children.Add(new ImageDrawing(image, rect));
            context.DrawDrawing(group);

            // And return the modified size.
            return finalSize;
        }

        #endregion
    }
}