using JelleDruyts.Windows;

namespace Schedulr.Messages
{
    public class TaskStatusMessage : StatusMessage
    {
        public ApplicationTask Task { get; private set; }

        public TaskStatusMessage(ApplicationTask task)
            : base(task.Name)
        {
            this.Task = task;
        }
    }
}