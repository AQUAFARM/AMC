using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using JelleDruyts.Windows;
using Schedulr.Extensibility;
using Schedulr.Models;

namespace Schedulr.Plugins.MonitorFolders
{
    [Plugin("Monitor Folders", "Monitor folders for new pictures and videos", "Looks for new pictures and videos in a list of folders and adds them to the queue (in the order they were added to the folder).", InstantiationPolicy = PluginInstantiationPolicy.MultipleInstancesPerScope)]
    [SupportedAccountEvent(AccountEventType.Activated)]
    public class MonitorFoldersPlugin : EventPlugin<MonitorFoldersPluginSettings, MonitorFoldersPluginSettingsControl>
    {
        #region Fields

        private Account account;
        private ICommand discoverMonitoredFoldersCommand;

        #endregion

        #region Constructors

        public MonitorFoldersPlugin()
        {
            this.discoverMonitoredFoldersCommand = new RelayCommand(DiscoverMonitoredFolders, CanDiscoverMonitoredFolders, "Discover Files In Monitored Folders", "Adds new files in monitored folders to the queue");
        }

        #endregion

        #region Plugin Implementation

        protected override MonitorFoldersPluginSettingsControl GetSettingsControl(MonitorFoldersPluginSettings settings)
        {
            return new MonitorFoldersPluginSettingsControl(settings, this.discoverMonitoredFoldersCommand);
        }

        public override void OnAccountEvent(AccountEventArgs args)
        {
            this.account = args.Account;
            if (this.discoverMonitoredFoldersCommand.CanExecute(null))
            {
                this.discoverMonitoredFoldersCommand.Execute(null);
            }
        }

        #endregion

        #region Commands

        private bool CanDiscoverMonitoredFolders(object parameter)
        {
            return (this.account != null && this.Settings != null && this.Settings.FoldersToMonitor.Count > 0);
        }

        private void DiscoverMonitoredFolders(object parameter)
        {
            var addingFilesTask = new ApplicationTask("Discovering new files in monitored folders", this.Settings.FoldersToMonitor.Count);
            this.Host.RegisterApplicationTask(addingFilesTask);
            var step = 0;
            var filesToAdd = new List<string>();
            try
            {
                var newFiles = new List<string>();
                var includePattern = (this.Settings.SearchMode == SearchMode.Exclude ? "*.*" : this.Settings.SearchPattern ?? string.Empty);
                var excludePattern = (this.Settings.SearchMode == SearchMode.Include ? string.Empty : this.Settings.ExcludePattern ?? string.Empty);
                var existingFiles = this.account.QueuedBatches.Union(this.account.UploadedBatches).SelectMany(b => b.Pictures).Select(p => p.FileName);
                var searchOption = (this.Settings.SearchRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                foreach (var folder in this.Settings.FoldersToMonitor)
                {
                    var folderMessage = string.Format(CultureInfo.CurrentCulture, "Searching for new files for account \"{0}\" in monitored folder \"{1}\": including \"{2}\" and excluding \"{3}\"", this.account.Name, folder, includePattern, excludePattern);
                    addingFilesTask.SetProgress(step, folderMessage);
                    this.Host.Logger.Log(folderMessage, TraceEventType.Information);
                    try
                    {
                        IEnumerable<string> candidateFiles = new string[0];

                        // Include all files in the include pattern.
                        foreach (var searchPattern in includePattern.Split(';'))
                        {
                            var discoveredFiles = Directory.GetFiles(folder, searchPattern, searchOption);
                            candidateFiles = candidateFiles.Union(discoveredFiles);
                        }

                        // Exclude files specified in the exclude pattern.
                        foreach (var searchPattern in excludePattern.Split(';'))
                        {
                            var filesToExclude = Directory.GetFiles(folder, searchPattern, searchOption);
                            candidateFiles = candidateFiles.Except(filesToExclude, StringComparer.InvariantCultureIgnoreCase);
                        }

                        // Exclude files already queued or uploaded.
                        candidateFiles = candidateFiles.Except(existingFiles, StringComparer.OrdinalIgnoreCase);

                        // Exclude hidden files.
                        candidateFiles = candidateFiles.Where(f => !(new FileInfo(f).Attributes.HasFlag(FileAttributes.Hidden)));

                        newFiles.AddRange(candidateFiles.OrderBy(f => Path.GetDirectoryName(f)));
                    }
                    catch (Exception exc)
                    {
                        var warningMessage = string.Format(CultureInfo.CurrentCulture, "Could not search files in directory \"{0}\": {1}", folder, exc.Message);
                        addingFilesTask.SetWarning(warningMessage, exc);
                        this.Host.Logger.Log(warningMessage, exc, TraceEventType.Warning);
                    }
                    step++;
                }
                filesToAdd.AddRange(newFiles.Distinct(StringComparer.InvariantCultureIgnoreCase).OrderBy(f => new FileInfo(f).CreationTime));
                var resultMessage = string.Format(CultureInfo.CurrentCulture, "Discovered {0} for account \"{1}\"", filesToAdd.Count.ToCountString("file"), this.account.Name);
                addingFilesTask.Status = resultMessage;
                this.Host.Logger.Log(resultMessage, TraceEventType.Information);
                if (filesToAdd.Count > 0)
                {
                    if (this.Settings.BatchMode == BatchMode.BatchPerFolder)
                    {
                        foreach (var directory in filesToAdd.GroupBy(f => Path.GetDirectoryName(f)).OrderBy(d => d.Key))
                        {
                            this.Host.AddPicturesToQueue(this.account, directory.ToArray(), null, true);
                        }
                    }
                    else if (this.Settings.BatchMode == BatchMode.SingleBatch)
                    {
                        this.Host.AddPicturesToQueue(this.account, filesToAdd, null, true);
                    }
                    else
                    {
                        this.Host.AddPicturesToQueue(this.account, filesToAdd, null, false);
                    }
                }
            }
            finally
            {
                addingFilesTask.SetComplete(filesToAdd.Count.ToCountString("file", null, " discovered"));
            }
        }

        #endregion
    }
}