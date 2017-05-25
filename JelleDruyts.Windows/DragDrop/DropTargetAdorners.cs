using System;

namespace JelleDruyts.Windows.DragDrop
{
    /// <summary>
    /// Provides the available drop target adorner types.
    /// </summary>
    public class DropTargetAdorners
    {
        /// <summary>
        /// Gets the type of the highlight adorner.
        /// </summary>
        public static Type Highlight
        {
            get { return typeof(DropTargetHighlightAdorner); }
        }

        /// <summary>
        /// Gets the type of the insertion adorner.
        /// </summary>
        public static Type Insert
        {
            get { return typeof(DropTargetInsertionAdorner); }
        }

        /// <summary>
        /// Gets the type of the tooltip adorner.
        /// </summary>
        public static Type ToolTip
        {
            get { return typeof(DropTargetToolTipAdorner); }
        }
    }
}