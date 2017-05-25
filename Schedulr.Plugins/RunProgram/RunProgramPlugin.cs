using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.RunProgram
{
    [Plugin("Run Program", "Run a program", "Runs any program that you specify.")]
    [SupportedAccountEvent(AccountEventType.Activated)]
    [SupportedAccountEvent(AccountEventType.Deactivated)]
    [SupportedAccountEvent(AccountEventType.Refreshed)]
    [SupportedAccountEvent(AccountEventType.Refreshing)]
    [SupportedApplicationEvent(ApplicationEventType.Closing)]
    [SupportedApplicationEvent(ApplicationEventType.Started)]
    [SupportedBatchEvent(BatchEventType.Added)]
    [SupportedBatchEvent(BatchEventType.Adding)]
    [SupportedBatchEvent(BatchEventType.Removed)]
    [SupportedBatchEvent(BatchEventType.Removing)]
    [SupportedConfigurationEvent(ConfigurationEventType.Exported)]
    [SupportedConfigurationEvent(ConfigurationEventType.Exporting)]
    [SupportedConfigurationEvent(ConfigurationEventType.Loaded)]
    [SupportedConfigurationEvent(ConfigurationEventType.Saved)]
    [SupportedConfigurationEvent(ConfigurationEventType.Saving)]
    [SupportedGeneralAccountEvent(GeneralAccountEventType.Added)]
    [SupportedGeneralAccountEvent(GeneralAccountEventType.Adding)]
    [SupportedGeneralAccountEvent(GeneralAccountEventType.Removed)]
    [SupportedGeneralAccountEvent(GeneralAccountEventType.Removing)]
    [SupportedPictureEvent(PictureEventType.Added)]
    [SupportedPictureEvent(PictureEventType.Adding)]
    [SupportedPictureEvent(PictureEventType.RemovedFromQueue)]
    [SupportedPictureEvent(PictureEventType.RemovedFromUploads)]
    [SupportedPictureEvent(PictureEventType.RemovingFromQueue)]
    [SupportedPictureEvent(PictureEventType.RemovingFromUploads)]
    [SupportedPictureEvent(PictureEventType.Uploaded)]
    [SupportedPictureEvent(PictureEventType.Uploading)]
    [SupportedScheduledTaskEvent(ScheduledTaskEventType.Created)]
    [SupportedScheduledTaskEvent(ScheduledTaskEventType.Creating)]
    [SupportedScheduledTaskEvent(ScheduledTaskEventType.Deleted)]
    [SupportedScheduledTaskEvent(ScheduledTaskEventType.Deleting)]
    [SupportedScheduledTaskEvent(ScheduledTaskEventType.EnabledChanged)]
    [SupportedScheduledTaskEvent(ScheduledTaskEventType.EnabledChanging)]
    [SupportedScheduledTaskEvent(ScheduledTaskEventType.Updated)]
    [SupportedScheduledTaskEvent(ScheduledTaskEventType.Updating)]
    public class RunProgramPlugin : EventPlugin<RunProgramPluginSettings, RunProgramPluginSettingsControl>
    {
        protected override RunProgramPluginSettingsControl GetSettingsControl(RunProgramPluginSettings settings)
        {
            return new RunProgramPluginSettingsControl(settings, this.Host);
        }

        public override void OnAccountEvent(AccountEventArgs args)
        {
            RunProgram(args);
        }

        public override void OnApplicationEvent(ApplicationEventArgs args)
        {
            RunProgram(args);
        }

        public override void OnBatchEvent(BatchEventArgs args)
        {
            RunProgram(args);
        }

        public override void OnConfigurationEvent(ConfigurationEventArgs args)
        {
            RunProgram(args);
        }

        public override void OnGeneralAccountEvent(GeneralAccountEventArgs args)
        {
            RunProgram(args);
        }

        public override void OnPictureEvent(PictureEventArgs args)
        {
            RunProgram(args);
        }

        public override void OnScheduledTaskEvent(ScheduledTaskEventArgs args)
        {
            RunProgram(args);
        }

        private void RunProgram(PluginEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.Settings.FileName))
            {
                if (!File.Exists(this.Settings.FileName))
                {
                    this.Host.Logger.Log("The configured program does not exist: " + this.Settings.FileName, TraceEventType.Warning);
                }
                else
                {
                    var arguments = this.Host.ProcessTemplate(this.Settings.Arguments, args.TemplateTokenValues);
                    RunProgram(this.Settings.FileName, arguments, this.Settings.WorkingDirectory, this.Settings.WaitForExit, this.Settings.WaitForExitTimeoutSeconds, this.Host.Logger);
                }
            }
        }

        internal static void RunProgram(string fileName, string arguments, string workingDirectory, bool waitForExit, int waitForExitTimeoutSeconds, ILogger logger)
        {
            arguments = Environment.ExpandEnvironmentVariables(arguments);
            var argumentsMessage = (string.IsNullOrEmpty(arguments) ? string.Empty : string.Format(CultureInfo.CurrentCulture, " with the following arguments: {0}", arguments));
            logger.Log(string.Format(CultureInfo.CurrentCulture, "Running program \"{0}\"{1}", fileName, argumentsMessage), TraceEventType.Information);

            var info = new ProcessStartInfo(fileName, arguments);
            info.WorkingDirectory = workingDirectory;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            using (var process = Process.Start(info))
            {
                if (waitForExit)
                {
                    if (waitForExitTimeoutSeconds > 0)
                    {
                        logger.Log(string.Format(CultureInfo.CurrentCulture, "Waiting {0} seconds for the program to exit", waitForExitTimeoutSeconds), TraceEventType.Verbose);
                        process.WaitForExit(waitForExitTimeoutSeconds * 1000);
                    }
                    else
                    {
                        logger.Log("Waiting for the program to exit", TraceEventType.Verbose);
                        process.WaitForExit();
                    }
                    if (process.HasExited)
                    {
                        logger.Log("Program has exited with exit code " + process.ExitCode, TraceEventType.Verbose);
                        var output = process.StandardOutput.ReadToEnd();
                        if (!string.IsNullOrEmpty(output))
                        {
                            logger.Log("Program output: " + output, TraceEventType.Verbose);
                        }

                        var error = process.StandardError.ReadToEnd();
                        if (!string.IsNullOrEmpty(error))
                        {
                            // Only log the error output as a warnign if the process didn't exit successfully (exit code not equal to zero).
                            // This is because e.g. ffmpeg always writes to the error output stream, even if nothing failed.
                            var eventType = (process.ExitCode == 0 ? TraceEventType.Information : TraceEventType.Warning);
                            logger.Log("Program error output: " + error, eventType);
                        }
                    }
                    else
                    {
                        logger.Log("Program is still running", TraceEventType.Verbose);
                    }
                }
            }
        }
    }
}