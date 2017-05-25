using System.Diagnostics;
using System.Globalization;
using System.IO;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.DeleteFile
{
    [Plugin("Delete Picture Or Video", "Delete the picture or video", "Deletes the picture or video file from disk.", InstantiationPolicy = PluginInstantiationPolicy.SingleInstancePerScope)]
    [SupportedPictureEvent(PictureEventType.Uploaded)]
    [SupportedPictureEvent(PictureEventType.RemovedFromQueue)]
    [SupportedPictureEvent(PictureEventType.RemovedFromUploads)]
    public class DeleteFilePlugin : EventPlugin
    {
        public override void OnPictureEvent(PictureEventArgs args)
        {
            // Do not delete the file for failed uploads.
            if (args.Event == PictureEventType.Uploaded && args.UploadResult.Status != PictureUploadStatus.Succeeded)
            {
                return;
            }

            File.Delete(args.Picture.FileName);
            this.Host.Logger.Log(string.Format(CultureInfo.CurrentCulture, "Deleting file according to settings: \"{0}\"", args.Picture.FileName), TraceEventType.Information);
        }
    }
}