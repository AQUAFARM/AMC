using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.BackupConfiguration
{
    [Plugin("Backup Configuration", "Create a backup of the configuration", "Creates a backup of the application's configuration file.")]
    [SupportedApplicationEvent(ApplicationEventType.Started)]
    [SupportedApplicationEvent(ApplicationEventType.Closing)]
    [SupportedConfigurationEvent(ConfigurationEventType.Saving)]
    [SupportedConfigurationEvent(ConfigurationEventType.Saved)]
    public class BackupConfigurationPlugin : EventPlugin<BackupConfigurationPluginSettings, BackupConfigurationPluginSettingsControl>
    {
        #region Plugin Implementation

        protected override BackupConfigurationPluginSettingsControl GetSettingsControl(BackupConfigurationPluginSettings settings)
        {
            return new BackupConfigurationPluginSettingsControl(settings);
        }

        public override void OnApplicationEvent(ApplicationEventArgs args)
        {
            PerformBackup(args.ApplicationInfo.ConfigurationFileName);
        }

        public override void OnConfigurationEvent(ConfigurationEventArgs args)
        {
            PerformBackup(args.ApplicationInfo.ConfigurationFileName);
        }

        #endregion

        #region Perform Backup

        private void PerformBackup(string configurationFileName)
        {
            // Determine the backup file name and the previous backup file that was created.
            var backupFileName = this.Settings.BackupFileName;
            var lastBackupFileName = backupFileName;
            IList<string> previousBackups = null;
            if (this.Settings.Strategy == BackupStrategy.SuffixWithTimestamp)
            {
                var backupFolder = Path.GetDirectoryName(backupFileName);
                var baseFileName = Path.GetFileNameWithoutExtension(backupFileName);
                var timestamp = DateTime.Now.ToString("-yyyy.MM.dd-HH.mm.ss");
                var extension = Path.GetExtension(backupFileName);
                backupFileName = Path.Combine(backupFolder, baseFileName + timestamp + extension);
                var backupFileWithTimestampExpression = new Regex(baseFileName + @"-\d\d\d\d.\d\d.\d\d-\d\d.\d\d.\d\d" + extension, RegexOptions.IgnoreCase);
                previousBackups = Directory.GetFiles(backupFolder).Where(f => backupFileWithTimestampExpression.IsMatch(f)).OrderByDescending(f => f, StringComparer.OrdinalIgnoreCase).ToList();
                lastBackupFileName = previousBackups.FirstOrDefault();
            }

            // Only create a backup if the file is actually different than the last one.
            var isDifferent = true;
            if (lastBackupFileName != null && File.Exists(lastBackupFileName))
            {
                if (new FileInfo(configurationFileName).Length == new FileInfo(lastBackupFileName).Length)
                {
                    if (string.Equals(File.ReadAllText(configurationFileName), File.ReadAllText(lastBackupFileName), StringComparison.Ordinal))
                    {
                        isDifferent = false;
                    }
                }
            }
            if (isDifferent)
            {
                this.Host.Logger.Log(string.Format(CultureInfo.CurrentCulture, "Creating backup of configuration file at \"{0}\"", backupFileName), TraceEventType.Information);
                File.Copy(configurationFileName, backupFileName, true);
            }
            else
            {
                this.Host.Logger.Log("A backup is skipped since the configuration has not changed since the last time", TraceEventType.Verbose);
            }

            // Delete old backups.
            if (this.Settings.Strategy == BackupStrategy.SuffixWithTimestamp)
            {
                if (this.Settings.NumberOfBackupsToKeep > 0)
                {
                    var numberToKeep = this.Settings.NumberOfBackupsToKeep;
                    if (isDifferent)
                    {
                        // If a new backup was created, keep one less of the previous backups.
                        numberToKeep -= 1;
                    }
                    foreach (var backupToDelete in previousBackups.Skip(numberToKeep))
                    {
                        this.Host.Logger.Log(string.Format(CultureInfo.CurrentCulture, "Deleting old backup of configuration file at \"{0}\"", backupFileName), TraceEventType.Information);
                        File.Delete(backupToDelete);
                    }
                }
            }
        }

        #endregion
    }
}