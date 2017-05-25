using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JelleDruyts.Windows.DragDrop
{
    /// <summary>
    /// An drag/drop adorner that shows an insertion marker.
    /// </summary>
    public class DropTargetInsertionAdorner : DropTargetAdorner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropTargetInsertionAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The adorned element.</param>
        public DropTargetInsertionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            ItemsControl itemsControl = DropInfo.VisualTarget as ItemsControl;

            if (itemsControl != null)
            {
                // Get the position of the item at the insertion index. If the insertion point is
                // to be after the last item, then get the position of the last item and add an 
                // offset later to draw it at the end of the list.
                ItemsControl itemParent;

                if (DropInfo.VisualTargetItem != null)
                {
                    itemParent = ItemsControl.ItemsControlFromItemContainer(DropInfo.VisualTargetItem);
                }
                else
                {
                    itemParent = itemsControl;
                }

                bool placeBehind = (DropInfo.InsertIndex == itemParent.Items.Count);
                int index = Math.Min(DropInfo.InsertIndex, itemParent.Items.Count - 1);
                UIElement adornedTargetVisual = (UIElement)itemParent.ItemContainerGenerator.ContainerFromIndex(index);
                if (adornedTargetVisual != null)
                {
                    var adornedTargetItem = itemParent.ItemContainerGenerator.ItemFromContainer(adornedTargetVisual);
                    if (DropInfo.TargetGroup != null)
                    {
                        if (DropInfo.NewGroupRequestedBefore)
                        {
                            // A new group is requested, show the adorner before the group control.
                            adornedTargetVisual = DropInfo.TargetGroupControl;
                            placeBehind = false;
                        }
                        else if (DropInfo.NewGroupRequestedAfter)
                        {
                            // A new group is requested, show the adorner before the group control.
                            adornedTargetVisual = DropInfo.TargetGroupControl;
                            placeBehind = true;
                        }
                        else if (!DropInfo.TargetGroup.Items.Contains(adornedTargetItem))
                        {
                            // Normally, the adorner is placed *before* the *next* item.
                            // But if that next item is in another group, show the adorner
                            // *after* the *previous* item (which is the last item in the group).
                            adornedTargetVisual = (UIElement)itemParent.ItemContainerGenerator.ContainerFromItem(DropInfo.TargetGroup.Items[DropInfo.TargetGroup.ItemCount - 1]);
                            placeBehind = true;
                        }
                    }

                    Rect itemRect = new Rect(adornedTargetVisual.TranslatePoint(new Point(), AdornedElement), adornedTargetVisual.RenderSize);
                    Point point1, point2;
                    double rotation = 0;

                    if (DropInfo.VisualTargetOrientation == Orientation.Vertical)
                    {
                        if (placeBehind)
                        {
                            itemRect.Y += adornedTargetVisual.RenderSize.Height;
                        }

                        point1 = new Point(itemRect.X, itemRect.Y);
                        point2 = new Point(itemRect.Right, itemRect.Y);
                    }
                    else
                    {
                        if (placeBehind)
                        {
                            itemRect.X += adornedTargetVisual.RenderSize.Width;
                        }

                        point1 = new Point(itemRect.X, itemRect.Y);
                        point2 = new Point(itemRect.X, itemRect.Bottom);
                        rotation = 90;
                    }

                    drawingContext.DrawLine(m_Pen, point1, point2);
                    DrawTriangle(drawingContext, point1, rotation);
                    DrawTriangle(drawingContext, point2, 180 + rotation);
                }
            }
        }

        private void DrawTriangle(DrawingContext drawingContext, Point origin, double rotation)
        {
            drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
            drawingContext.PushTransform(new RotateTransform(rotation));

            drawingContext.DrawGeometry(m_Pen.Brush, null, m_Triangle);

            drawingContext.Pop();
            drawingContext.Pop();
        }

        /// <summary>
        /// Initializes the <see cref="DropTargetInsertionAdorner"/> class.
        /// </summary>
        static DropTargetInsertionAdorner()
        {
            // Create the pen and triangle in a static constructor and freeze them to improve performance.
            const int triangleSize = 3;

            m_Pen = new Pen(Brushes.Gray, 2);
            m_Pen.Freeze();

            LineSegment firstLine = new LineSegment(new Point(0, -triangleSize), false);
            firstLine.Freeze();
            LineSegment secondLine = new LineSegment(new Point(0, triangleSize), false);
            secondLine.Freeze();

            PathFigure figure = new PathFigure { StartPoint = new Point(triangleSize, 0) };
            figure.Segments.Add(firstLine);
            figure.Segments.Add(secondLine);
            figure.Freeze();

            m_Triangle = new PathGeometry();
            m_Triangle.Figures.Add(figure);
            m_Triangle.Freeze();
        }

        static Pen m_Pen;
        static PathGeometry m_Triangle;
    }
}
