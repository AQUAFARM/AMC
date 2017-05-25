using System.ComponentModel;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.BackupConfiguration
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class BackupConfigurationPluginSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the name of the backup file.
        /// </summary>
        [DataMember]
        public string BackupFileName
        {
            get { return this.GetValue(BackupFileNameProperty); }
            set { this.SetValue(BackupFileNameProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="BackupFileName"/> observable property.
        /// </summary>
        public static ObservableProperty<string> BackupFileNameProperty = new ObservableProperty<string, BackupConfigurationPluginSettings>(o => o.BackupFileName);

        /// <summary>
        /// Gets or sets the backup strategy.
        /// </summary>
        [DataMember]
        [DefaultValue(BackupStrategy.Overwrite)]
        public BackupStrategy Strategy
        {
            get { return this.GetValue(StrategyProperty); }
            set { this.SetValue(StrategyProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Strategy"/> observable property.
        /// </summary>
        public static ObservableProperty<BackupStrategy> StrategyProperty = new ObservableProperty<BackupStrategy, BackupConfigurationPluginSettings>(o => o.Strategy, BackupStrategy.Overwrite);

        /// <summary>
        /// Gets or sets the number of backup files to keep, or 0 to keep all.
        /// </summary>
        [DataMember]
        public int NumberOfBackupsToKeep
        {
            get { return this.GetValue(NumberOfBackupsToKeepProperty); }
            set { this.SetValue(NumberOfBackupsToKeepProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="NumberOfBackupsToKeep"/> observable property.
        /// </summary>
        public static ObservableProperty<int> NumberOfBackupsToKeepProperty = new ObservableProperty<int, BackupConfigurationPluginSettings>(o => o.NumberOfBackupsToKeep);

        #endregion
    }
}