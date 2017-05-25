using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JelleDruyts.Windows.Media;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.Watermark
{
    /// <summary>
    /// Interaction logic for WatermarkPluginSettingsControl.xaml
    /// </summary>
    public partial class WatermarkPluginSettingsControl : UserControl
    {
        private static IEnumerable<string> systemFonts = Fonts.SystemFontFamilies.Select(f => f.Source).OrderBy(f => f);
        private WatermarkPluginSettings settings;
        private IPluginHost host;

        public WatermarkPluginSettingsControl(WatermarkPluginSettings settings, IPluginHost host)
        {
            InitializeComponent();
            this.settings = settings;
            this.host = host;
            this.fontNameComboBox.ItemsSource = systemFonts;
            this.DataContext = this.settings;
            this.settings.PropertyChanged += new PropertyChangedEventHandler(SettingsPropertyChanged);
            UpdateColorPreview();
        }

        private void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == WatermarkPluginSettings.TextColorProperty.Name || e.PropertyName == WatermarkPluginSettings.TextOpacityProperty.Name)
            {
                UpdateColorPreview();
            }
        }

        private void UpdateColorPreview()
        {
            this.textColorPreviewRectangle.Fill = this.settings.TextColor.ToSolidColorBrush(this.settings.TextOpacity);
        }

        private void textColorButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.Color = this.settings.TextColor;
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.settings.TextColor = dialog.Color;
            }
        }

        private void browseImageWatermarkFileNameButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Please select the watermark image";
            openFileDialog.Filter = "All files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                this.settings.ImageWatermarkFileName = openFileDialog.FileName;
            }
        }

        private void editTextWatermarkTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            this.settings.TextWatermark = this.host.ShowTemplateEditorDialog(this.settings.TextWatermark, "Edit the watermark text");
        }
    }
}