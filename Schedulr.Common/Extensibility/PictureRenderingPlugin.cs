using System.IO;
using JelleDruyts.Windows.Media;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides a convenient base class for event plugins that manipulate a picture (not video) before it is uploaded.
    /// </summary>
    public abstract class PictureRenderingPlugin : RenderingPlugin
    {
        #region Fields

        private Stream lastRenderedStreamToDispose;

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Called when a picture or video needs to be rendered before it is uploaded.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        /// <param name="fileToRender">The current stream that contains the file to render.</param>
        /// <returns>The stream that contains the modified file.</returns>
        public override Stream OnRenderingFile(RenderingEventArgs args, Stream fileToRender)
        {
            if (args.IsVideo)
            {
                return fileToRender;
            }
            else
            {
                var manipulation = GetManipulation(args);
                if (manipulation == null)
                {
                    return fileToRender;
                }
                else
                {
                    using (manipulation)
                    {
                        this.lastRenderedStreamToDispose = ImageManipulator.ManipulateImage(fileToRender, manipulation);
                        return this.lastRenderedStreamToDispose;
                    }
                }
            }
        }

        /// <summary>
        /// Called when a file has been completely rendered. Use this to perform cleanup if needed.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public override void OnRenderingFileCompleted(RenderingEventArgs args)
        {
            if (this.lastRenderedStreamToDispose != null)
            {
                this.lastRenderedStreamToDispose.Dispose();
                this.lastRenderedStreamToDispose = null;
            }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Gets the manipulation to apply to the image.
        /// </summary>
        /// <param name="args">The <see cref="Schedulr.Extensibility.RenderingEventArgs"/> instance containing the event data.</param>
        /// <returns>The image manipulation to apply.</returns>
        protected abstract IImageManipulation GetManipulation(RenderingEventArgs args);

        #endregion
    }

    /// <summary>
    /// Provides a convenient base class for event plugins that manipulate a picture (not video) before it is uploaded.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings for this plugin.</typeparam>
    /// <typeparam name="TSettingsControl">The type of the control that is used to edit the settings for this plugin.</typeparam>
    public abstract class PictureRenderingPlugin<TSettings, TSettingsControl> : RenderingPlugin<TSettings, TSettingsControl> where TSettings : class, new()
    {
        #region Fields

        private Stream lastRenderedStreamToDispose;

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Called when a picture or video needs to be rendered before it is uploaded.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        /// <param name="fileToRender">The current stream that contains the file to render.</param>
        /// <returns>The stream that contains the modified file.</returns>
        public override Stream OnRenderingFile(RenderingEventArgs args, Stream fileToRender)
        {
            if (args.IsVideo)
            {
                return fileToRender;
            }
            else
            {
                var manipulation = GetManipulation(args);
                if (manipulation == null)
                {
                    return fileToRender;
                }
                else
                {
                    using (manipulation)
                    {
                        this.lastRenderedStreamToDispose = ImageManipulator.ManipulateImage(fileToRender, manipulation);
                        return this.lastRenderedStreamToDispose;
                    }
                }
            }
        }

        /// <summary>
        /// Called when a file has been completely rendered. Use this to perform cleanup if needed.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public override void OnRenderingFileCompleted(RenderingEventArgs args)
        {
            if (this.lastRenderedStreamToDispose != null)
            {
                this.lastRenderedStreamToDispose.Dispose();
                this.lastRenderedStreamToDispose = null;
            }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Gets the manipulation to apply to the image.
        /// </summary>
        /// <param name="args">The <see cref="Schedulr.Extensibility.RenderingEventArgs"/> instance containing the event data.</param>
        /// <returns>The image manipulation to apply.</returns>
        protected abstract IImageManipulation GetManipulation(RenderingEventArgs args);

        #endregion
    }
}