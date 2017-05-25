
namespace Schedulr.Messages
{
    /// <summary>
    /// Defines the possible reasons the picture queue changed.
    /// </summary>
    public enum PictureQueueChangedReason
    {
        /// <summary>
        /// Indicates that the picture queue changed because pictures were moved.
        /// </summary>
        PicturesMoved,

        /// <summary>
        /// Indicates that the picture queue changed because pictures were added.
        /// </summary>
        PicturesAdded,

        /// <summary>
        /// Indicates that the picture queue changed because pictures were removed.
        /// </summary>
        PicturesRemoved,

        /// <summary>
        /// Indicates that the picture queue changed because pictures were shuffled.
        /// </summary>
        PicturesShuffled,

        /// <summary>
        /// Indicates that the picture queue changed because pictures were uploaded.
        /// </summary>
        PicturesUploaded
    }
}