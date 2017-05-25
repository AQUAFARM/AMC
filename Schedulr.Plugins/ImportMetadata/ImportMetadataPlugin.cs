using System.Collections.Generic;
using Schedulr.Extensibility;
using Schedulr.Providers;

namespace Schedulr.Plugins.ImportMetadata
{
    [Plugin("Import Metadata", "Automatically retrieve metadata when pictures are added", "Imports picture metadata (such as the title, description, tags, geographic location) from a picture (not video) when it is added.", InstantiationPolicy = PluginInstantiationPolicy.SingleInstancePerScope)]
    [SupportedPictureEvent(PictureEventType.Adding)]
    public class ImportMetadataPlugin : EventPlugin<ImportMetadataPluginSettings, ImportMetadataPluginSettingsControl>
    {
        protected override ImportMetadataPluginSettingsControl GetSettingsControl(ImportMetadataPluginSettings settings)
        {
            return new ImportMetadataPluginSettingsControl(settings, this.Host);
        }

        public override void OnPictureEvent(PictureEventArgs args)
        {
            if (!args.IsVideo && this.Settings.UseCustomPictureDescription)
            {
                var metadata = PictureMetadataProvider.RetrieveMetadataFromFile(args.Picture.FileName, args.Picture, this.Host.Logger);
                if (metadata != null)
                {
                    var tokenValues = new Dictionary<string, object>(args.TemplateTokenValues);
                    TemplateTokenProvider.ProvideTemplateTokenValues(tokenValues, metadata);
                    args.Picture.Description = this.Host.ProcessTemplate(this.Settings.CustomPictureDescription, tokenValues);
                }
            }
        }
    }
}