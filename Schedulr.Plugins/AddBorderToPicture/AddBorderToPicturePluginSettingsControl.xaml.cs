using System.ComponentModel;
using System.Windows.Controls;
using JelleDruyts.Windows.Media;

namespace Schedulr.Plugins.AddBorderToPicture
{
    /// <summary>
    /// Interaction logic for AddBorderToPicturePluginSettingsControl.xaml
    /// </summary>
    public partial class AddBorderToPicturePluginSettingsControl : UserControl
    {
        private AddBorderToPicturePluginSettings settings;

        public AddBorderToPicturePluginSettingsControl(AddBorderToPicturePluginSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
            this.DataContext = this.settings;
            this.settings.PropertyChanged += new PropertyChangedEventHandler(SettingsPropertyChanged);
            UpdateColorPreview();
        }

        private void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == AddBorderToPicturePluginSettings.BorderColorProperty.Name)
            {
                UpdateColorPreview();
            }
        }

        private void UpdateColorPreview()
        {
            this.borderColorPreviewRectangle.Fill = this.settings.BorderColor.ToSolidColorBrush();
        }

        private void borderColorButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.Color = this.settings.BorderColor;
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.settings.BorderColor = dialog.Color;
            }
        }
    }
}