using Schedulr.Models;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides data for picture and video rendering handled by plugins.
    /// </summary>
    public class RenderingEventArgs : PluginEventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the picture being rendered.
        /// </summary>
        public Picture Picture { get; private set; }

        /// <summary>
        /// Gets a value that determines if the application is treating the file as a video.
        /// </summary>
        public bool IsVideo { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureEventArgs"/> class.
        /// </summary>
        /// <param name="applicationInfo">The information about the application.</param>
        /// <param name="picture">The picture being rendered.</param>
        /// <param name="isVideo">Determines if the application is treating the file as a video.</param>
        public RenderingEventArgs(ApplicationInfo applicationInfo, Picture picture, bool isVideo)
            : base(applicationInfo)
        {
            this.Picture = picture;
            this.IsVideo = isVideo;
        }

        #endregion
    }
}