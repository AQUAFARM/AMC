using System;
using System.Windows;
using System.Windows.Controls;

namespace Schedulr.Plugins.SaveCopy
{
    /// <summary>
    /// Interaction logic for SaveCopyPluginSettingsControl.xaml
    /// </summary>
    public partial class SaveCopyPluginSettingsControl : UserControl
    {
        private SaveCopyPluginSettings settings;

        public SaveCopyPluginSettingsControl(SaveCopyPluginSettings settings)
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