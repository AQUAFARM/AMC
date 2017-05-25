using System.Diagnostics;
using System.Globalization;
using System.IO;
using JelleDruyts.Windows;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.SaveCopy
{
    [Plugin("Save Copy Of Picture Or Video", "Save a copy of the file", "Saves a copy of the picture or video. Make sure to add this to the bottom of the list to include all modifications.", InstantiationPolicy = PluginInstantiationPolicy.MultipleInstancesPerScope)]
    [SupportedRendering(RenderingType.Picture)]
    [SupportedRendering(RenderingType.Video)]
    public class SaveCopyPlugin : RenderingPlugin<SaveCopyPluginSettings, SaveCopyPluginSettingsControl>
    {
        #region Fields

        private Stream lastRenderedStreamToDispose;

        #endregion

        protected override SaveCopyPluginSettingsControl GetSettingsControl(SaveCopyPluginSettings settings)
        {
            return new SaveCopyPluginSettingsControl(settings);
        }

        public override Stream OnRenderingFile(RenderingEventArgs args, Stream fileToRender)
        {
            var renderedFile = fileToRender;
            if ((args.IsVideo && this.Settings.RunOnVideos) || (!args.IsVideo && this.Settings.RunOnPictures))
            {
                if (!string.IsNullOrEmpty(this.Settings.DestinationFolder))
                {
                    if (!Directory.Exists(this.Settings.DestinationFolder))
                    {
                        Directory.CreateDirectory(this.Settings.DestinationFolder);
                    }
                    var fileName = Path.Combine(this.Settings.DestinationFolder, Path.GetFileName(args.Picture.FileName));
                    if (args.IsVideo && !string.IsNullOrEmpty(this.Settings.VideoFileExtension))
                    {
                        fileName = Path.ChangeExtension(fileName, FileSystem.EnsureValidFileExtension(this.Settings.VideoFileExtension));
                    }
                    else if (!args.IsVideo && !string.IsNullOrEmpty(this.Settings.PictureFileExtension))
                    {
                        fileName = Path.ChangeExtension(fileName, FileSystem.EnsureValidFileExtension(this.Settings.PictureFileExtension));
                    }
                    this.Host.Logger.Log(string.Format(CultureInfo.CurrentCulture, "Creating a copy of the rendered file file at \"{0}\"", fileName), TraceEventType.Information);
                    fileToRender.CopyTo(fileName);
                    this.lastRenderedStreamToDispose = File.OpenRead(fileName);
                    renderedFile = this.lastRenderedStreamToDispose;
                }
            }

            return renderedFile;
        }

        public override void OnRenderingFileCompleted(RenderingEventArgs args)
        {
            if (this.lastRenderedStreamToDispose != null)
            {
                this.lastRenderedStreamToDispose.Dispose();
                this.lastRenderedStreamToDispose = null;
            }
        }
    }
}