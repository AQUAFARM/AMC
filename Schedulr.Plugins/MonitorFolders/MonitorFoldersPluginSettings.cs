using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.MonitorFolders
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class MonitorFoldersPluginSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the folders to monitor.
        /// </summary>
        [DataMember]
        public ObservableCollection<string> FoldersToMonitor
        {
            get { return this.GetValue(FoldersToMonitorProperty); }
            set { this.SetValue(FoldersToMonitorProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="FoldersToMonitor"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<string>> FoldersToMonitorProperty = new ObservableProperty<ObservableCollection<string>, MonitorFoldersPluginSettings>(o => o.FoldersToMonitor);

        /// <summary>
        /// Gets or sets the search pattern.
        /// </summary>
        [DataMember]
        [DefaultValue("*.*")]
        public string SearchPattern
        {
            get { return this.GetValue(SearchPatternProperty); }
            set { this.SetValue(SearchPatternProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="SearchPattern"/> observable property.
        /// </summary>
        public static ObservableProperty<string> SearchPatternProperty = new ObservableProperty<string, MonitorFoldersPluginSettings>(o => o.SearchPattern, "*.*");

        /// <summary>
        /// Gets or sets a value that determines if the search is recursive.
        /// </summary>
        [DataMember]
        [DefaultValue(true)]
        public bool SearchRecursive
        {
            get { return this.GetValue(SearchRecursiveProperty); }
            set { this.SetValue(SearchRecursiveProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="SearchRecursive"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> SearchRecursiveProperty = new ObservableProperty<bool, MonitorFoldersPluginSettings>(o => o.SearchRecursive, true);

        /// <summary>
        /// Gets or sets the way that batches are created when adding files.
        /// </summary>
        [DataMember]
        public BatchMode BatchMode
        {
            get { return this.GetValue(BatchModeProperty); }
            set { this.SetValue(BatchModeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="BatchMode"/> observable property.
        /// </summary>
        public static ObservableProperty<BatchMode> BatchModeProperty = new ObservableProperty<BatchMode, MonitorFoldersPluginSettings>(o => o.BatchMode);

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        [DataMember]
        public SearchMode SearchMode
        {
            get { return this.GetValue(SearchModeProperty); }
            set { this.SetValue(SearchModeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="SearchMode"/> observable property.
        /// </summary>
        public static ObservableProperty<SearchMode> SearchModeProperty = new ObservableProperty<SearchMode, MonitorFoldersPluginSettings>(o => o.SearchMode);

        /// <summary>
        /// Gets or sets the search pattern for files to exclude.
        /// </summary>
        [DataMember]
        public string ExcludePattern
        {
            get { return this.GetValue(ExcludePatternProperty); }
            set { this.SetValue(ExcludePatternProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ExcludePattern"/> observable property.
        /// </summary>
        public static ObservableProperty<string> ExcludePatternProperty = new ObservableProperty<string, MonitorFoldersPluginSettings>(o => o.ExcludePattern, "*.xmp;*.thm;*.dng;*.nef;*.raw;thumbs.db");

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorFoldersPluginSettings"/> class.
        /// </summary>
        public MonitorFoldersPluginSettings()
        {
            this.FoldersToMonitor = new ObservableCollection<string>();
        }

        #endregion
    }
}