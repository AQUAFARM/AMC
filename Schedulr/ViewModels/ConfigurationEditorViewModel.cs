using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JelleDruyts.Windows;
using JelleDruyts.Windows.ObjectModel;
using Microsoft.Win32;
using Schedulr.Extensibility;
using Schedulr.Infrastructure;
using Schedulr.Messages;
using Schedulr.Models;
using Schedulr.Providers;
using Schedulr.Views.Dialogs;

namespace Schedulr.ViewModels
{
    public class ConfigurationEditorViewModel : ViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the commands that are available for the selected account.
        /// </summary>
        public IEnumerable<ICommand> SelectedAccountCommands { get; private set; }

        /// <summary>
        /// Gets the action commands that are available.
        /// </summary>
        public IEnumerable<ICommand> ActionsCommands { get; private set; }

        /// <summary>
        /// Gets the dialog commands that are available.
        /// </summary>
        public IEnumerable<ICommand> DialogCommands { get; private set; }

        /// <summary>
        /// Gets the input bindings for the commands.
        /// </summary>
        public IEnumerable<InputBinding> InputBindings { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets the accounts.
        /// </summary>
        public ObservableCollection<Account> Accounts
        {
            get { return this.GetValue(AccountsProperty); }
            set { this.SetValue(AccountsProperty, value); }
        }

        public static ObservableProperty<ObservableCollection<Account>> AccountsProperty = new ObservableProperty<ObservableCollection<Account>, ConfigurationEditorViewModel>(o => o.Accounts);

        /// <summary>
        /// Gets or sets the selected account.
        /// </summary>
        public Account SelectedAccount
        {
            get { return this.GetValue(SelectedAccountProperty); }
            set { this.SetValue(SelectedAccountProperty, value); }
        }

        public static ObservableProperty<Account> SelectedAccountProperty = new ObservableProperty<Account, ConfigurationEditorViewModel>(o => o.SelectedAccount);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationEditorViewModel"/> class.
        /// </summary>
        public ConfigurationEditorViewModel()
        {
            this.SelectedAccountCommands = new ICommand[]
            {
                new RelayCommand(RemoveAccount, CanRemoveAccount, "_Remove", "Removes the selected account [DEL]", new KeyGesture(Key.Delete)),
                new RelayCommand(SetDefaultAccount, CanSetDefaultAccount, "Set _Default", "Makes the selected account show by default when starting the application [ENTER]", new KeyGesture(Key.Enter)),
                new RelayCommand(ImportPictures, CanImportPictures, "_Import Pictures", "Imports the queued pictures from the configuration file of a previous version of the application [CTRL-M]", new KeyGesture(Key.M, ModifierKeys.Control))
            };

            this.ActionsCommands = new ICommand[]
            {
                new RelayCommand(AddAccount, CanAddAccount, "_Add Account...", "Adds a new account [INS]", new KeyGesture(Key.Insert)),
                App.Instance.SaveChangesCommand,
                App.Instance.UndoChangesCommand,
                App.Instance.ExportConfigurationCommand,
                App.Instance.ImportConfigurationCommand,
                App.Instance.OpenLogFileCommand
            };

            this.DialogCommands = new ICommand[]
            {
                new RelayCommand(Close, CanClose, "_Close", "Closes this dialog [ESC]", new KeyGesture(Key.Escape))
            };

            this.InputBindings = this.SelectedAccountCommands.Union(this.ActionsCommands).Union(this.DialogCommands).OfType<RelayCommand>().SelectMany(r => r.InputGestures.Select(g => new InputBinding(r, g)));
            Messenger.Register<ConfigurationChangedMessage>(OnConfigurationChanged);
        }

        #endregion

        #region Message Handlers

        private void OnConfigurationChanged(ConfigurationChangedMessage message)
        {
            Logger.Log("ConfigurationWindowViewModel - Setting accounts", TraceEventType.Verbose);
            this.Accounts = message.Configuration.Accounts;
        }

        #endregion

        #region Selected Account Commands

        private bool CanImportPictures(object parameter)
        {
            return (this.SelectedAccount != null);
        }

        private void ImportPictures(object parameter)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Please select the configuration file from a previous version of the application from which to import the queued pictures";
            openFileDialog.Filter = "XML files|*.xml|All files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var fileName = openFileDialog.FileName;
                    var count = SchedulrConfigurationProvider.ImportFromLegacyConfiguration(this.SelectedAccount, fileName);
                    Messenger.Send<StatusMessage>(new StatusMessage(string.Format(CultureInfo.CurrentCulture, "{0} queued pictures imported from \"{1}\"", count, Path.GetFileName(fileName))));
                }
                catch (Exception)
                {
                    MessageBox.Show(App.Current.MainWindow, string.Format(CultureInfo.CurrentCulture, "The queued pictures from the specified configuration file could not be imported.{0}{0}Note that this is only intended to import queued pictures from a previous version of the application. Export the configuration from the previous version and import it back into the selected account here.{0}{0}Please see the log file for more details.", Environment.NewLine), "Error importing pictures", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private bool CanSetDefaultAccount(object parameter)
        {
            return (this.SelectedAccount != null && !this.SelectedAccount.IsDefaultAccount);
        }

        private void SetDefaultAccount(object parameter)
        {
            foreach (var account in this.Accounts)
            {
                account.IsDefaultAccount = false;
            }
            this.SelectedAccount.IsDefaultAccount = true;
        }

        private bool CanRemoveAccount(object parameter)
        {
            return (this.SelectedAccount != null);
        }

        private void RemoveAccount(object parameter)
        {
            var account = this.SelectedAccount;
            PluginManager.OnGeneralAccountEvent(new GeneralAccountEventArgs(GeneralAccountEventType.Removing, App.Info, account));
            Messenger.Send<AccountActionMessage>(new AccountActionMessage(account, ListAction.Removed));
            PluginManager.OnGeneralAccountEvent(new GeneralAccountEventArgs(GeneralAccountEventType.Removed, App.Info, account));
        }

        #endregion

        #region Actions Commands

        private bool CanAddAccount(object parameter)
        {
            return FlickrClient.IsOnline();
        }

        private void AddAccount(object parameter)
        {
            PluginManager.OnGeneralAccountEvent(new GeneralAccountEventArgs(GeneralAccountEventType.Adding, App.Info, null));
            var addingAccountTask = new ApplicationTask("Adding account");
            Messenger.Send(new TaskStatusMessage(addingAccountTask));
            string accountName = null;
            try
            {
                var flickr = new FlickrClient();
                var authenticationUrl = flickr.BeginAuthentication();
                var authenticationDialog = new AuthenticationDialog(authenticationUrl);
                var result = authenticationDialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    var accessToken = flickr.EndAuthentication(authenticationDialog.VerificationCode);
                    var userInfo = flickr.GetUserInfo();
                    var isDefaultAccount = (this.Accounts != null && this.Accounts.Count == 0);

                    // Check if an account with the same Id or Name already exists.
                    var accountExists = this.Accounts.Any(a => string.Equals(a.Id, userInfo.UserId, StringComparison.OrdinalIgnoreCase) || string.Equals(a.Name, userInfo.UserName, StringComparison.OrdinalIgnoreCase));
                    if (accountExists)
                    {
                        var message = "This account is already configured; you cannot add the same account twice.";
                        addingAccountTask.Status = message;
                        MessageBox.Show(message, "Duplicate Account", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var account = new Account { AuthenticationToken = accessToken.Token, TokenSecret = accessToken.TokenSecret, IsDefaultAccount = isDefaultAccount, Id = userInfo.UserId, Name = userInfo.UserName, UserInfo = userInfo };
                        SchedulrConfigurationProvider.AddPluginToRetrieveMetadata(account);
                        accountName = account.Name;
                        Messenger.Send<AccountActionMessage>(new AccountActionMessage(account, ListAction.Added));
                        PluginManager.OnGeneralAccountEvent(new GeneralAccountEventArgs(GeneralAccountEventType.Added, App.Info, account));
                    }
                }
            }
            catch (Exception exc)
            {
                var errorMessage = "An error occurred while adding the account";
                Logger.Log(errorMessage, exc);
                addingAccountTask.SetError(errorMessage, exc);
            }
            finally
            {
                var message = (accountName == null ? "No account was added" : string.Format(CultureInfo.CurrentCulture, "\"{0}\" added", accountName));
                addingAccountTask.SetComplete(message);
            }
        }

        #endregion

        #region Dialog Commands

        private bool CanClose(object parameter)
        {
            return (this.Accounts != null && this.Accounts.Count > 0);
        }

        private void Close(object parameter)
        {
            Messenger.Send<DialogCloseRequestedMessage>(new DialogCloseRequestedMessage(Dialog.ConfigurationEditor));
        }

        #endregion
    }
}