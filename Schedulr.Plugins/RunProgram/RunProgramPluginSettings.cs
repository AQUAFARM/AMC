using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.RunProgram
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class RunProgramPluginSettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the file name of the program to run.
        /// </summary>
        [DataMember]
        public string FileName
        {
            get { return this.GetValue(FileNameProperty); }
            set { this.SetValue(FileNameProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="FileName"/> observable property.
        /// </summary>
        public static ObservableProperty<string> FileNameProperty = new ObservableProperty<string, RunProgramPluginSettings>(o => o.FileName);

        /// <summary>
        /// Gets or sets the command-line arguments for the program.
        /// </summary>
        [DataMember]
        public string Arguments
        {
            get { return this.GetValue(ArgumentsProperty); }
            set { this.SetValue(ArgumentsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Arguments"/> observable property.
        /// </summary>
        public static ObservableProperty<string> ArgumentsProperty = new ObservableProperty<string, RunProgramPluginSettings>(o => o.Arguments);

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        [DataMember]
        public string WorkingDirectory
        {
            get { return this.GetValue(WorkingDirectoryProperty); }
            set { this.SetValue(WorkingDirectoryProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WorkingDirectory"/> observable property.
        /// </summary>
        public static ObservableProperty<string> WorkingDirectoryProperty = new ObservableProperty<string, RunProgramPluginSettings>(o => o.WorkingDirectory);

        /// <summary>
        /// Gets or sets a value that determines if we should wait for the program to exit.
        /// </summary>
        [DataMember]
        public bool WaitForExit
        {
            get { return this.GetValue(WaitForExitProperty); }
            set { this.SetValue(WaitForExitProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WaitForExit"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> WaitForExitProperty = new ObservableProperty<bool, RunProgramPluginSettings>(o => o.WaitForExit);

        /// <summary>
        /// Gets or sets the timeout in seconds to wait for the program to exit (if <see cref="WaitForExit"/> is <see langword="true"/>).
        /// </summary>
        [DataMember]
        public int WaitForExitTimeoutSeconds
        {
            get { return this.GetValue(WaitForExitTimeoutSecondsProperty); }
            set { this.SetValue(WaitForExitTimeoutSecondsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WaitForExitTimeoutSeconds"/> observable property.
        /// </summary>
        public static ObservableProperty<int> WaitForExitTimeoutSecondsProperty = new ObservableProperty<int, RunProgramPluginSettings>(o => o.WaitForExitTimeoutSeconds);

        #endregion
    }
}