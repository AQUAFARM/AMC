using System.IO;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Represents a plugin that can render pictures or videos before they are uploaded.
    /// </summary>
    public interface IRenderingPlugin : IPlugin
    {
        /// <summary>
        /// Called when a picture or video needs to be rendered before it is uploaded.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        /// <param name="fileToRender">The current stream that contains the file to render.</param>
        /// <returns>The stream that contains the modified file.</returns>
        Stream OnRenderingFile(RenderingEventArgs args, Stream fileToRender);

        /// <summary>
        /// Called when a file has been completely rendered. Use this to perform cleanup if needed.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        void OnRenderingFileCompleted(RenderingEventArgs args);
    }
}