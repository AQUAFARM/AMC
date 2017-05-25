using System.Diagnostics;
using System.Globalization;
using System.Windows;
using JelleDruyts.Windows.Media;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.AddBorderToPicture
{
    [Plugin("Add Border To Picture", "Add a border (for pictures only)", "Adds a border to a picture (not video).", InstantiationPolicy = PluginInstantiationPolicy.MultipleInstancesPerScope)]
    [SupportedRendering(RenderingType.Picture)]
    public class AddBorderToPicturePlugin : PictureRenderingPlugin<AddBorderToPicturePluginSettings, AddBorderToPicturePluginSettingsControl>
    {
        protected override AddBorderToPicturePluginSettingsControl GetSettingsControl(AddBorderToPicturePluginSettings settings)
        {
            return new AddBorderToPicturePluginSettingsControl(settings);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        protected override IImageManipulation GetManipulation(RenderingEventArgs args)
        {
            this.Host.Logger.Log(string.Format(CultureInfo.CurrentCulture, "Adding border to picture \"{0}\"", args.Picture.FileName), TraceEventType.Information);
            return new BorderManipulation(new Thickness(this.Settings.BorderMarginLeft, this.Settings.BorderMarginTop, this.Settings.BorderMarginRight, this.Settings.BorderMarginBottom))
            {
                BorderBrush = this.Settings.BorderColor.ToSolidColorBrush()
            };
        }
    }
}