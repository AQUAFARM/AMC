using System.Diagnostics;
using System.Globalization;
using System.IO;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.MoveFile
{
    [Plugin("Move Picture Or Video", "Move the picture or video", "Moves the picture or video file on disk.", InstantiationPolicy = PluginInstantiationPolicy.SingleInstancePerScope)]
    [SupportedPictureEvent(PictureEventType.Uploaded)]
    [SupportedPictureEvent(PictureEventType.RemovedFromQueue)]
    [SupportedPictureEvent(PictureEventType.RemovedFromUploads)]
    public class MoveFilePlugin : EventPlugin<MoveFilePluginSettings, MoveFilePluginSettingsControl>
    {
        protected override MoveFilePluginSettingsControl GetSettingsControl(MoveFilePluginSettings settings)
        {
            return new MoveFilePluginSettingsControl(settings);
        }

        public override void OnPictureEvent(PictureEventArgs args)
        {
            // Do not move the file for failed uploads.
            if (args.Event == PictureEventType.Uploaded && args.UploadResult.Status != PictureUploadStatus.Succeeded)
            {
                return;
            }

            var destinationFileName = Path.Combine(this.Settings.DestinationFolder, Path.GetFileName(args.Picture.FileName));
            File.Move(args.Picture.FileName, destinationFileName);
            this.Host.Logger.Log(string.Format(CultureInfo.CurrentCulture, "Moved file to \"{0}\" according to settings: \"{1}\"", this.Settings.DestinationFolder, args.Picture.FileName), TraceEventType.Information);
        }
    }
}