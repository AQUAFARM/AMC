using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using JelleDruyts.Windows;
using Microsoft.Win32;
using Schedulr.Extensibility;
using Schedulr.Infrastructure;
using Schedulr.Messages;
using Schedulr.Models;
using Schedulr.Providers;
using Schedulr.Views;

namespace Schedulr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constants & Static Properties

        /// <summary>
        /// Gets the default title for the main window.
        /// </summary>
        public const string MainWindowTitle = Constants.ApplicationName;

        /// <summary>
        /// Gets the display version of the application.
        /// </summary>
        public static string DisplayVersion = "v" + FullVersion.ToString(2);

        /// <summary>
        /// Gets the full technical version of the application.
        /// </summary>
        public static Version FullVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        /// <summary>
        /// Gets the singleton <see cref="App"/> instance.
        /// </summary>
        public static App Instance { get { return (App)Application.Current; } }

        /// <summary>
        /// The known file extensions for video files.
        /// </summary>
        private static string VideoFileExtensions { get; set; }

        /// <summary>
        /// The application info.
        /// </summary>
        public static ApplicationInfo Info { get; private set; }

        #endregion

        #region Properties

        public Program Program { get; private set; }
        public RelayCommand OpenLogFileCommand { get; private set; }
        public RelayCommand SaveChangesCommand { get; private set; }
        public RelayCommand UndoChangesCommand { get; private set; }
        public RelayCommand ExportConfigurationCommand { get; private set; }
        public RelayCommand ImportConfigurationCommand { get; private set; }

        #endregion

        #region Static Constructor

        /// <summary>
        /// Initializes the <see cref="App"/> class.
        /// </summary>
        static App()
        {
            string videoFileExtensionsSetting = ConfigurationManager.AppSettings["VideoFileExtensions"];
            if (!string.IsNullOrEmpty(videoFileExtensionsSetting))
            {
                VideoFileExtensions = videoFileExtensionsSetting;
            }
            else
            {
                VideoFileExtensions = string.Empty;
            }
            var mainAssembly = Assembly.GetExecutingAssembly();
            Info = new ApplicationInfo(mainAssembly.GetName().Name, mainAssembly.Location, PathProvider.ConfigurationFilePath);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">More than one instance of the <see cref="T:System.Windows.Application"/> class is created per <see cref="T:System.AppDomain"/>.</exception>
        public App()
        {
            this.OpenLogFileCommand = new RelayCommand(OpenLogFile, CanOpenLogFile, "Open _Log File", "Opens the file that contains the activity log for this application [CTRL-L]", new KeyGesture(Key.L, ModifierKeys.Control));
            this.SaveChangesCommand = new RelayCommand(SaveChanges, CanSaveChanges, "_Save All Changes", "Saves the current configuration. This is done automatically when closing the application, but it can be safer to save once in a while or before trying some things out [CTRL-S]", new KeyGesture(Key.S, ModifierKeys.Control));
            this.UndoChangesCommand = new RelayCommand(UndoChanges, CanUndoChanges, "_Undo All Changes", "Reloads the last saved configuration. Since the application automatically saves all changes when it closes, this allows you to undo changes before closing [CTRL-Z]", new KeyGesture(Key.Z, ModifierKeys.Control));
            this.ExportConfigurationCommand = new RelayCommand(ExportConfiguration, CanExportConfiguration, "E_xport Configuration...", "Exports the current configuration to a file [CTRL-X]", new KeyGesture(Key.X, ModifierKeys.Control));
            this.ImportConfigurationCommand = new RelayCommand(ImportConfiguration, CanImportConfiguration, "_Import Configuration...", "Imports a configuration file (this will overwrite all your changes) [CTRL-I]", new KeyGesture(Key.I, ModifierKeys.Control));
            this.InitializeComponent();
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes the application.
        /// </summary>
        /// <param name="program">The main program.</param>
        /// <param name="selectedAccount">The selected account from the command line arguments.</param>
        /// <param name="uploadUIRequested">Determines if the upload confirmation dialog was requested.</param>
        public void Initialize(Program program, Account selectedAccount, bool uploadUIRequested)
        {
            // Ensure that the current culture is used for all controls (see http://www.west-wind.com/Weblog/posts/796725.aspx).
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            this.Program = program;

            // Create and show the main window.
            var main = new MainWindow();
            main.ContentRendered += delegate
            {
                Logger.Log("App - Content rendered", TraceEventType.Verbose);
                var loadingTask = new ApplicationTask("Loading configuration");
                try
                {
                    // Set configuration.
                    Messenger.Send<StatusMessage>(new TaskStatusMessage(loadingTask));
                    SendConfigurationChangedMessage();

                    // Show the upload confirmation dialog if requested.
                    if (uploadUIRequested && selectedAccount != null)
                    {
                        Messenger.Send<UploadPicturesRequestedMessage>(new UploadPicturesRequestedMessage(selectedAccount.QueuedBatches.FirstOrDefault(), selectedAccount, UploadPicturesRequestReason.CommandLineUI));
                    }
                }
                finally
                {
                    loadingTask.SetComplete(this.Program.Configuration.Accounts.Count.ToCountString("account", null, " loaded"));
                }
            };
            main.Closing += delegate(object sender, CancelEventArgs args)
            {
                // Send closing message to allow cancellation.
                Logger.Log("App - Main window closing", TraceEventType.Verbose);
                var message = new ApplicationClosingMessage();
                Messenger.Send<ApplicationClosingMessage>(message);
                args.Cancel = message.Cancel;
            };
            main.Closed += delegate
            {
                // Close.
                Messenger.Send<ApplicationClosedMessage>(new ApplicationClosedMessage());
                Logger.Log("App - Main window closed", TraceEventType.Verbose);
            };
            Logger.Log("App - Main window created", TraceEventType.Verbose);
            Messenger.Send<ApplicationLoadedMessage>(new ApplicationLoadedMessage());
            Logger.Log("App - ApplicationLoaded message sent", TraceEventType.Verbose);
            main.Show();
            Logger.Log("App - Main window shown", TraceEventType.Verbose);
        }

        #endregion

        #region Commands

        private bool CanOpenLogFile(object parameter)
        {
            return (File.Exists(PathProvider.LogFilePath));
        }

        private void OpenLogFile(object parameter)
        {
            Process.Start(PathProvider.LogFilePath);
        }

        private bool CanSaveChanges(object parameter)
        {
            return true;
        }

        private void SaveChanges(object parameter)
        {
            PluginManager.OnConfigurationEvent(new ConfigurationEventArgs(ConfigurationEventType.Saving, App.Info, this.Program.Configuration, App.Info.ConfigurationFileName, false));
            Messenger.Send<StatusMessage>(new StatusMessage("All changes are saved"));
            this.Program.SaveConfiguration();
            PluginManager.OnConfigurationEvent(new ConfigurationEventArgs(ConfigurationEventType.Saved, App.Info, this.Program.Configuration, App.Info.ConfigurationFileName, false));
        }

        private bool CanUndoChanges(object parameter)
        {
            return true;
        }

        private void UndoChanges(object parameter)
        {
            var result = MessageBox.Show(App.Current.MainWindow, "Are you sure you want to undo all your changes?", "Undo changes?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Messenger.Send<StatusMessage>(new StatusMessage("All changes are undone"));
                var isNewConfiguration = this.Program.LoadConfiguration();
                SendConfigurationChangedMessage();
            }
        }

        private bool CanExportConfiguration(object parameter)
        {
            return true;
        }

        private void ExportConfiguration(object parameter)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Please select the configuration file to export to";
            saveFileDialog.Filter = "XML files|*.xml|All files|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var fileName = saveFileDialog.FileName;
                    PluginManager.OnConfigurationEvent(new ConfigurationEventArgs(ConfigurationEventType.Exporting, App.Info, this.Program.Configuration, fileName, false));
                    Messenger.Send<StatusMessage>(new StatusMessage(string.Format(CultureInfo.CurrentCulture, "Configuration exported to \"{0}\"", Path.GetFileName(fileName))));
                    this.Program.SaveConfiguration(fileName);
                    PluginManager.OnConfigurationEvent(new ConfigurationEventArgs(ConfigurationEventType.Exported, App.Info, this.Program.Configuration, fileName, false));
                }
                catch (Exception)
                {
                    MessageBox.Show(App.Current.MainWindow, string.Format(CultureInfo.CurrentCulture, "The specified configuration file could not be exported.{0}{0}Please see the log file for more details.", Environment.NewLine), "Error exporting configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private bool CanImportConfiguration(object parameter)
        {
            return true;
        }

        private void ImportConfiguration(object parameter)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Please select the configuration file to import";
            openFileDialog.Filter = "XML files|*.xml|All files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var fileName = openFileDialog.FileName;
                    Messenger.Send<StatusMessage>(new StatusMessage(string.Format(CultureInfo.CurrentCulture, "Configuration imported from \"{0}\"", Path.GetFileName(fileName))));
                    this.Program.LoadConfiguration(fileName);
                    SendConfigurationChangedMessage();
                }
                catch (Exception)
                {
                    MessageBox.Show(App.Current.MainWindow, string.Format(CultureInfo.CurrentCulture, "The specified configuration file could not be imported.{0}{0}Please see the log file for more details.", Environment.NewLine), "Error importing configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        #endregion

        #region Helper Methods

        private void SendConfigurationChangedMessage()
        {
            Messenger.Send<ConfigurationChangedMessage>(new ConfigurationChangedMessage(this.Program.Configuration));
            Logger.Log("App - Configuration changed message sent", TraceEventType.Verbose);
        }

        /// <summary>
        /// Gets the OPC pack URL to a relative application resource.
        /// </summary>
        /// <param name="relativeResourcePath">The relative path to the embedded resource.</param>
        /// <returns>The OPC pack URL to the resource.</returns>
        internal static string GetResourceUrl(string relativeResourcePath)
        {
            return "pack://application:,,," + relativeResourcePath;
        }

        internal static bool IsVideoFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            // Flickr officially supports JPEGs, non-animated GIFs, and PNGs.
            // You can also upload TIFFs and some other file types, but they will automatically be converted to and stored in JPEG format.

            // Flickr accepts the following video file formats:
            //     AVI (Proprietary codecs may not work)
            //     WMV
            //     MOV (AVID or other proprietary codecs may not work)
            //     MPEG (1, 2, and 4)
            //     3gp
            //     M2TS
            //     OGG
            //     OGV

            // Check if the file extension is any of the known video file extensions.
            var extension = Path.GetExtension(fileName);

            // Remove the '.' in front of the file extension.
            if (!string.IsNullOrEmpty(extension) && extension.StartsWith(".", StringComparison.Ordinal))
            {
                extension = extension.Substring(1);
            }
            return App.VideoFileExtensions.IndexOf(extension, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        #endregion
    }
}