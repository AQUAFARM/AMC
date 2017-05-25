using System.Windows.Media;
using DrawingColor = System.Drawing.Color;
using MediaColor = System.Windows.Media.Color;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Provides extension methods for collection types.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Converts a <see cref="MediaColor"/> to a <see cref="DrawingColor"/>.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The converted color.</returns>
        public static DrawingColor ToDrawingColor(this MediaColor color)
        {
            return DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts a <see cref="DrawingColor"/> to a <see cref="MediaColor"/>.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The converted color.</returns>
        public static MediaColor ToMediaColor(this DrawingColor color)
        {
            return MediaColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts a <see cref="MediaColor"/> to a <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        /// <returns>A solid color brush of the specified color.</returns>
        public static SolidColorBrush ToSolidColorBrush(this MediaColor color)
        {
            return color.ToSolidColorBrush(1.0);
        }

        /// <summary>
        /// Converts a <see cref="MediaColor"/> to a <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        /// <param name="opacity">The opacity of the brush.</param>
        /// <returns>A solid color brush of the specified color and opacity.</returns>
        public static SolidColorBrush ToSolidColorBrush(this MediaColor color, double opacity)
        {
            return new SolidColorBrush(color) { Opacity = opacity };
        }

        /// <summary>
        /// Converts a <see cref="DrawingColor"/> to a <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        /// <returns>A solid color brush of the specified color.</returns>
        public static SolidColorBrush ToSolidColorBrush(this DrawingColor color)
        {
            return color.ToSolidColorBrush(1.0);
        }

        /// <summary>
        /// Converts a <see cref="DrawingColor"/> to a <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        /// <param name="opacity">The opacity of the brush.</param>
        /// <returns>A solid color brush of the specified color and opacity.</returns>
        public static SolidColorBrush ToSolidColorBrush(this DrawingColor color, double opacity)
        {
            return color.ToMediaColor().ToSolidColorBrush(opacity);
        }
    }
}