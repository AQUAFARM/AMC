using System.ComponentModel;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Defines the available event types for pictures and videos.
    /// </summary>
    public enum PictureEventType
    {
        /// <summary>
        /// Defines the event that is raised when a picture or video is about to be added to the queue.
        /// </summary>
        [Description("A picture or video is about to be added to the queue")]
        Adding,

        /// <summary>
        /// Defines the event that is raised when a picture or video has just been added to the queue.
        /// </summary>
        [Description("A picture or video has just been added to the queue")]
        Added,

        /// <summary>
        /// Defines the event that is raised when a picture or video is about to be uploaded.
        /// </summary>
        [Description("A picture or video is about to be uploaded")]
        Uploading,

        /// <summary>
        /// Defines the event that is raised when a picture or video has just been uploaded.
        /// </summary>
        [Description("A picture or video has just been uploaded")]
        Uploaded,

        /// <summary>
        /// Defines the event that is raised when a picture or video is about to be removed from the queue.
        /// </summary>
        [Description("A picture or video is about to be removed from the queue")]
        RemovingFromQueue,

        /// <summary>
        /// Defines the event that is raised when a picture or video has just been removed from the queue.
        /// </summary>
        [Description("A picture or video has just been removed from the queue")]
        RemovedFromQueue,

        /// <summary>
        /// Defines the event that is raised when a picture or video is about to be removed from the upload history.
        /// </summary>
        [Description("A picture or video is about to be removed from the upload history")]
        RemovingFromUploads,

        /// <summary>
        /// Defines the event that is raised when a picture or video has just been removed from the upload history.
        /// </summary>
        [Description("A picture or video has just been removed from the upload history")]
        RemovedFromUploads
    }
}