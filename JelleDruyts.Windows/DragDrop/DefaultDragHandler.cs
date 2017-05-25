using System.Linq;
using System.Windows;

namespace JelleDruyts.Windows.DragDrop
{
    /// <summary>
    /// The default drag handler.
    /// </summary>
    public class DefaultDragHandler : IDragSource
    {
        /// <summary>
        /// Queries whether a drag can be started.
        /// </summary>
        /// <param name="dragInfo">Information about the drag.</param>
        /// <remarks>
        /// To allow a drag to be started, the <see cref="DragInfo.Effects"/> property on <paramref name="dragInfo"/>
        /// should be set to a value other than <see cref="System.Windows.DragDropEffects.None"/>.
        /// </remarks>
        public virtual void StartDrag(DragInfo dragInfo)
        {
            int itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 1)
            {
                dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();
            }
            else if (itemCount > 1)
            {
                dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(dragInfo.SourceItems);
            }

            dragInfo.Effects = (dragInfo.Data != null) ?
                DragDropEffects.Copy | DragDropEffects.Move :
                DragDropEffects.None;
        }
    }
}
