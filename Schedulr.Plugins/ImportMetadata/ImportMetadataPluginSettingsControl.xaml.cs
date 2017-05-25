using System.IO;
using System.Windows;
using System.Windows.Controls;
using Schedulr.Extensibility;
using Schedulr.Models;
using Schedulr.Providers;

namespace Schedulr.Plugins.ImportMetadata
{
    /// <summary>
    /// Interaction logic for ImportMetadataPluginSettingsControl.xaml
    /// </summary>
    public partial class ImportMetadataPluginSettingsControl : UserControl
    {
        private ImportMetadataPluginSettings settings;
        private IPluginHost host;

        public ImportMetadataPluginSettingsControl(ImportMetadataPluginSettings settings, IPluginHost host)
        {
            InitializeComponent();
            this.settings = settings;
            this.host = host;
            this.DataContext = this.settings;
        }

        private void editCustomPictureDescriptionButton_Click(object sender, RoutedEventArgs e)
        {
            PictureMetadata sampleMetadata;
            // Use actual metadata for the current sample picture if available.
            var samplePicture = TemplateTokenProvider.GetSampleValue<Picture>();
            if (samplePicture != null && File.Exists(samplePicture.FileName))
            {
                sampleMetadata = PictureMetadataProvider.RetrieveMetadataFromFile(samplePicture.FileName, null);
            }
            else
            {
                sampleMetadata = new PictureMetadata();
            }
            var additionalTokens = TemplateTokenProvider.ProvideTemplateTokens(sampleMetadata);
            this.settings.CustomPictureDescription = this.host.ShowTemplateEditorDialog(this.settings.CustomPictureDescription, "Edit the custom picture's description", additionalTokens);
        }
    }
}