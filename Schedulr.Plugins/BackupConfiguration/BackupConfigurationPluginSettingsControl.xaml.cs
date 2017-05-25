using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Schedulr.Plugins.BackupConfiguration
{
    /// <summary>
    /// Interaction logic for BackupConfigurationSettingsControl.xaml
    /// </summary>
    public partial class BackupConfigurationPluginSettingsControl : UserControl
    {
        private BackupConfigurationPluginSettings settings;

        public BackupConfigurationPluginSettingsControl(BackupConfigurationPluginSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
            this.DataContext = this.settings;
        }

        private void browseBackupFileNameButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Please select a file name for the backups";
            saveFileDialog.Filter = "XML files|*.xml|All files|*.*";
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                this.settings.BackupFileName = saveFileDialog.FileName;
            }
        }
    }
}