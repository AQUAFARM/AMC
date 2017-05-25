
namespace JelleDruyts.Windows.DragDrop
{
    /// <summary>
    /// Interface implemented by Drag Handlers.
    /// </summary>
    public interface IDragSource
    {
        /// <summary>
        /// Queries whether a drag can be started.
        /// </summary>
        /// 
        /// <param name="dragInfo">
        /// Information about the drag.
        /// </param>
        /// 
        /// <remarks>
        /// To allow a drag to be started, the <see cref="DragInfo.Effects"/> property on <paramref name="dragInfo"/> 
        /// should be set to a value other than <see cref="System.Windows.DragDropEffects.None"/>. 
        /// </remarks>
        void StartDrag(DragInfo dragInfo);
    }
}
