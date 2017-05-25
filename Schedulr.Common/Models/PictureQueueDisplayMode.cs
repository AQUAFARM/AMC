
namespace Schedulr.Models
{
    /// <summary>
    /// Determines how pictures in the queue are displayed.
    /// </summary>
    public enum PictureQueueDisplayMode
    {
        /// <summary>
        /// Display one batch vertically at a time.
        /// </summary>
        Vertical = 0,

        /// <summary>
        /// Display batches horizontally and let them flow to the next row if no space is available.
        /// </summary>
        HorizontalFlow = 1
    }
}