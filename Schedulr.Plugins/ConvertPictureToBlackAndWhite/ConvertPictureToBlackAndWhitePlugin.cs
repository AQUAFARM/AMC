using System.Diagnostics;
using JelleDruyts.Windows.Media;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.ConvertPictureToBlackAndWhite
{
    [Plugin("Convert Picture To Black & White", "Convert to black & white (for pictures only)", "Converts a picture (not video) to black & white.", InstantiationPolicy = PluginInstantiationPolicy.MultipleInstancesPerScope)]
    [SupportedRendering(RenderingType.Picture)]
    public class ConvertPictureToBlackAndWhitePlugin : PictureRenderingPlugin
    {
        protected override IImageManipulation GetManipulation(RenderingEventArgs args)
        {
            this.Host.Logger.Log(string.Format("Converting picture \"{0}\" to black and white", args.Picture.FileName), TraceEventType.Information);
            return new BlackAndWhiteManipulation();
        }
    }
}