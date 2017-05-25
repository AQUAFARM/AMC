using System.Windows.Controls;

namespace Schedulr.Plugins.DetermineCollections
{
    /// <summary>
    /// Interaction logic for DetermineCollectionsPluginSettingsControl.xaml
    /// </summary>
    public partial class DetermineCollectionsPluginSettingsControl : UserControl
    {
        private DetermineCollectionsPluginSettings settings;

        public DetermineCollectionsPluginSettingsControl(DetermineCollectionsPluginSettings settings)
        {
            this.settings = settings;
            InitializeComponent();
            this.DataContext = this.settings;
        }
    }
}