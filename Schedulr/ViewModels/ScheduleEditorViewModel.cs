using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using JelleDruyts.Windows;
using JelleDruyts.Windows.Controls;
using JelleDruyts.Windows.ObjectModel;
using Microsoft.Win32;
using Schedulr.Extensibility;
using Schedulr.Infrastructure;
using Schedulr.Messages;
using Schedulr.Models;

namespace Schedulr.ViewModels
{
    public class ScheduleEditorViewModel : ViewModel
    {
        #region Fields

        private Account account;
        private RelayCommand createTaskCommand;
        private RelayCommand updateTaskCommand;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the task commands that are available.
        /// </summary>
        public IEnumerable<ICommand> TaskCommands { get; private set; }

        /// <summary>
        /// Gets the status commands that are available.
        /// </summary>
        public IEnumerable<ICommand> StatusCommands { get; private set; }

        /// <summary>
        /// Gets the command that refreshes the status of the wake timers.
        /// </summary>
        public ICommand RefreshWakeTimersStatusCommand { get; private set; }

        /// <summary>
        /// Gets the input bindings for the commands.
        /// </summary>
        public IEnumerable<InputBinding> InputBindings { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets the upload schedule.
        /// </summary>
        public UploadSchedule UploadSchedule
        {
            get { return this.GetValue(UploadScheduleProperty); }
            set { this.SetValue(UploadScheduleProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UploadSchedule"/> observable property.
        /// </summary>
        public static ObservableProperty<UploadSchedule> UploadScheduleProperty = new ObservableProperty<UploadSchedule, ScheduleEditorViewModel>(o => o.UploadSchedule);

        /// <summary>
        /// Gets or sets the status of the scheduled task.
        /// </summary>
        public ScheduledTaskStatus Status
        {
            get { return this.GetValue(StatusProperty); }
            set { this.SetValue(StatusProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Status"/> observable property.
        /// </summary>
        public static ObservableProperty<ScheduledTaskStatus> StatusProperty = new ObservableProperty<ScheduledTaskStatus, ScheduleEditorViewModel>(o => o.Status);

        /// <summary>
        /// Gets or sets the visibility of the task status.
        /// </summary>
        public Visibility StatusVisibility
        {
            get { return this.GetValue(StatusVisibilityProperty); }
            set { this.SetValue(StatusVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="StatusVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> StatusVisibilityProperty = new ObservableProperty<Visibility, ScheduleEditorViewModel>(o => o.StatusVisibility);

        /// <summary>
        /// Gets or sets a value that determines if a scheduled task exists for this account.
        /// </summary>
        public bool ScheduledTaskExists
        {
            get { return this.GetValue(ScheduledTaskExistsProperty); }
            set { this.SetValue(ScheduledTaskExistsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ScheduledTaskExists"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> ScheduledTaskExistsProperty = new ObservableProperty<bool, ScheduleEditorViewModel>(o => o.ScheduledTaskExists);

        /// <summary>
        /// Gets or sets a value that determines if the schedule has unsaved changes.
        /// </summary>
        public bool HasUnsavedChanges
        {
            get { return this.GetValue(HasUnsavedChangesProperty); }
            set { this.SetValue(HasUnsavedChangesProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="HasUnsavedChanges"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> HasUnsavedChangesProperty = new ObservableProperty<bool, ScheduleEditorViewModel>(o => o.HasUnsavedChanges);

        /// <summary>
        /// Gets or sets the status message to display to the user.
        /// </summary>
        public string StatusMessage
        {
            get { return this.GetValue(StatusMessageProperty); }
            set { this.SetValue(StatusMessageProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="StatusMessage"/> observable property.
        /// </summary>
        public static ObservableProperty<string> StatusMessageProperty = new ObservableProperty<string, ScheduleEditorViewModel>(o => o.StatusMessage);

        /// <summary>
        /// Gets or sets a value that determines if the status message should be displayed as a warning.
        /// </summary>
        public bool StatusMessageIsWarning
        {
            get { return this.GetValue(StatusMessageIsWarningProperty); }
            set { this.SetValue(StatusMessageIsWarningProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="StatusMessageIsWarning"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> StatusMessageIsWarningProperty = new ObservableProperty<bool, ScheduleEditorViewModel>(o => o.StatusMessageIsWarning);

        /// <summary>
        /// Gets or sets the visibility of the wake timers warning message.
        /// </summary>
        public Visibility WakeTimersWarningVisibility
        {
            get { return this.GetValue(WakeTimersWarningVisibilityProperty); }
            set { this.SetValue(WakeTimersWarningVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WakeTimersWarningVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> WakeTimersWarningVisibilityProperty = new ObservableProperty<Visibility, ScheduleEditorViewModel>(o => o.WakeTimersWarningVisibility, Visibility.Collapsed);

        #endregion

        #region Convenience Properties

        public DateTime StartDate
        {
            get
            {
                return this.UploadSchedule.StartTime.Date;
            }
            set
            {
                this.UploadSchedule.StartTime = value.Date.AddHours(this.StartHour).AddMinutes(this.StartMinute);
                OnPropertyChanged("StartDate");
            }
        }

        public int StartHour
        {
            get
            {
                return this.UploadSchedule.StartTime.Hour;
            }
            set
            {
                this.UploadSchedule.StartTime = this.StartDate.Date.AddHours(value).AddMinutes(this.StartMinute);
                OnPropertyChanged("StartHour");
            }
        }

        public int StartMinute
        {
            get
            {
                return this.UploadSchedule.StartTime.Minute;
            }
            set
            {
                this.UploadSchedule.StartTime = this.StartDate.Date.AddHours(this.StartHour).AddMinutes(value);
                OnPropertyChanged("StartMinute");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleEditorViewModel"/> class.
        /// </summary>
        public ScheduleEditorViewModel()
        {
            this.createTaskCommand = new RelayCommand(CreateTask, CanCreateTask, "_Create", "Creates the task [CTRL-S]", new KeyGesture(Key.S, ModifierKeys.Control));
            this.updateTaskCommand = new RelayCommand(UpdateTask, CanUpdateTask, "_Save Changes", "Saves changes to the task [CTRL-S]", new KeyGesture(Key.S, ModifierKeys.Control));
            this.TaskCommands = new ICommand[]
            {
                this.createTaskCommand,
                this.updateTaskCommand,
                new RelayCommand(DeleteTask, CanDeleteTask, "_Delete", "Deletes the task [DEL]", new KeyGesture(Key.Delete)),
            };
            this.StatusCommands = new ICommand[]
            {
                new RelayCommand(EnableTask, CanEnableTask, "_Enable", "Enables the task [CTRL-E]", new KeyGesture(Key.E, ModifierKeys.Control)),
                new RelayCommand(DisableTask, CanDisableTask, "D_isable", "Disables the task [CTRL-E]", new KeyGesture(Key.E, ModifierKeys.Control)),
                new RelayCommand(RefreshStatus, CanRefreshStatus, "_Refresh Status", "Refreshes the status of the task [F5]", new KeyGesture(Key.F5))
            };
            this.RefreshWakeTimersStatusCommand = new RelayCommand(RefreshWakeTimersStatus, CanRefreshWakeTimersStatus);
            this.InputBindings = this.TaskCommands.Union(this.StatusCommands).OfType<RelayCommand>().SelectMany(r => r.InputGestures.Select(g => new InputBinding(r, g)));
            this.UploadSchedule = new UploadSchedule();
            Messenger.Register<AccountActionMessage>(OnAccountActionMessage);
        }

        #endregion

        #region Message Handlers

        private void OnAccountActionMessage(AccountActionMessage message)
        {
            if (message.Action == ListAction.CurrentChanged)
            {
                Logger.Log("ScheduleEditorViewModel - Current account changed, updating view", TraceEventType.Verbose);
                this.account = message.Account;
                if (this.UploadSchedule != null)
                {
                    this.UploadSchedule.PropertyChanged -= UploadScheduleChanged;
                }
                this.HasUnsavedChanges = false;
                if (this.account != null)
                {
                    this.UploadSchedule = this.account.UploadSchedule;
                }
                else
                {
                    this.UploadSchedule = new UploadSchedule();
                }
                this.StartDate = this.StartDate;
                this.StartHour = this.StartHour;
                this.StartMinute = this.StartMinute;
                this.UploadSchedule.PropertyChanged += UploadScheduleChanged;
                RefreshWakeTimersStatus(null);
                RefreshStatus();
                Logger.Log("ScheduleEditorViewModel - Current account changed, updating view - done", TraceEventType.Verbose);
            }
        }

        private void UploadScheduleChanged(object sender, PropertyChangedEventArgs e)
        {
            this.HasUnsavedChanges = true;
            if (string.Equals(e.PropertyName, UploadSchedule.WakeComputerProperty.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(e.PropertyName, UploadSchedule.OnlyIfNotOnBatteryPowerProperty.Name, StringComparison.OrdinalIgnoreCase))
            {
                RefreshWakeTimersStatus(null);
            }
            RefreshStatus();
        }

        #endregion

        #region Task Commands

        private bool CanCreateTask(object parameter)
        {
            return !this.ScheduledTaskExists;
        }

        private void CreateTask(object parameter)
        {
            PluginManager.OnScheduledTaskEvent(new ScheduledTaskEventArgs(ScheduledTaskEventType.Creating, App.Info, this.account, ScheduledTaskClient.GetTaskName(this.account), false, this.UploadSchedule));
            SaveTask("Creating task");
            PluginManager.OnScheduledTaskEvent(new ScheduledTaskEventArgs(ScheduledTaskEventType.Created, App.Info, this.account, ScheduledTaskClient.GetTaskName(this.account), true, this.UploadSchedule));
        }

        private bool CanUpdateTask(object parameter)
        {
            return this.ScheduledTaskExists;
        }

        private void UpdateTask(object parameter)
        {
            var isEnabled = (this.Status != null ? this.Status.Enabled : false);
            PluginManager.OnScheduledTaskEvent(new ScheduledTaskEventArgs(ScheduledTaskEventType.Updating, App.Info, this.account, ScheduledTaskClient.GetTaskName(this.account), isEnabled, this.UploadSchedule));
            SaveTask("Updating task");
            PluginManager.OnScheduledTaskEvent(new ScheduledTaskEventArgs(ScheduledTaskEventType.Updated, App.Info, this.account, ScheduledTaskClient.GetTaskName(this.account), isEnabled, this.UploadSchedule));
        }

        private bool CanDeleteTask(object parameter)
        {
            return this.ScheduledTaskExists;
        }

        private void DeleteTask(object parameter)
        {
            var isEnabled = (this.Status != null ? this.Status.Enabled : false);
            PluginManager.OnScheduledTaskEvent(new ScheduledTaskEventArgs(ScheduledTaskEventType.Deleting, App.Info, this.account, ScheduledTaskClient.GetTaskName(this.account), isEnabled, this.UploadSchedule));
            ScheduledTaskClient.DeleteAccountTask(this.account);
            this.HasUnsavedChanges = false;
            Messenger.Send<UploadScheduleChangedMessage>(new UploadScheduleChangedMessage());
            Messenger.Send<StatusMessage>(new StatusMessage(string.Format(CultureInfo.CurrentCulture, "The scheduled task for account \"{0}\" was deleted.", this.account.Name)));
            RefreshStatus();
            PluginManager.OnScheduledTaskEvent(new ScheduledTaskEventArgs(ScheduledTaskEventType.Deleted, App.Info, this.account, ScheduledTaskClient.GetTaskName(this.account), false, this.UploadSchedule));
        }

        #endregion

        #region Status Commands

        private bool CanRefreshStatus(object parameter)
        {
            return this.ScheduledTaskExists;
        }

        private void RefreshStatus(object parameter)
        {
            RefreshStatus();
        }

        private bool CanEnableTask(object parameter)
        {
            return (this.Status != null && !this.Status.Enabled);
        }

        private void EnableTask(object parameter)
        {
            SetTaskEnabled(true);
        }

        private bool CanDisableTask(object parameter)
        {
            return (this.Status != null && this.Status.Enabled);
        }

        private void DisableTask(object parameter)
        {
            SetTaskEnabled(false);
        }

        #endregion

        #region RefreshWakeTimersStatus Command

        private bool CanRefreshWakeTimersStatus(object parameter)
        {
            return true;
        }

        private void RefreshWakeTimersStatus(object parameter)
        {
            try
            {
                // The WakeComputer or OnlyIfNotOnBatteryPower property changed.
                // If the computer should be woken, verify that wake timers are actually enabled on the system.
                var showWarning = false;
                if (this.UploadSchedule.WakeComputer)
                {
                    using (var powerSchemesKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes"))
                    {
                        if (powerSchemesKey != null)
                        {
                            var activePowerSchemeId = (string)powerSchemesKey.GetValue("ActivePowerScheme");
                            using (var wakeTimersKey = powerSchemesKey.OpenSubKey(activePowerSchemeId + @"\238c9fa8-0aad-41ed-83f4-97be242c8f20\bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d"))
                            {
                                // The wake timers are disabled by default (i.e. if there is no registry key).
                                var wakeTimersEnabledAC = false;
                                var wakeTimersEnabledBattery = false;

                                // If there is a registry key, check the values there.
                                if (wakeTimersKey != null)
                                {
                                    wakeTimersEnabledAC = ((int)wakeTimersKey.GetValue("ACSettingIndex") == 1);
                                    wakeTimersEnabledBattery = ((int)wakeTimersKey.GetValue("DCSettingIndex") == 1);
                                }

                                // Show the warning when the wake timers are not enabled on AC power.
                                showWarning = !wakeTimersEnabledAC;
                                if (!showWarning && !this.UploadSchedule.OnlyIfNotOnBatteryPower)
                                {
                                    // The task should even run when on battery power, check the DC setting as well.
                                    showWarning = !wakeTimersEnabledBattery;
                                }
                            }
                        }
                    }
                }
                this.WakeTimersWarningVisibility = (showWarning ? Visibility.Visible : Visibility.Collapsed);
            }
            catch (Exception exc)
            {
                Logger.Log("Error while checking the registry for wake timers", exc);
            }
        }

        #endregion

        #region Helper Methods

        private void SaveTask(string description)
        {
            UpdateScheduledTask(description, this.account, (userName, password) =>
            {
                ScheduledTaskClient.SaveAccountTask(this.account, userName, password);
                this.HasUnsavedChanges = false;
            });
        }

        private void SetTaskEnabled(bool enabled)
        {
            PluginManager.OnScheduledTaskEvent(new ScheduledTaskEventArgs(ScheduledTaskEventType.EnabledChanging, App.Info, this.account, ScheduledTaskClient.GetTaskName(this.account), !enabled, this.UploadSchedule));
            var description = (enabled ? "Enabling task" : "Disabling task");
            UpdateScheduledTask(description, this.account, (userName, password) =>
            {
                ScheduledTaskClient.SetAccountTaskEnabled(this.account, enabled, userName, password);
            });
            PluginManager.OnScheduledTaskEvent(new ScheduledTaskEventArgs(ScheduledTaskEventType.EnabledChanging, App.Info, this.account, ScheduledTaskClient.GetTaskName(this.account), enabled, this.UploadSchedule));
        }

        private void UpdateScheduledTask(string description, Account account, Action<string, string> updateActionWithUserNameAndPasswordCallback)
        {
            var updatingTaskTask = new ApplicationTask(description);
            var completedMessage = "Done";
            try
            {
                Messenger.Send<TaskStatusMessage>(new TaskStatusMessage(updatingTaskTask));
                updatingTaskTask.Status = string.Format(CultureInfo.CurrentCulture, "{0} for account \"{1}\"", description, account.Name);
                string userName;
                string password;
                bool cancelled;
                ProvideCredentialsIfNeeded(account, out userName, out password, out cancelled);
                if (cancelled)
                {
                    completedMessage = "Cancelled";
                }
                else
                {
                    try
                    {
                        updateActionWithUserNameAndPasswordCallback(userName, password);
                        Messenger.Send<UploadScheduleChangedMessage>(new UploadScheduleChangedMessage());
                    }
                    catch (Exception exc)
                    {
                        Logger.Log("Error updating scheduled task", exc);
                        updatingTaskTask.SetError(exc);
                        completedMessage = "Failed";
                        MessageBox.Show(exc.Message, "Error updating task", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    RefreshStatus();
                }
            }
            finally
            {
                updatingTaskTask.SetComplete(completedMessage);
            }
        }

        private static void ProvideCredentialsIfNeeded(Account account, out string userName, out string password, out bool cancelled)
        {
            userName = null;
            password = null;
            cancelled = false;
            if (!account.UploadSchedule.OnlyIfLoggedOn)
            {
                using (UserCredentialsDialog dialog = new UserCredentialsDialog(App.Info.Name, "Enter your Windows password", "Since this task will also run if you are not logged on, Windows needs to validate your password."))
                {
                    dialog.Flags = UserCredentialsDialogFlags.KeepUsername | UserCredentialsDialogFlags.DoNotPersist;
                    userName = WindowsIdentity.GetCurrent().Name;
                    dialog.User = userName;
                    var result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        password = dialog.PasswordToString();
                    }
                    else
                    {
                        cancelled = true;
                    }
                }
            }
        }

        private void RefreshStatus()
        {
            this.ScheduledTaskExists = ScheduledTaskClient.AccountTaskExists(this.account);
            this.Status = ScheduledTaskClient.GetAccountTaskStatus(this.account);
            this.StatusVisibility = (this.Status == null ? Visibility.Collapsed : Visibility.Visible);
            string message = null;
            bool isWarning = false;
            if (this.HasUnsavedChanges)
            {
                message = string.Format(CultureInfo.CurrentCulture, "There are unsaved changes. Click \"{0}\" to update the scheduled task now.", this.updateTaskCommand.Name.Replace("_", string.Empty));
                isWarning = true;
            }
            else
            {
                if (this.ScheduledTaskExists)
                {
                    if (this.Status != null && this.Status.Enabled)
                    {
                        message = "The scheduled task is active.";
                    }
                    else
                    {
                        message = "The scheduled task is not enabled; your files will not be automatically uploaded.";
                        isWarning = true;
                    }
                }
                else
                {
                    message = string.Format(CultureInfo.CurrentCulture, "A scheduled task has not been created yet; your files will not be automatically uploaded. Click \"{0}\" to create the task now.", this.createTaskCommand.Name.Replace("_", string.Empty));
                    isWarning = true;
                }
            }
            this.StatusMessage = message;
            this.StatusMessageIsWarning = isWarning;
        }

        #endregion
    }
}