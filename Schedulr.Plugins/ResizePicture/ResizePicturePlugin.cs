using System.Diagnostics;
using JelleDruyts.Windows.Media;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.ResizePicture
{
    [Plugin("Resize Picture", "Resize (for pictures only)", "Resizes a picture (not video).", InstantiationPolicy = PluginInstantiationPolicy.MultipleInstancesPerScope)]
    [SupportedRendering(RenderingType.Picture)]
    public class ResizePicturePlugin : PictureRenderingPlugin<ResizePicturePluginSettings, ResizePicturePluginSettingsControl>
    {
        protected override ResizePicturePluginSettingsControl GetSettingsControl(ResizePicturePluginSettings settings)
        {
            return new ResizePicturePluginSettingsControl(settings);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        protected override IImageManipulation GetManipulation(RenderingEventArgs args)
        {
            this.Host.Logger.Log(string.Format("Resizing picture \"{0}\" to {1} pixels on the longest side", args.Picture.FileName, this.Settings.LongestSide), TraceEventType.Information);
            return new ResizeManipulation(this.Settings.LongestSide) { AllowedDirections = AllowedResizeDirections.Down };
        }
    }
}