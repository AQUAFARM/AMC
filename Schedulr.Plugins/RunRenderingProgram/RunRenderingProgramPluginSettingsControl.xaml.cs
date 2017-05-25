using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using JelleDruyts.Windows.Text;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.RunRenderingProgram
{
    /// <summary>
    /// Interaction logic for RunRenderingProgramPluginSettingsControl.xaml
    /// </summary>
    public partial class RunRenderingProgramPluginSettingsControl : UserControl
    {
        private RunRenderingProgramPluginSettings settings;
        private IPluginHost host;

        public RunRenderingProgramPluginSettingsControl(RunRenderingProgramPluginSettings settings, IPluginHost host)
        {
            InitializeComponent();
            this.settings = settings;
            this.host = host;
            this.DataContext = this.settings;
            var info = new StringBuilder();
            var inputFileToken = string.Format(CultureInfo.CurrentCulture, TemplateProcessor.DefaultTokenFormatWithoutFormatSpecifier, RunRenderingProgramPlugin.RenderingInputFileTokenName);
            var outputFileToken = string.Format(CultureInfo.CurrentCulture, TemplateProcessor.DefaultTokenFormatWithoutFormatSpecifier, RunRenderingProgramPlugin.RenderingOutputFileTokenName);
            info.AppendFormat(CultureInfo.CurrentCulture, "Make sure to use the special \"{0}\" and \"{1}\" tokens in the arguments if you want to refer to the input and output files respectively. ", inputFileToken, outputFileToken);
            info.Append("This makes sure that the input file will include any previous modifications, and that subsequent modifications will be based on the program's output file.");
            this.argumentsInfoTextBlock.Text = info.ToString();
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
            var additionalTokens = new TemplateTokenInfo[]
            {
                new TemplateTokenInfo(RunRenderingProgramPlugin.RenderingInputFileTokenName, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Input" + RunRenderingProgramPlugin.GetFileExtension("Input.jpg", this.settings.InputFileExtension))),
                new TemplateTokenInfo(RunRenderingProgramPlugin.RenderingOutputFileTokenName, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Output" + RunRenderingProgramPlugin.GetFileExtension("Output.jpg", this.settings.OutputFileExtension))),
            };
            this.settings.Arguments = this.host.ShowTemplateEditorDialog(this.settings.Arguments, "Edit the program arguments", additionalTokens);
        }
    }
}