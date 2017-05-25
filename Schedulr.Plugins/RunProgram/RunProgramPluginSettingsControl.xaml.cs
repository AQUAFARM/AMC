using System.IO;
using System.Windows;
using System.Windows.Controls;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.RunProgram
{
    /// <summary>
    /// Interaction logic for RunProgramPluginSettingsControl.xaml
    /// </summary>
    public partial class RunProgramPluginSettingsControl : UserControl
    {
        private RunProgramPluginSettings settings;
        private IPluginHost host;

        public RunProgramPluginSettingsControl(RunProgramPluginSettings settings, IPluginHost host)
        {
            InitializeComponent();
            this.settings = settings;
            this.host = host;
            this.DataContext = this.settings;
        }

        private void browseFileNameButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = "Please select the program to run";
            openFileDialog.Filter = "Programs|*.exe;*.com;*.bat;*.cmd|All files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                this.settings.FileName = openFileDialog.FileName;
                this.settings.WorkingDirectory = Path.GetDirectoryName(this.settings.FileName);
            }
        }

        private void browseWorkingDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Please select a working folder for the program.";
            dialog.SelectedPath = this.settings.WorkingDirectory;
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.settings.WorkingDirectory = dialog.SelectedPath;
            }
        }

        private void editArgumentsTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            this.settings.Arguments = this.host.ShowTemplateEditorDialog(this.settings.Arguments, "Edit the program arguments");
        }
    }
}