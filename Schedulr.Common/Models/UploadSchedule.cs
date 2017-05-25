using System;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// The upload schedule for an account.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class UploadSchedule : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the schedule type.
        /// </summary>
        [DataMember]
        public ScheduleType Type
        {
            get { return this.GetValue(TypeProperty); }
            set { this.SetValue(TypeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Type"/> observable property.
        /// </summary>
        public static ObservableProperty<ScheduleType> TypeProperty = new ObservableProperty<ScheduleType, UploadSchedule>(o => o.Type, ScheduleType.Daily);

        /// <summary>
        /// Gets or sets the start time of the scheduled task.
        /// </summary>
        [DataMember]
        public DateTime StartTime
        {
            get { return this.GetValue(StartTimeProperty); }
            set { this.SetValue(StartTimeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="StartTime"/> observable property.
        /// </summary>
        public static ObservableProperty<DateTime> StartTimeProperty = new ObservableProperty<DateTime, UploadSchedule>(o => o.StartTime);

        /// <summary>
        /// Gets or sets a value that determines if the scheduled task can only be run if the user is logged on.
        /// </summary>
        [DataMember]
        public bool OnlyIfLoggedOn
        {
            get { return this.GetValue(OnlyIfLoggedOnProperty); }
            set { this.SetValue(OnlyIfLoggedOnProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="OnlyIfLoggedOn"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> OnlyIfLoggedOnProperty = new ObservableProperty<bool, UploadSchedule>(o => o.OnlyIfLoggedOn, true);

        /// <summary>
        /// Gets or sets a value that determines if the computer can be woken to run the scheduled task.
        /// </summary>
        [DataMember]
        public bool WakeComputer
        {
            get { return this.GetValue(WakeComputerProperty); }
            set { this.SetValue(WakeComputerProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WakeComputer"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> WakeComputerProperty = new ObservableProperty<bool, UploadSchedule>(o => o.WakeComputer);

        /// <summary>
        /// Gets or sets a value that determines if the scheduled task can only be run if the computer is not on battery power.
        /// </summary>
        [DataMember]
        public bool OnlyIfNotOnBatteryPower
        {
            get { return this.GetValue(OnlyIfNotOnBatteryPowerProperty); }
            set { this.SetValue(OnlyIfNotOnBatteryPowerProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="OnlyIfNotOnBatteryPower"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> OnlyIfNotOnBatteryPowerProperty = new ObservableProperty<bool, UploadSchedule>(o => o.OnlyIfNotOnBatteryPower, true);

        /// <summary>
        /// Gets or sets the hourly repetition interval.
        /// </summary>
        [DataMember]
        public int HourlyInterval
        {
            get { return this.GetValue(HourlyIntervalProperty); }
            set { this.SetValue(HourlyIntervalProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="HourlyInterval"/> observable property.
        /// </summary>
        public static ObservableProperty<int> HourlyIntervalProperty = new ObservableProperty<int, UploadSchedule>(o => o.HourlyInterval);

        /// <summary>
        /// Gets or sets the duration of the hourly repetition interval.
        /// </summary>
        [DataMember]
        public int HourlyIntervalDuration
        {
            get { return this.GetValue(HourlyIntervalDurationProperty); }
            set { this.SetValue(HourlyIntervalDurationProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="HourlyIntervalDuration"/> observable property.
        /// </summary>
        public static ObservableProperty<int> HourlyIntervalDurationProperty = new ObservableProperty<int, UploadSchedule>(o => o.HourlyIntervalDuration, 24);

        /// <summary>
        /// Gets or sets the daily repetition interval (if the <see cref="Type"/> is <see cref="ScheduleType.Daily"/>).
        /// </summary>
        [DataMember]
        public int DailyInterval
        {
            get { return this.GetValue(DailyIntervalProperty); }
            set { this.SetValue(DailyIntervalProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="DailyInterval"/> observable property.
        /// </summary>
        public static ObservableProperty<int> DailyIntervalProperty = new ObservableProperty<int, UploadSchedule>(o => o.DailyInterval, 1);

        /// <summary>
        /// Gets or sets the weekly repetition interval (if the <see cref="Type"/> is <see cref="ScheduleType.Weekly"/>).
        /// </summary>
        [DataMember]
        public int WeeklyInterval
        {
            get { return this.GetValue(WeeklyIntervalProperty); }
            set { this.SetValue(WeeklyIntervalProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WeeklyInterval"/> observable property.
        /// </summary>
        public static ObservableProperty<int> WeeklyIntervalProperty = new ObservableProperty<int, UploadSchedule>(o => o.WeeklyInterval, 1);

        /// <summary>
        /// Gets or sets a value that determines if the scheduled task will run on Monday (if the <see cref="Type"/> is <see cref="ScheduleType.Weekly"/>).
        /// </summary>
        [DataMember]
        public bool WeeklyOnMonday
        {
            get { return this.GetValue(WeeklyOnMondayProperty); }
            set { this.SetValue(WeeklyOnMondayProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WeeklyOnMonday"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> WeeklyOnMondayProperty = new ObservableProperty<bool, UploadSchedule>(o => o.WeeklyOnMonday);

        /// <summary>
        /// Gets or sets a value that determines if the scheduled task will run on Tuesday (if the <see cref="Type"/> is <see cref="ScheduleType.Weekly"/>).
        /// </summary>
        [DataMember]
        public bool WeeklyOnTuesday
        {
            get { return this.GetValue(WeeklyOnTuesdayProperty); }
            set { this.SetValue(WeeklyOnTuesdayProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WeeklyOnTuesday"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> WeeklyOnTuesdayProperty = new ObservableProperty<bool, UploadSchedule>(o => o.WeeklyOnTuesday);

        /// <summary>
        /// Gets or sets a value that determines if the scheduled task will run on Wednesday (if the <see cref="Type"/> is <see cref="ScheduleType.Weekly"/>).
        /// </summary>
        [DataMember]
        public bool WeeklyOnWednesday
        {
            get { return this.GetValue(WeeklyOnWednesdayProperty); }
            set { this.SetValue(WeeklyOnWednesdayProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WeeklyOnWednesday"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> WeeklyOnWednesdayProperty = new ObservableProperty<bool, UploadSchedule>(o => o.WeeklyOnWednesday);

        /// <summary>
        /// Gets or sets a value that determines if the scheduled task will run on Thursday (if the <see cref="Type"/> is <see cref="ScheduleType.Weekly"/>).
        /// </summary>
        [DataMember]
        public bool WeeklyOnThursday
        {
            get { return this.GetValue(WeeklyOnThursdayProperty); }
            set { this.SetValue(WeeklyOnThursdayProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WeeklyOnThursday"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> WeeklyOnThursdayProperty = new ObservableProperty<bool, UploadSchedule>(o => o.WeeklyOnThursday);

        /// <summary>
        /// Gets or sets a value that determines if the scheduled task will run on Friday (if the <see cref="Type"/> is <see cref="ScheduleType.Weekly"/>).
        /// </summary>
        [DataMember]
        public bool WeeklyOnFriday
        {
            get { return this.GetValue(WeeklyOnFridayProperty); }
            set { this.SetValue(WeeklyOnFridayProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WeeklyOnFriday"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> WeeklyOnFridayProperty = new ObservableProperty<bool, UploadSchedule>(o => o.WeeklyOnFriday);

        /// <summary>
        /// Gets or sets a value that determines if the scheduled task will run on Saturday (if the <see cref="Type"/> is <see cref="ScheduleType.Weekly"/>).
        /// </summary>
        [DataMember]
        public bool WeeklyOnSaturday
        {
            get { return this.GetValue(WeeklyOnSaturdayProperty); }
            set { this.SetValue(WeeklyOnSaturdayProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WeeklyOnSaturday"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> WeeklyOnSaturdayProperty = new ObservableProperty<bool, UploadSchedule>(o => o.WeeklyOnSaturday);

        /// <summary>
        /// Gets or sets a value that determines if the scheduled task will run on Sunday (if the <see cref="Type"/> is <see cref="ScheduleType.Weekly"/>).
        /// </summary>
        [DataMember]
        public bool WeeklyOnSunday
        {
            get { return this.GetValue(WeeklyOnSundayProperty); }
            set { this.SetValue(WeeklyOnSundayProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WeeklyOnSunday"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> WeeklyOnSundayProperty = new ObservableProperty<bool, UploadSchedule>(o => o.WeeklyOnSunday);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadSchedule"/> class.
        /// </summary>
        public UploadSchedule()
        {
            this.StartTime = DateTime.Now;
        }

        #endregion
    }
}