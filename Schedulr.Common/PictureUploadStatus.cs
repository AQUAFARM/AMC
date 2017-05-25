
namespace Schedulr
{
    /// <summary>
    /// Defines the reasons why a picture upload failed.
    /// </summary>
    public enum PictureUploadStatus
    {
        /// <summary>
        /// The upload succeeded.
        /// </summary>
        Succeeded,

        /// <summary>
        /// The upload timed out and was cancelled.
        /// </summary>
        Timeout,

        /// <summary>
        /// The upload failed because the file to upload does not exist.
        /// </summary>
        FileDoesNotExist,

        /// <summary>
        /// The upload failed because an exception occurred.
        /// </summary>
        ExceptionOccurred,

        /// <summary>
        /// No picture ID was returned by Flickr to identify the uploaded picture.
        /// </summary>
        NoPictureId
    }
}