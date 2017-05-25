using System.Diagnostics;
using System.Windows;
using JelleDruyts.Windows.Media;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.CropPicture
{
    [Plugin("Crop Picture", "Crop (for pictures only)", "Crops a picture (not video).", InstantiationPolicy = PluginInstantiationPolicy.MultipleInstancesPerScope)]
    [SupportedRendering(RenderingType.Picture)]
    public class CropPicturePlugin : PictureRenderingPlugin<CropPicturePluginSettings, CropPicturePluginSettingsControl>
    {
        protected override CropPicturePluginSettingsControl GetSettingsControl(CropPicturePluginSettings settings)
        {
            return new CropPicturePluginSettingsControl(settings);
        }

        protected override IImageManipulation GetManipulation(RenderingEventArgs args)
        {
            this.Host.Logger.Log(string.Format("Cropping picture \"{0}\"", args.Picture.FileName), TraceEventType.Information);
            return new CropManipulation(new Thickness(this.Settings.CropMarginLeft, this.Settings.CropMarginTop, this.Settings.CropMarginRight, this.Settings.CropMarginBottom));
        }
    }
}