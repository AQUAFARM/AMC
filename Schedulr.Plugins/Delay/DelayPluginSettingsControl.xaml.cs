using System.Windows.Controls;

namespace Schedulr.Plugins.Delay
{
    /// <summary>
    /// Interaction logic for DelayPluginSettingsControl.xaml
    /// </summary>
    public partial class DelayPluginSettingsControl : UserControl
    {
        private DelayPluginSettings settings;

        public DelayPluginSettingsControl(DelayPluginSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
            this.DataContext = this.settings;
        }
    }
}