using System;

namespace Schedulr.Infrastructure
{
    public class ScheduledTaskStatus
    {
        public string TaskName { get; private set; }
        public bool Enabled { get; private set; }
        public DateTime? LastRunTime { get; private set; }
        public int? LastRunResult { get; private set; }
        public DateTime? NextRunTime { get; private set; }
        public string State { get; private set; }

        public ScheduledTaskStatus(string taskName, bool enabled, DateTime? lastRunTime, int? lastRunResult, DateTime? nextRunTime, string state)
        {
            this.TaskName = taskName;
            this.Enabled = enabled;
            this.LastRunTime = lastRunTime;
            this.LastRunResult = lastRunResult;
            this.NextRunTime = nextRunTime;
            this.State = state;
        }
    }
}
