using Schedulr.Models;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides data for picture and video events handled by plugins.
    /// </summary>
    public class PictureEventArgs : EventPluginEventArgs<PictureEventType>
    {
        #region Properties

        /// <summary>
        /// Gets the account related to the event.
        /// </summary>
        public Account Account { get; private set; }

        /// <summary>
        /// Gets the batch related to the event.
        /// </summary>
        public Batch Batch { get; private set; }

        /// <summary>
        /// Gets the picture related to the event.
        /// </summary>
        public Picture Picture { get; private set; }

        /// <summary>
        /// Gets a value that determines if the application is treating the file as a video.
        /// </summary>
        public bool IsVideo { get; private set; }

        /// <summary>
        /// When uploaded: contains the upload result.
        /// </summary>
        public PictureUploadResult UploadResult { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureEventArgs"/> class.
        /// </summary>
        /// <param name="event">The plugin event that occurred.</param>
        /// <param name="applicationInfo">The information about the application.</param>
        /// <param name="account">The account related to the event.</param>
        /// <param name="batch">The batch related to the event.</param>
        /// <param name="picture">The picture related to the event.</param>
        /// <param name="isVideo">Determines if the application is treating the file as a video.</param>
        public PictureEventArgs(PictureEventType @event, ApplicationInfo applicationInfo, Account account, Batch batch, Picture picture, bool isVideo)
            : this(@event, applicationInfo, account, batch, picture, isVideo, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureEventArgs"/> class.
        /// </summary>
        /// <param name="event">The plugin event that occurred.</param>
        /// <param name="applicationInfo">The information about the application.</param>
        /// <param name="account">The account related to the event.</param>
        /// <param name="batch">The batch related to the event.</param>
        /// <param name="picture">The picture related to the event.</param>
        /// <param name="isVideo">Determines if the application is treating the file as a video.</param>
        /// <param name="uploadResult">When uploaded: contains the upload result.</param>
        public PictureEventArgs(PictureEventType @event, ApplicationInfo applicationInfo, Account account, Batch batch, Picture picture, bool isVideo, PictureUploadResult uploadResult)
            : base(@event, applicationInfo)
        {
            this.Account = account;
            this.Batch = batch;
            this.Picture = picture;
            this.IsVideo = isVideo;
            this.UploadResult = uploadResult;
        }

        #endregion
    }
}