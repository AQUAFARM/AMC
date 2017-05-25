using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.TaskScheduler;
using Schedulr.Models;

namespace Schedulr.Infrastructure
{
    public static class ScheduledTaskClient
    {
        #region AccountTaskExists

        public static bool AccountTaskExists(Account account)
        {
            if (account == null)
            {
                return false;
            }
            else
            {
                using (TaskService service = new TaskService())
                {
                    var task = GetTaskForAccount(service, account);
                    return (task != null);
                }
            }
        }

        #endregion

        #region SaveAccountTask

        public static void SaveAccountTask(Account account, string userName, string password)
        {
            using (TaskService service = new TaskService())
            {
                // Get an existing task or create a new one.
                var taskDefinition = GetOrCreateTaskDefinitionForAccount(service, account);

                // Create the trigger.
                taskDefinition.Triggers.Clear();
                Trigger trigger;
                if (account.UploadSchedule.Type == ScheduleType.Daily)
                {
                    var dailyTrigger = new DailyTrigger();
                    if (account.UploadSchedule.DailyInterval <= 0)
                    {
                        throw new ArgumentException("The daily interval is not a positive number: " + account.UploadSchedule.DailyInterval);
                    }
                    dailyTrigger.DaysInterval = (short)account.UploadSchedule.DailyInterval;
                    trigger = dailyTrigger;
                }
                else if (account.UploadSchedule.Type == ScheduleType.Weekly)
                {
                    var weeklyTrigger = new WeeklyTrigger();
                    if (account.UploadSchedule.WeeklyInterval <= 0)
                    {
                        throw new ArgumentException("The weekly interval is not a positive number: " + account.UploadSchedule.WeeklyInterval);
                    }
                    weeklyTrigger.WeeksInterval = (short)account.UploadSchedule.WeeklyInterval;
                    weeklyTrigger.DaysOfWeek = 0;
                    if (account.UploadSchedule.WeeklyOnMonday) { weeklyTrigger.DaysOfWeek |= DaysOfTheWeek.Monday; }
                    if (account.UploadSchedule.WeeklyOnTuesday) { weeklyTrigger.DaysOfWeek |= DaysOfTheWeek.Tuesday; }
                    if (account.UploadSchedule.WeeklyOnWednesday) { weeklyTrigger.DaysOfWeek |= DaysOfTheWeek.Wednesday; }
                    if (account.UploadSchedule.WeeklyOnThursday) { weeklyTrigger.DaysOfWeek |= DaysOfTheWeek.Thursday; }
                    if (account.UploadSchedule.WeeklyOnFriday) { weeklyTrigger.DaysOfWeek |= DaysOfTheWeek.Friday; }
                    if (account.UploadSchedule.WeeklyOnSaturday) { weeklyTrigger.DaysOfWeek |= DaysOfTheWeek.Saturday; }
                    if (account.UploadSchedule.WeeklyOnSunday) { weeklyTrigger.DaysOfWeek |= DaysOfTheWeek.Sunday; }
                    trigger = weeklyTrigger;
                }
                else
                {
                    throw new ArgumentException("The schedule type is invalid: " + account.UploadSchedule.Type.ToString());
                }

                if (account.UploadSchedule.HourlyInterval > 0)
                {
                    trigger.Repetition.Interval = TimeSpan.FromHours(account.UploadSchedule.HourlyInterval);
                }
                if (account.UploadSchedule.HourlyIntervalDuration > 0)
                {
                    trigger.Repetition.Duration = TimeSpan.FromHours(account.UploadSchedule.HourlyIntervalDuration);
                }
                else
                {
                    // Normally 0 means indefinitely, but we replace that with 24 hours.
                    trigger.Repetition.Duration = TimeSpan.FromHours(24);
                }

                trigger.StartBoundary = account.UploadSchedule.StartTime;

                taskDefinition.Triggers.Add(trigger);

                // v1 of the task scheduler only supports one ExecAction, does not allow it to be removed and needs it to be added directly like below.
                // This would keep adding it in v2 as well, so in that case we first clear the actions.
                if (service.HighestSupportedVersion > new Version(1, 1))
                {
                    taskDefinition.Actions.Clear();
                }
                taskDefinition.Actions.Add(new ExecAction(App.Info.ExecutablePath, Program.GetCommandLineArgumentsForUploadBackground(account.Name), null));

                // Settings.
                taskDefinition.Settings.DisallowStartIfOnBatteries = account.UploadSchedule.OnlyIfNotOnBatteryPower;
                taskDefinition.Settings.StopIfGoingOnBatteries = false;
                taskDefinition.Settings.RunOnlyIfNetworkAvailable = true;
                taskDefinition.Settings.WakeToRun = account.UploadSchedule.WakeComputer;
                // Note: do not set taskDefinition.Principal.LogonType because that is not supported on v1 of the task scheduler.

                // Save changes.
                RegisterTask(service, account, taskDefinition, userName, password);
            }
        }

        #endregion

        #region DeleteAccountTask

        public static bool DeleteAccountTask(Account account)
        {
            using (TaskService service = new TaskService())
            {
                var task = GetTaskForAccount(service, account);
                if (task != null)
                {
                    service.RootFolder.DeleteTask(task.Name);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region SetAccountTaskEnabled

        public static void SetAccountTaskEnabled(Account account, bool enabled, string userName, string password)
        {
            using (TaskService service = new TaskService())
            {
                var task = GetTaskForAccount(service, account);
                if (task != null)
                {
                    task.Enabled = enabled;

                    // Save changes.
                    RegisterTask(service, account, task.Definition, userName, password);
                }
            }
        }

        #endregion

        #region GetAccountTaskStatus

        public static ScheduledTaskStatus GetAccountTaskStatus(Account account)
        {
            if (account == null)
            {
                return null;
            }
            else
            {
                using (TaskService service = new TaskService())
                {
                    var task = GetTaskForAccount(service, account);
                    if (task == null)
                    {
                        return null;
                    }
                    else
                    {
                        DateTime? lastRunTime = (task.LastRunTime == DateTime.MinValue ? (DateTime?)null : task.LastRunTime);
                        int? lastRunResult = (lastRunTime.HasValue ? task.LastTaskResult : (int?)null);
                        DateTime? nextRunTime = (task.NextRunTime < DateTime.Now ? (DateTime?)null : task.NextRunTime);
                        return new ScheduledTaskStatus(task.Name, task.Enabled, lastRunTime, lastRunResult, nextRunTime, task.State.ToString());
                    }
                }
            }
        }

        #endregion

        #region GetNextRunTimesForAccount

        public static IList<DateTime> GetNextRunTimesForAccount(Account account, int count)
        {
            IList<DateTime> runTimes = new DateTime[0];
            if (count > 0)
            {
                using (TaskService service = new TaskService())
                {
                    var task = GetTaskForAccount(service, account);
                    if (task != null)
                    {
                        try
                        {
                            runTimes = task.GetRunTimes(DateTime.Now, DateTime.MaxValue, (uint)count).ToList();
                        }
                        catch (Exception exc)
                        {
                            Logger.Log(string.Format(CultureInfo.CurrentCulture, "Could not get the next {0} run times for the scheduled task", count), exc);
                        }
                    }
                }
            }
            return runTimes;
        }

        #endregion

        #region Helper Methods

        public static string GetTaskName(Account account)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", App.Info.Name, account.Id);
        }

        private static Microsoft.Win32.TaskScheduler.Task GetTaskForAccount(TaskService service, Account account)
        {
            var taskName = GetTaskName(account);
            return service.RootFolder.Tasks.Where(t => string.Equals(t.Name, taskName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        private static TaskDefinition GetOrCreateTaskDefinitionForAccount(TaskService service, Account account)
        {
            var task = GetTaskForAccount(service, account);
            if (task == null)
            {
                var taskDefinition = service.NewTask();
                taskDefinition.RegistrationInfo.Author = App.Info.Name;
                try
                {
                    taskDefinition.RegistrationInfo.Date = DateTime.Now;
                }
                catch (FileNotFoundException)
                {
                    // This exception can be thrown on v1 of the task scheduler.
                }
                taskDefinition.RegistrationInfo.Description = string.Format(CultureInfo.CurrentCulture, "Uploads the next batch of files for the account \"{0}\" to Flickr.", account.Name);
                taskDefinition.Settings.Enabled = true;
                return taskDefinition;
            }
            else
            {
                return task.Definition;
            }
        }

        private static void RegisterTask(TaskService service, Account account, TaskDefinition taskDefinition, string userName, string password)
        {
            var taskName = GetTaskName(account);
            var logonType = (account.UploadSchedule.OnlyIfLoggedOn ? TaskLogonType.InteractiveToken : TaskLogonType.Password);

            if (logonType == TaskLogonType.Password || logonType == TaskLogonType.InteractiveTokenOrPassword)
            {
                if (string.IsNullOrEmpty(userName))
                {
                    throw new ArgumentException("The username and password must be supplied if the user does not have to be logged on to run the task.");
                }
            }
            try
            {
                // Register the task in the root folder.
                service.RootFolder.RegisterTaskDefinition(taskName, taskDefinition, TaskCreation.CreateOrUpdate, userName, password, logonType, null);
            }
            catch (COMException exc)
            {
                if (exc.ErrorCode == -2147023570)
                {
                    throw new SecurityException("Logon failure: unknown user name or bad password.", exc);
                }
                throw;
            }
        }

        #endregion
    }
}