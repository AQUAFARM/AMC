using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows.Shell;
using JelleDruyts.Windows;
using Schedulr.Infrastructure;
using Schedulr.Messages;

namespace Schedulr.ViewModels
{
    public class StatusPanelViewModel : ViewModel
    {
        #region Fields

        private Timer removeTasksTimer;
        private System.Threading.ReaderWriterLockSlim executingTasksLock = new System.Threading.ReaderWriterLockSlim();

        #endregion

        #region Properties

        public ObservableCollection<ApplicationTaskViewModel> ExecutingTasks { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusPanelViewModel"/> class.
        /// </summary>
        public StatusPanelViewModel()
        {
            this.removeTasksTimer = new Timer(1000); // Check every second to remove tasks.
            this.removeTasksTimer.Elapsed += new ElapsedEventHandler(OnRemoveTasksTimerElapsed);
            this.ExecutingTasks = new ObservableCollection<ApplicationTaskViewModel>();
            Messenger.Register<StatusMessage>(OnStatusMessage);
        }

        #endregion

        #region Message Handlers

        private void OnStatusMessage(StatusMessage message)
        {
            this.ExecuteUIActionAsync(() =>
            {
                var taskMessage = message as TaskStatusMessage;
                ApplicationTask task;
                if (taskMessage == null)
                {
                    task = new ApplicationTask(message.EventType.ToString());
                    if (message.Details != null)
                    {
                        task.Status = message.Details;
                    }
                    if (message.Exception != null)
                    {
                        task.SetError(message.Exception);
                    }
                    else if (message.EventType <= TraceEventType.Error)
                    {
                        task.SetError();
                    }
                    else if (message.EventType == TraceEventType.Warning)
                    {
                        task.SetWarning();
                    }
                    task.SetComplete(message.Message);
                }
                else
                {
                    task = taskMessage.Task;
                }
                this.executingTasksLock.EnterWriteLock();
                try
                {
                    this.ExecutingTasks.Insert(0, new ApplicationTaskViewModel(task));
                }
                finally
                {
                    this.executingTasksLock.ExitWriteLock();
                }
                this.removeTasksTimer.Enabled = true;
                task.StatusChanged += new EventHandler<EventArgs>(OnTaskStatusChanged);
                UpdateStatus();
            });
        }

        #endregion

        #region Event Handlers

        private void OnRemoveTasksTimerElapsed(object sender, ElapsedEventArgs e)
        {
            ApplicationTaskViewModel[] completedTasks = null;
            this.executingTasksLock.EnterReadLock();
            try
            {
                // Remove all tasks that completed more than 30 seconds ago.
                completedTasks = this.ExecutingTasks.Where(t => !t.DetailsVisible && t.Task.TimeCompleted.HasValue && t.Task.TimeCompleted < DateTimeOffset.Now.Subtract(TimeSpan.FromSeconds(30))).ToArray();
            }
            finally
            {
                this.executingTasksLock.ExitReadLock();
            }

            if (completedTasks != null && completedTasks.Length > 0)
            {
                this.ExecuteUIAction(() =>
                {
                    this.executingTasksLock.EnterWriteLock();
                    try
                    {
                        foreach (var task in completedTasks)
                        {
                            this.ExecutingTasks.Remove(task);
                            task.Task.StatusChanged -= new EventHandler<EventArgs>(OnTaskStatusChanged);
                        }
                        this.removeTasksTimer.Enabled = (this.ExecutingTasks.Count > 0);
                    }
                    finally
                    {
                        this.executingTasksLock.ExitWriteLock();
                    }
                });
            }
        }

        private void OnTaskStatusChanged(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        #endregion

        #region Helper Methods

        private void UpdateStatus()
        {
            this.executingTasksLock.EnterReadLock();
            try
            {
                // A task changed, update the global status.
                var anyTaskStillRunning = !this.ExecutingTasks.All(t => t.Task.IsComplete);
                if (anyTaskStillRunning)
                {
                    // Calculate the total progress.
                    var progressTasks = this.ExecutingTasks.Where(t => t.Task.PercentComplete.HasValue);
                    if (progressTasks.Count() > 0)
                    {
                        // There are tasks with progress, set the total progress value on the task bar.
                        var totalPercentComplete = progressTasks.Sum(t => t.Task.PercentComplete.Value) / progressTasks.Count();
                        var state = (progressTasks.Any(t => t.Task.IsError) ? TaskbarItemProgressState.Error : TaskbarItemProgressState.Normal);
                        var incompleteTaskCount = this.ExecutingTasks.Where(t => !t.Task.IsComplete).Count();
                        SetProgress(state, totalPercentComplete, incompleteTaskCount);
                    }
                    else
                    {
                        // There are tasks but they are not reporting their progress, set an indeterminate state.
                        SetProgress(TaskbarItemProgressState.Indeterminate);
                    }
                }
                else
                {
                    SetProgress(TaskbarItemProgressState.None);
                }
            }
            finally
            {
                this.executingTasksLock.ExitReadLock();
            }
        }

        private void SetProgress(TaskbarItemProgressState state)
        {
            SetProgress(state, null, null);
        }

        private void SetProgress(TaskbarItemProgressState state, double? percentComplete, int? incompleteTaskCount)
        {
            this.ExecuteUIActionAsync(() =>
            {
                if (percentComplete.HasValue && incompleteTaskCount.HasValue)
                {
                    var taskCountMessage = incompleteTaskCount.Value.ToCountString("task");
                    var progressMessage = string.Format(CultureInfo.CurrentCulture, "{0} - Executing {1} ({2} complete)", MainWindowViewModel.MainWindowTitleProperty.DefaultValue, taskCountMessage, percentComplete.Value.ToPercentageString());

                    ViewModelLocator.Instance.MainWindowViewModel.MainWindowTitle = progressMessage;
                    ViewModelLocator.Instance.MainWindowViewModel.TaskbarItemInfo.ProgressValue = percentComplete.Value;
                }
                else
                {
                    ViewModelLocator.Instance.MainWindowViewModel.MainWindowTitle = MainWindowViewModel.MainWindowTitleProperty.DefaultValue;
                }
                ViewModelLocator.Instance.MainWindowViewModel.TaskbarItemInfo.ProgressState = state;
            });
        }

        #endregion
    }
}