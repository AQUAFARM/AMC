using System.Windows;
using System.Windows.Media;

namespace JelleDruyts.Windows.DragDrop
{
    /// <summary>
    /// An drag/drop adorner that shows a highlight.
    /// </summary>
    public class DropTargetHighlightAdorner : DropTargetAdorner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropTargetHighlightAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The adorned element.</param>
        public DropTargetHighlightAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (DropInfo.VisualTargetItem != null)
            {
                Rect rect = new Rect(
                    DropInfo.VisualTargetItem.TranslatePoint(new Point(), AdornedElement),
                    VisualTreeHelper.GetDescendantBounds(DropInfo.VisualTargetItem).Size);
                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Gray, 2), rect, 2, 2);
            }
        }
    }
}
