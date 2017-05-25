using System.ComponentModel;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.RunRenderingProgram
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class RunRenderingProgramPluginSettings : ObservableObject
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
        public static ObservableProperty<string> FileNameProperty = new ObservableProperty<string, RunRenderingProgramPluginSettings>(o => o.FileName);

        /// <summary>
        /// Gets or sets the command-line arguments for the program.
        /// </summary>
        [DataMember]
        [DefaultValue("\"$(RenderingInputFile)\" \"$(RenderingOutputFile)\"")]
        public string Arguments
        {
            get { return this.GetValue(ArgumentsProperty); }
            set { this.SetValue(ArgumentsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Arguments"/> observable property.
        /// </summary>
        public static ObservableProperty<string> ArgumentsProperty = new ObservableProperty<string, RunRenderingProgramPluginSettings>(o => o.Arguments, "\"$(RenderingInputFile)\" \"$(RenderingOutputFile)\"");

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
        public static ObservableProperty<string> WorkingDirectoryProperty = new ObservableProperty<string, RunRenderingProgramPluginSettings>(o => o.WorkingDirectory);

        /// <summary>
        /// Gets or sets a value that determines if the program should be run on pictures.
        /// </summary>
        [DataMember]
        [DefaultValue(true)]
        public bool RunOnPictures
        {
            get { return this.GetValue(RunOnPicturesProperty); }
            set { this.SetValue(RunOnPicturesProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="RunOnPictures"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> RunOnPicturesProperty = new ObservableProperty<bool, RunRenderingProgramPluginSettings>(o => o.RunOnPictures, true);

        /// <summary>
        /// Gets or sets a value that determines if the program should be run on videos.
        /// </summary>
        [DataMember]
        [DefaultValue(true)]
        public bool RunOnVideos
        {
            get { return this.GetValue(RunOnVideosProperty); }
            set { this.SetValue(RunOnVideosProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="RunOnVideos"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> RunOnVideosProperty = new ObservableProperty<bool, RunRenderingProgramPluginSettings>(o => o.RunOnVideos, true);

        /// <summary>
        /// Gets or sets the file extension of the temporary input file.
        /// </summary>
        [DataMember]
        public string InputFileExtension
        {
            get { return this.GetValue(InputFileExtensionProperty); }
            set { this.SetValue(InputFileExtensionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="InputFileExtension"/> observable property.
        /// </summary>
        public static ObservableProperty<string> InputFileExtensionProperty = new ObservableProperty<string, RunRenderingProgramPluginSettings>(o => o.InputFileExtension);

        /// <summary>
        /// Gets or sets the file extension of the temporary output file.
        /// </summary>
        [DataMember]
        public string OutputFileExtension
        {
            get { return this.GetValue(OutputFileExtensionProperty); }
            set { this.SetValue(OutputFileExtensionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="OutputFileExtension"/> observable property.
        /// </summary>
        public static ObservableProperty<string> OutputFileExtensionProperty = new ObservableProperty<string, RunRenderingProgramPluginSettings>(o => o.OutputFileExtension);

        #endregion
    }
}