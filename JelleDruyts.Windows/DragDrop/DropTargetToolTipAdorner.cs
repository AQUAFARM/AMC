using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace JelleDruyts.Windows.DragDrop
{
    /// <summary>
    /// An drag/drop adorner that shows a tooltip.
    /// </summary>
    public class DropTargetToolTipAdorner : DropTargetAdorner
    {
        private static Brush foregroundBrush = new SolidColorBrush(Color.FromRgb(0x40, 0x40, 0x40)); //Brushes.Black;
        private static Brush backgroundBrush = new LinearGradientBrush(Colors.White, Colors.LightGray, 90.0);
        private static Pen borderPen = new Pen(foregroundBrush, 0.5);
        private static Typeface typeface = new Typeface("Verdana");
        private static FormattedText formattedText;

        /// <summary>
        /// Gets or sets the tool tip message.
        /// </summary>
        public static string ToolTipMessage
        {
            get
            {
                return formattedText.Text;
            }
            set
            {
                formattedText = new FormattedText(value, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 10, foregroundBrush);
                formattedText.MaxTextWidth = 200;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropTargetToolTipAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The adorned element.</param>
        public DropTargetToolTipAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (DropInfo.MousePosition.HasValue)
            {
                var topLeft = new Point(DropInfo.MousePosition.Value.X, DropInfo.MousePosition.Value.Y + 30);
                drawingContext.DrawRoundedRectangle(backgroundBrush, borderPen, new Rect(topLeft, new Size(formattedText.Width + 6, formattedText.Height + 6)), 1.5, 1.5);
                drawingContext.DrawText(formattedText, new Point(topLeft.X + 3, topLeft.Y + 3));
            }
        }
    }
}