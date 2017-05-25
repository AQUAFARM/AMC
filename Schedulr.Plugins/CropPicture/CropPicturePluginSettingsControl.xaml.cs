using System.Windows.Controls;

namespace Schedulr.Plugins.CropPicture
{
    /// <summary>
    /// Interaction logic for CropPicturePluginSettingsControl.xaml
    /// </summary>
    public partial class CropPicturePluginSettingsControl : UserControl
    {
        private CropPicturePluginSettings settings;

        public CropPicturePluginSettingsControl(CropPicturePluginSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
            this.DataContext = this.settings;
        }
    }
}