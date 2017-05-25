using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;
using JelleDruyts.Windows;
using Schedulr.Extensibility;
using Schedulr.Infrastructure;
using Schedulr.Messages;
using Schedulr.Models;
using Schedulr.Providers;

namespace Schedulr
{
    /// <summary>
    /// The entry point for the application.
    /// </summary>
    public sealed class Program : SingleInstanceApplication
    {
        #region Constants

        /// <summary>
        /// Gets the command-line argument prefix.
        /// </summary>
        public const string ArgumentPrefix = "/";

        /// <summary>
        /// Gets the command-line argument name to request help.
        /// </summary>
        public const string ArgumentNameHelp = ArgumentPrefix + "?";

        /// <summary>
        /// Gets the command-line argument name to request help (alternative).
        /// </summary>
        public const string ArgumentNameHelpAlt = ArgumentPrefix + "help";

        /// <summary>
        /// Gets the command-line argument key name to specify an account.
        /// </summary>
        public const string ArgumentKeyAccount = "account";

        /// <summary>
        /// Gets the command-line argument name to specify an account.
        /// </summary>
        public const string ArgumentNameAccount = ArgumentPrefix + ArgumentKeyAccount;

        /// <summary>
        /// Gets the command-line argument name to start an upload in the background.
        /// </summary>
        public const string ArgumentNameUpload = ArgumentPrefix + "upload";

        /// <summary>
        /// Gets the command-line argument name to start an upload through the user interface.
        /// </summary>
        public const string ArgumentNameUploadUI = ArgumentPrefix + "uploadui";

        /// <summary>
        /// Gets the command-line argument name to add pictures as individual batches.
        /// </summary>
        public const string ArgumentNameAdd = ArgumentPrefix + "add";

        /// <summary>
        /// Gets the command-line argument name to add pictures as a single batch.
        /// </summary>
        public const string ArgumentNameAddBatch = ArgumentPrefix + "addbatch";

        #endregion

        #region Fields

        /// <summary>
        /// A value that determines if the application is running interactively or in the background.
        /// </summary>
        private bool isInteractive;

        /// <summary>
        /// The application instance if it is running interactively.
        /// </summary>
        private App app;

        /// <summary>
        /// The account that was last active.
        /// </summary>
        private Account lastActiveAccount;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configuration being used by the current application.
        /// </summary>
        public SchedulrConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets the account that was requested from the command-line.
        /// </summary>
        public Account RequestedAccount { get; private set; }

        /// <summary>
        /// Gets the full path of the directory where the application files are located.
        /// </summary>
        public static string ApplicationDirectory { get; private set; }

        #endregion

        #region Main

        [STAThread]
        public static void Main(string[] args)
        {
            Logger.Log("Program - Main entering", TraceEventType.Verbose);
            Program.ApplicationDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // Unblock any files that were blocked by Windows because they were downloaded.
            try
            {
                Logger.Log("Program - Unblocking files in " + Program.ApplicationDirectory, TraceEventType.Verbose);
                foreach (var fileName in Directory.GetFiles(Program.ApplicationDirectory))
                {
                    FileSystem.UnblockFile(fileName);
                }
            }
            catch (Exception exc)
            {
                Logger.Log("An exception occurred while attempting to unblock any files blocked by Windows.", exc, TraceEventType.Warning);
            }

            var app = new Program();
            app.IsSystemWideSingleInstance = true;
            app.Run(args);
        }

        #endregion

        #region Startup

        /// <summary>
        /// Called when the first instance of the application is started.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        protected override void OnStartupFirstInstance(IList<string> args)
        {
            // Initialize.
            LogStartup(string.Format(CultureInfo.CurrentCulture, "Application {0} started ({1})", App.DisplayVersion, App.FullVersion.ToString()), args);
            TemplateTokenProvider.SetSampleValue<ApplicationInfo>(App.Info);
            AsyncOperationManager.SynchronizationContext = new DispatcherSynchronizationContext();

            // Watch for account changes.
            Messenger.Register<AccountActionMessage>(OnAccountAction);

            // Load the configuration.
            var isNewConfiguration = LoadConfigurationInternal(null, true);

            // Start the application.
            ApplicationInstanceRequested(args, true);

            // Deactivate the last used account.
            DeactivateAccount(this.lastActiveAccount);

            // Save the configuration.
            PluginManager.OnConfigurationEvent(new ConfigurationEventArgs(ConfigurationEventType.Saving, App.Info, this.Configuration, App.Info.ConfigurationFileName, false));
            SaveConfigurationInternal(null, true);
            PluginManager.OnConfigurationEvent(new ConfigurationEventArgs(ConfigurationEventType.Saved, App.Info, this.Configuration, App.Info.ConfigurationFileName, false));
            Logger.Log("Program - Configuration saved", TraceEventType.Verbose);

            PluginManager.OnApplicationEvent(new ApplicationEventArgs(ApplicationEventType.Closing, App.Info));

            // Unload plugins.
            UnloadApplicationPlugins();
            Logger.Log("Program - Plugins unloaded", TraceEventType.Verbose);

            Logger.Log("Application exited", TraceEventType.Information);
        }

        #endregion

        #region Startup Next Instance

        /// <summary>
        /// Called when a subsequent instance of the application is started.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        protected override void OnStartupNextInstance(IList<string> args)
        {
            //base.OnStartupNextInstance(e);
            LogStartup("A new application instance was requested", args);

            // Activate the main window if available to make bring the application back to the foreground.
            if (this.app != null && this.app.MainWindow != null)
            {
                this.app.MainWindow.Activate();
            }

            // Pass the new instance request to the already running application.
            ApplicationInstanceRequested(args, false);
        }

        #endregion

        #region Exception Handling

        /// <summary>
        /// Called when an unhandled exception has occurred.
        /// </summary>
        /// <param name="exception">The unhandled exception that has occurred.</param>
        protected override void OnUnhandledException(Exception exception)
        {
            // Log the exception and show a message to the user. The application will terminate no matter what.
            Logger.Log("An unhandled exception occurred", exception);
            var message = new StringBuilder();
            message.Append("An unexpected exception occurred and the application will need to close. We apologize for the inconvenience.").AppendLine().AppendLine();
            message.AppendFormat(CultureInfo.CurrentCulture, "Please contact us at {0} and provide as much details as possible so we can fix the issue.", App.Info.Url);
            if (exception != null)
            {
                message.AppendLine().AppendLine();
                message.Append("Exception message (more details are available in the log file):").AppendLine();
                message.Append(exception.Message);
            }
            MessageBox.Show(message.ToString(), "An unexpected exception occurred", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion

        #region Configuration Handling

        /// <summary>
        /// Saves the configuration to the default file location.
        /// </summary>
        public void SaveConfiguration()
        {
            SaveConfigurationInternal(null, false);
            UpdateJumpList();
        }

        /// <summary>
        /// Saves the configuration to the specified file location.
        /// </summary>
        /// <param name="fileName">The name of the file to which to save the configuration.</param>
        public void SaveConfiguration(string fileName)
        {
            SaveConfigurationInternal(fileName, false);
        }

        private void SaveConfigurationInternal(string fileName, bool finalSave)
        {
            // Save plugin configurations.
            if (!finalSave)
            {
                // For the final save, do not save account-level plugins since the account is already deactivated and plugins are unloaded.
                SaveAccountPluginConfigurations(this.lastActiveAccount);
            }
            SaveApplicationPluginConfigurations(this.Configuration);
            Logger.Log("Program - Plugins saved to configuration", TraceEventType.Verbose);

            if (fileName == null)
            {
                SchedulrConfigurationProvider.Save(this.Configuration);
            }
            else
            {
                SchedulrConfigurationProvider.Save(this.Configuration, fileName);
            }
        }

        /// <summary>
        /// Loads the configuration from the default file location, or creates a new configuration instance if the file does not exist or could not be loaded.
        /// </summary>
        /// <returns>A value that determines if the configuration was just created.</returns>
        public bool LoadConfiguration()
        {
            return LoadConfigurationInternal(null, false);
        }

        /// <summary>
        /// Loads the configuration from the specified file location.
        /// </summary>
        /// <param name="fileName">The name of the file that contains the configuration.</param>
        public void LoadConfiguration(string fileName)
        {
            LoadConfigurationInternal(fileName, false);
        }

        /// <summary>
        /// Loads the configuration from the default file location, or creates a new configuration instance if the file does not exist or could not be loaded.
        /// </summary>
        /// <param name="fileName">The name of the file that contains the configuration.</param>
        /// <param name="initialLoad">A value that determines if this is the initial load on application startup.</param>
        /// <returns>A value that determines if the configuration was just created.</returns>
        private bool LoadConfigurationInternal(string fileName, bool initialLoad)
        {
            Logger.Log("Program - Loading configuration", TraceEventType.Verbose);

            // Deactivate current account and unload plugins.
            DeactivateAccount(this.lastActiveAccount);
            UnloadApplicationPlugins();
            Logger.Log("Program - Plugins unloaded (if any)", TraceEventType.Verbose);

            // Load the configuration.
            var isNewConfiguration = false;
            if (fileName == null)
            {
                fileName = App.Info.ConfigurationFileName;
                this.Configuration = SchedulrConfigurationProvider.LoadOrCreate(out isNewConfiguration);
            }
            else
            {
                this.Configuration = SchedulrConfigurationProvider.Load(fileName);
            }
            TemplateTokenProvider.SetSampleValue<SchedulrConfiguration>(this.Configuration);

            // Initialize plugins from configuration.
            InitializePlugins(this.Configuration);
            LoadApplicationPlugins(this.Configuration);
            Logger.Log("Program - Plugins loaded", TraceEventType.Verbose);

            if (initialLoad)
            {
                // For the initial load, raise the Application Started event before the Configuration Loaded event.
                PluginManager.OnApplicationEvent(new ApplicationEventArgs(ApplicationEventType.Started, App.Info));
            }

            PluginManager.OnConfigurationEvent(new ConfigurationEventArgs(ConfigurationEventType.Loaded, App.Info, this.Configuration, fileName, false));

            Logger.Log("Program - Configuration loaded", TraceEventType.Verbose);
            return isNewConfiguration;
        }

        #endregion

        #region Active Account Handling

        private void ActivateAccount(Account account)
        {
            if (this.lastActiveAccount != account)
            {
                DeactivateAccount(this.lastActiveAccount);
                this.lastActiveAccount = account;
                TemplateTokenProvider.SetSampleValue<Account>(account);
                if (account == null)
                {
                    TemplateTokenProvider.SetSampleValue<Batch>(null);
                    TemplateTokenProvider.SetSampleValue<Picture>(null);
                    TemplateTokenProvider.SetSampleValue<UploadSchedule>(null);
                }
                else
                {
                    LoadAccountPlugins(account);
                    TemplateTokenProvider.SetSampleValue<Batch>(account.QueuedBatches.FirstOrDefault() ?? account.UploadedBatches.FirstOrDefault());
                    TemplateTokenProvider.SetSampleValue<Picture>(account.QueuedBatches.Union(account.UploadedBatches).SelectMany(b => b.Pictures).FirstOrDefault());
                    TemplateTokenProvider.SetSampleValue<UploadSchedule>(account.UploadSchedule);
                    PluginManager.OnAccountEvent(new AccountEventArgs(AccountEventType.Activated, App.Info, account));
                    Logger.Log("Program - Account activated: " + account.Name, TraceEventType.Verbose);
                }
            }
        }

        private void DeactivateAccount(Account account)
        {
            if (account != null)
            {
                PluginManager.OnAccountEvent(new AccountEventArgs(AccountEventType.Deactivated, App.Info, account));
                SaveAccountPluginConfigurations(account);
                UnloadAccountPlugins();
                Logger.Log("Program - Account deactivated: " + account.Name, TraceEventType.Verbose);
            }
        }

        #endregion

        #region Plugin Handling

        private static void InitializePlugins(SchedulrConfiguration configuration)
        {
            PluginManager.SetPluginCollectionSettings(configuration.PluginCollectionSettings);
        }

        private static void LoadApplicationPlugins(SchedulrConfiguration configuration)
        {
            PluginManager.LoadPlugins(configuration.Plugins);
        }

        private static void SaveApplicationPluginConfigurations(SchedulrConfiguration configuration)
        {
            configuration.Plugins.ReplaceItems(PluginManager.GetLoadedPlugins(PluginScope.Application));
        }

        private static void UnloadApplicationPlugins()
        {
            PluginManager.UnloadPlugins(PluginScope.Application);
        }

        private static void LoadAccountPlugins(Account account)
        {
            if (account != null)
            {
                PluginManager.LoadPlugins(account.Plugins);
            }
        }

        private static void SaveAccountPluginConfigurations(Account account)
        {
            if (account != null)
            {
                account.Plugins.ReplaceItems(PluginManager.GetLoadedPlugins(PluginScope.Account));
            }
        }

        private static void UnloadAccountPlugins()
        {
            PluginManager.UnloadPlugins(PluginScope.Account);
        }

        private void OnAccountAction(AccountActionMessage message)
        {
            if (message.Action == ListAction.CurrentChanged)
            {
                ActivateAccount(message.Account);
            }
        }

        #endregion

        #region Application Instance Requested

        /// <summary>
        /// Handles a new application instance request.
        /// </summary>
        /// <param name="arguments">The command-line arguments.</param>
        /// <param name="firstInstance">Determines if this is the first instance or if the application is already running.</param>
        private void ApplicationInstanceRequested(IList<string> arguments, bool firstInstance)
        {
            CommandLineActions requestedActions;
            Account selectedAccount;
            IList<string> fileNames;
            var shouldContinue = ParseArguments(arguments, firstInstance, out selectedAccount, out requestedActions, out fileNames);
            if (!shouldContinue)
            {
                return;
            }

            this.RequestedAccount = selectedAccount;
            ActivateAccount(selectedAccount);

            if (requestedActions.HasFlag(CommandLineActions.Add) || requestedActions.HasFlag(CommandLineActions.AddBatch))
            {
                if (fileNames.Count == 0)
                {
                    Logger.Log("Files were requested to be added but there were no files to add", TraceEventType.Warning);
                }
                else
                {
                    Logger.Log("Add files requested for account " + selectedAccount.Name, TraceEventType.Information);
                    var addToSingleBatch = requestedActions.HasFlag(CommandLineActions.AddBatch);
                    Tasks.AddPicturesToQueue(selectedAccount, fileNames, null, addToSingleBatch);
                }
            }

            if (firstInstance)
            {
                if (requestedActions.HasFlag(CommandLineActions.UploadBackground))
                {
                    Logger.Log("Upload requested for account " + selectedAccount.Name, TraceEventType.Information);

                    // Perform the upload in the background.
                    this.isInteractive = false;

                    // Upload the batch and at the end save changes to the configuration file.
                    var tempApp = new Application();
                    var uploading = Tasks.UploadBatch(selectedAccount, selectedAccount.QueuedBatches.FirstOrDefault(), () =>
                    {
                        tempApp.Dispatcher.InvokeShutdown();
                    });

                    // In the mean time run a new application so the message pump is running and
                    // new application instance requests can be processed while uploading.
                    if (uploading)
                    {
                        tempApp.Run();
                    }
                }
                else
                {
                    // Start the application interactively.
                    this.isInteractive = true;
                    Logger.Log("Program - Creating app", TraceEventType.Verbose);
                    this.app = new App();
                    Logger.Log("Program - Updating Windows Jump List", TraceEventType.Verbose);
                    UpdateJumpList();
                    Logger.Log("Program - Initializing app", TraceEventType.Verbose);
                    this.app.Initialize(this, selectedAccount, requestedActions.HasFlag(CommandLineActions.UploadUI));
                    Logger.Log("Program - Running app", TraceEventType.Verbose);
                    this.app.Run();
                    Logger.Log("Program - Closing app", TraceEventType.Verbose);
                }
            }
            else
            {
                if (this.isInteractive)
                {
                    // The application is already running interactively, perform the requested action or do nothing if the application was started normally.
                    if (requestedActions.HasFlag(CommandLineActions.UploadBackground) || requestedActions.HasFlag(CommandLineActions.UploadUI))
                    {
                        Logger.Log("Upload requested for account " + selectedAccount.Name, TraceEventType.Information);
                        var reason = (requestedActions.HasFlag(CommandLineActions.UploadBackground) ? UploadPicturesRequestReason.CommandLineBackground : UploadPicturesRequestReason.CommandLineUI);
                        Messenger.Send<UploadPicturesRequestedMessage>(new UploadPicturesRequestedMessage(selectedAccount.QueuedBatches.FirstOrDefault(), selectedAccount, reason));
                    }
                }
                else
                {
                    // An upload is already running; show a message box and exit.
                    var message = string.Format(CultureInfo.CurrentCulture, "{1} is currently uploading files in the background so you cannot start the application at this time.{0}{0}Please try again after the upload has completed. You can monitor progress by looking at the log file at the following location:{0}{0}{2}", Environment.NewLine, App.Info.Name, PathProvider.LogFilePath);
                    MessageBox.Show(message, App.Info.Name, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Parses the command-line arguments.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="firstInstance">Determines if this is the first instance of the application.</param>
        /// <param name="selectedAccount">Upon return, contains the selected account.</param>
        /// <param name="requestedActions">Upon return, contains the requested command-line actions.</param>
        /// <param name="fileNames">Upon return, contains any file names passed for the requested actions.</param>
        /// <returns><see langword="true"/> if the command-line arguments were valid and the application should continue, <see langword="false"/> otherwise.</returns>
        private bool ParseArguments(IList<string> arguments, bool firstInstance, out Account selectedAccount, out CommandLineActions requestedActions, out IList<string> fileNames)
        {
            // Parse the command-line arguments.
            IDictionary<string, string> nameValueArguments;
            IList<string> valueArguments;
            ParseArguments(arguments, out nameValueArguments, out valueArguments);

            // Determine the selected account to use.
            if (nameValueArguments.ContainsKey(Program.ArgumentKeyAccount.ToLowerInvariant()))
            {
                // An account name was specified, look up the account in the configuration.
                var accountName = GetArgument(nameValueArguments, Program.ArgumentKeyAccount);
                selectedAccount = this.Configuration.Accounts.Where(a => string.Equals(a.Name, accountName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            }
            else
            {
                // No account was specified, use the default account.
                selectedAccount = this.Configuration.Accounts.GetDefaultAccount();
                if (selectedAccount == null && this.Configuration.Accounts.Count > 0)
                {
                    selectedAccount = this.Configuration.Accounts[0];
                }
            }

            // Determine the available file names.
            fileNames = new List<string>();
            foreach (var potentialFileName in valueArguments)
            {
                if (File.Exists(potentialFileName))
                {
                    fileNames.Add(potentialFileName);
                }
            }

            // Determine the actions to execute.
            var shouldContinue = true;
            var showHelp = false;
            string errorMessage = null;
            requestedActions = CommandLineActions.None;
            if (valueArguments.Contains(Program.ArgumentNameHelp, StringComparer.OrdinalIgnoreCase) || valueArguments.Contains(Program.ArgumentNameHelpAlt, StringComparer.OrdinalIgnoreCase))
            {
                showHelp = true;
            }
            else
            {
                if (valueArguments.Contains(Program.ArgumentNameUpload, StringComparer.OrdinalIgnoreCase))
                {
                    // An upload is requested, check the account.
                    requestedActions |= CommandLineActions.UploadBackground;
                    if (selectedAccount == null)
                    {
                        showHelp = true;
                        errorMessage = "An upload was requested but the specified account was not found.";
                    }
                    else
                    {
                        if (selectedAccount.QueuedBatches.Count == 0)
                        {
                            Logger.Log("An upload was requested but the account had no files to upload.", TraceEventType.Information);
                            shouldContinue = false;
                        }
                    }
                }
                if (valueArguments.Contains(Program.ArgumentNameUploadUI, StringComparer.OrdinalIgnoreCase) && selectedAccount != null)
                {
                    requestedActions |= CommandLineActions.UploadUI;
                }
                if (valueArguments.Contains(Program.ArgumentNameAddBatch))
                {
                    requestedActions |= CommandLineActions.AddBatch;
                    if (selectedAccount == null)
                    {
                        errorMessage = "Files were requested to be added to the queue but the specified account was not found.";
                        showHelp = true;
                    }
                }
                if (valueArguments.Contains(Program.ArgumentNameAdd) || (requestedActions == CommandLineActions.None && fileNames.Count > 0))
                {
                    // If no action is requested explicitly and there are files in the arguments, assume they should be added.
                    requestedActions |= CommandLineActions.Add;
                    if (selectedAccount == null)
                    {
                        errorMessage = "Files were requested to be added to the queue but the specified account was not found.";
                        showHelp = true;
                    }
                }
            }

            if (showHelp)
            {
                // Incorrect parameters were passed, show a usage message and exit.
                shouldContinue = false;
                var usageMessage = new StringBuilder();
                var icon = MessageBoxImage.Information;
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    usageMessage.AppendFormat(CultureInfo.CurrentCulture, "You have started {0} with incorrect command-line parameters. {1}", App.Info.Name, errorMessage).AppendLine().AppendLine();
                    icon = MessageBoxImage.Error;
                    Logger.Log("Incorrect command-line parameters. " + errorMessage, TraceEventType.Error);
                }
                usageMessage.Append("Usage:").AppendLine();
                usageMessage.AppendFormat(CultureInfo.CurrentCulture, "    {0} [{1}:name] [{2}] [{3} | {4}] [file1 file2...]", App.Info.ExecutableName, Program.ArgumentNameAccount, Program.ArgumentNameUpload, Program.ArgumentNameAdd, Program.ArgumentNameAddBatch).AppendLine().AppendLine();
                usageMessage.Append("Examples:").AppendLine().AppendLine();
                usageMessage.Append("Upload the next batch of files from the default account:").AppendLine();
                usageMessage.AppendFormat(CultureInfo.CurrentCulture, "    {0} {1}", App.Info.ExecutableName, Program.ArgumentNameUpload).AppendLine().AppendLine();
                usageMessage.Append("Upload the next batch of files from a specified account:").AppendLine();
                usageMessage.AppendFormat(CultureInfo.CurrentCulture, "    {0} {1} {2}:\"My Account\"", App.Info.ExecutableName, Program.ArgumentNameUpload, Program.ArgumentNameAccount).AppendLine().AppendLine();
                usageMessage.Append("Add files to the queue of the default account:").AppendLine();
                usageMessage.AppendFormat(CultureInfo.CurrentCulture, "    {0} \"My Picture 1.jpg\" \"My Picture 2.jpg\"", App.Info.ExecutableName).AppendLine().AppendLine();
                usageMessage.Append("Add files to the queue of a specified account:").AppendLine();
                usageMessage.AppendFormat(CultureInfo.CurrentCulture, "    {0} {1} {2}:\"My Account\" \"My Picture 1.jpg\" \"My Picture 2.jpg\"", App.Info.ExecutableName, Program.ArgumentNameAdd, Program.ArgumentNameAccount).AppendLine().AppendLine();
                usageMessage.Append("Add a single batch of files to the queue of the default account:").AppendLine();
                usageMessage.AppendFormat(CultureInfo.CurrentCulture, "    {0} {1} \"My Picture 1.jpg\" \"My Picture 2.jpg\"", App.Info.ExecutableName, Program.ArgumentNameAddBatch);
                MessageBox.Show(usageMessage.ToString(), App.Info.Name, MessageBoxButton.OK, icon);
            }

            return shouldContinue;
        }

        private static void ParseArguments(IList<string> args, out IDictionary<string, string> nameValueArguments, out IList<string> valueArguments)
        {
            nameValueArguments = new Dictionary<string, string>();
            valueArguments = new List<string>();
            foreach (var arg in args)
            {
                // Parse arguments in the form "/name:value".
                if (arg != null && arg.StartsWith(ArgumentPrefix) && arg.IndexOf(":") >= 2)
                {
                    var separatorIndex = arg.IndexOf(":");
                    var argumentName = arg.Substring(1, separatorIndex - 1);
                    var argumentValue = arg.Substring(separatorIndex + 1);
                    nameValueArguments.Add(argumentName.ToLowerInvariant(), argumentValue);
                }
                else
                {
                    valueArguments.Add(arg);
                }
            }
        }

        private static string GetArgument(IDictionary<string, string> arguments, params string[] argumentNames)
        {
            foreach (var argumentName in argumentNames)
            {
                if (arguments.ContainsKey(argumentName.ToLowerInvariant()))
                {
                    return arguments[argumentName.ToLowerInvariant()];
                }
            }
            return null;
        }

        /// <summary>
        /// Logs application startup (either the first instance or a subsequent instance).
        /// </summary>
        /// <param name="startupMessage">The startup message.</param>
        /// <param name="arguments">The arguments.</param>
        private static void LogStartup(string startupMessage, ICollection<string> arguments)
        {
            Logger.Log(startupMessage, TraceEventType.Information);
            if (arguments != null && arguments.Count > 0)
            {
                var argumentsMessage = new StringBuilder("Command-line parameters: ");
                foreach (var argument in arguments)
                {
                    if (argument.Contains(' '))
                    {
                        // Enclose arguments with spaces in quotes for display purposes.
                        argumentsMessage.AppendFormat(CultureInfo.CurrentCulture, "\"{0}\"", argument);
                    }
                    else
                    {
                        argumentsMessage.Append(argument);
                    }
                    argumentsMessage.Append(" ");
                }
                Logger.Log(argumentsMessage.ToString().Trim(), TraceEventType.Information);
            }
        }

        #endregion

        #region Jump List

        /// <summary>
        /// Updates the Windows Jump List.
        /// </summary>
        private void UpdateJumpList()
        {
            if (this.app != null)
            {
                var jumpList = new JumpList();
                foreach (var account in this.Configuration.Accounts)
                {
                    var task = new JumpTask();
                    task.Title = string.Format(CultureInfo.CurrentCulture, "Upload Now ({0})", account.Name);
                    task.ApplicationPath = App.Info.ExecutablePath;
                    task.Arguments = Program.GetCommandLineArgumentsForUploadUI(account.Name);
                    task.Description = string.Format(CultureInfo.CurrentCulture, "Upload the next batch of files for the \"{0}\" account now", account.Name);
                    jumpList.JumpItems.Add(task);
                }
                jumpList.JumpItems.Add(new JumpTask { Title = "Open Log File", ApplicationPath = "notepad.exe", Arguments = PathProvider.LogFilePath, Description = "Open the log file" });
                jumpList.JumpItems.Add(new JumpTask { Title = App.Info.Name + " Homepage", ApplicationPath = App.Info.Url, Description = "Go to the homepage for " + App.Info.Name });
                jumpList.Apply();
                JumpList.SetJumpList(this.app, jumpList);
            }
        }

        #endregion

        #region GetCommandLineArgumentsForUpload

        /// <summary>
        /// Gets the command line arguments for uploading pictures from a specified account in the background.
        /// </summary>
        /// <param name="accountName">The account name.</param>
        /// <returns>The command line arguments to use to upload pictures from the specified account.</returns>
        public static string GetCommandLineArgumentsForUploadBackground(string accountName)
        {
            return GetCommandLineArgumentsForUpload(Program.ArgumentNameUpload, accountName);
        }

        /// <summary>
        /// Gets the command line arguments for uploading pictures from a specified account through the user interface.
        /// </summary>
        /// <param name="accountName">The account name.</param>
        /// <returns>The command line arguments to use to upload pictures from the specified account.</returns>
        public static string GetCommandLineArgumentsForUploadUI(string accountName)
        {
            return GetCommandLineArgumentsForUpload(Program.ArgumentNameUploadUI, accountName);
        }

        /// <summary>
        /// Gets the command line arguments for uploading pictures from a specified account.
        /// </summary>
        /// <param name="uploadArgument">The upload argument.</param>
        /// <param name="accountName">The account name.</param>
        /// <returns>The command line arguments to use to upload pictures from the specified account.</returns>
        private static string GetCommandLineArgumentsForUpload(string uploadArgument, string accountName)
        {
            if (!string.IsNullOrEmpty(accountName))
            {
                uploadArgument += string.Format(CultureInfo.InvariantCulture, " {0}:\"{1}\"", Program.ArgumentNameAccount, accountName);
            }
            return uploadArgument;
        }

        #endregion

        #region CommandLineActions Enum

        [Flags]
        private enum CommandLineActions
        {
            None = 0,
            UploadBackground = 1,
            UploadUI = 2,
            Add = 4,
            AddBatch = 8
        }

        #endregion
    }
}