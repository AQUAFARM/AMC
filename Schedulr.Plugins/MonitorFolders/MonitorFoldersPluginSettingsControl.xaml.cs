using System;
using System.Windows.Controls;
using System.Windows.Input;
using JelleDruyts.Windows;

namespace Schedulr.Plugins.MonitorFolders
{
    /// <summary>
    /// Interaction logic for MonitorFoldersPluginSettingsControl.xaml
    /// </summary>
    public partial class MonitorFoldersPluginSettingsControl : UserControl
    {
        #region Properties

        public MonitorFoldersPluginSettings Settings { get; private set; }
        public ICommand AddFolderToMonitorCommand { get; private set; }
        public ICommand RemoveFolderToMonitorCommand { get; private set; }
        public ICommand DiscoverMonitoredFoldersCommand { get; private set; }

        #endregion

        #region Constructors

        public MonitorFoldersPluginSettingsControl(MonitorFoldersPluginSettings settings, ICommand discoverMonitoredFoldersCommand)
        {
            InitializeComponent();
            this.Settings = settings;
            this.AddFolderToMonitorCommand = new RelayCommand(AddFolderToMonitor, CanAddFolderToMonitor, "Add Folder To Monitor", "Adds a folder to monitor for new files");
            this.RemoveFolderToMonitorCommand = new RelayCommand(RemoveFolderToMonitor, CanRemoveFolderToMonitor, "Remove Folder To Monitor", "Stops monitoring the folder for new files");
            this.DiscoverMonitoredFoldersCommand = discoverMonitoredFoldersCommand;
            this.DataContext = this;
        }

        #endregion

        #region Commands

        private bool CanAddFolderToMonitor(object parameter)
        {
            return (this.Settings != null);
        }

        private void AddFolderToMonitor(object parameter)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Please select a folder to monitor for new files. All new pictures and videos in the selected folder (or any of its subfolders) will automatically be added to the queue.";
            dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.Settings.FoldersToMonitor.Add(dialog.SelectedPath);
            }
        }

        private bool CanRemoveFolderToMonitor(object parameter)
        {
            return (this.Settings != null && this.Settings.FoldersToMonitor.Contains(parameter as string));
        }

        private void RemoveFolderToMonitor(object parameter)
        {
            this.Settings.FoldersToMonitor.Remove(parameter as string);
        }

        #endregion
    }
}