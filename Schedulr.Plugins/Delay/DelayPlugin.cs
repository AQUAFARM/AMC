using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Schedulr.Extensibility;

namespace Schedulr.Plugins.Delay
{
    [Plugin("Delay", "Delay", "Delays for a number of seconds before running the next action.")]
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
    public class DelayPlugin : EventPlugin<DelayPluginSettings, DelayPluginSettingsControl>
    {
        protected override DelayPluginSettingsControl GetSettingsControl(DelayPluginSettings settings)
        {
            return new DelayPluginSettingsControl(settings);
        }

        public override void OnAccountEvent(AccountEventArgs args)
        {
            PerformDelay(args);
        }

        public override void OnApplicationEvent(ApplicationEventArgs args)
        {
            PerformDelay(args);
        }

        public override void OnBatchEvent(BatchEventArgs args)
        {
            PerformDelay(args);
        }

        public override void OnConfigurationEvent(ConfigurationEventArgs args)
        {
            PerformDelay(args);
        }

        public override void OnGeneralAccountEvent(GeneralAccountEventArgs args)
        {
            PerformDelay(args);
        }

        public override void OnPictureEvent(PictureEventArgs args)
        {
            PerformDelay(args);
        }

        public override void OnScheduledTaskEvent(ScheduledTaskEventArgs args)
        {
            PerformDelay(args);
        }

        private void PerformDelay(PluginEventArgs args)
        {
            this.Host.Logger.Log(string.Format(CultureInfo.CurrentCulture, "Delaying for {0} seconds", this.Settings.DelaySeconds), TraceEventType.Information);
            Thread.Sleep(1000 * this.Settings.DelaySeconds);
        }
    }
}