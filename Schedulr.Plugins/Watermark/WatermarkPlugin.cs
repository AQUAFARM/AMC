using System.Diagnostics;
using System.Windows.Media;
using JelleDruyts.Windows.Media;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.Watermark
{
    [Plugin("Watermark Picture", "Add a watermark (for pictures only)", "Adds a watermark to a picture (not video).")]
    [SupportedRendering(RenderingType.Picture)]
    public class WatermarkPlugin : PictureRenderingPlugin<WatermarkPluginSettings, WatermarkPluginSettingsControl>
    {
        protected override WatermarkPluginSettingsControl GetSettingsControl(WatermarkPluginSettings settings)
        {
            return new WatermarkPluginSettingsControl(settings, this.Host);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        protected override IImageManipulation GetManipulation(Schedulr.Extensibility.RenderingEventArgs args)
        {
            this.Host.Logger.Log(string.Format("Adding watermark to picture \"{0}\"", args.Picture.FileName), TraceEventType.Information);
            WatermarkManipulation manipulation = null;
            if (this.Settings.Type == WatermarkType.Text)
            {
                var textWatermark = this.Host.ProcessTemplate(this.Settings.TextWatermark, args.TemplateTokenValues);
                manipulation = new TextWatermarkManipulation(textWatermark)
                {
                    Brush = this.Settings.TextColor.ToSolidColorBrush(this.Settings.TextOpacity),
                    FontSize = this.Settings.FontSize,
                    Typeface = new Typeface(this.Settings.FontName)
                };
            }
            else
            {
                if (!string.IsNullOrEmpty(this.Settings.ImageWatermarkFileName))
                {
                    manipulation = new ImageWatermarkManipulation(this.Settings.ImageWatermarkFileName);
                }
            }
            if (manipulation != null)
            {
                manipulation.Position = this.Settings.Position;
                manipulation.Margin = this.Settings.Margin;
            }
            return manipulation;
        }
    }
}