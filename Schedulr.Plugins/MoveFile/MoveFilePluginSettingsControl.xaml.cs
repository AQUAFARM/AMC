using System;
using System.Windows;
using System.Windows.Controls;

namespace Schedulr.Plugins.MoveFile
{
    /// <summary>
    /// Interaction logic for MoveFilePluginSettingsControl.xaml
    /// </summary>
    public partial class MoveFilePluginSettingsControl : UserControl
    {
        private MoveFilePluginSettings settings;

        public MoveFilePluginSettingsControl(MoveFilePluginSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
            this.DataContext = this.settings;
        }

        private void browseDestinationFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Please select a folder to move the file to.";
            dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.settings.DestinationFolder = dialog.SelectedPath;
            }
        }
    }
}