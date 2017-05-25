using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Shell;
using JelleDruyts.Windows;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Infrastructure;
using Schedulr.Messages;
using Schedulr.Models;

namespace Schedulr.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region Properties

        public RelayCommand VisitHomepageCommand { get; private set; }
        public RelayCommand ManageAccountsCommand { get; private set; }
        public RelayCommand UpdateAccountsCommand { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public SchedulrConfiguration Configuration
        {
            get { return this.GetValue(ConfigurationProperty); }
            set { this.SetValue(ConfigurationProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Configuration"/> observable property.
        /// </summary>
        public static ObservableProperty<SchedulrConfiguration> ConfigurationProperty = new ObservableProperty<SchedulrConfiguration, MainWindowViewModel>(o => o.Configuration);

        /// <summary>
        /// Gets or sets the available accounts.
        /// </summary>
        public ObservableCollection<AccountViewModel> Accounts
        {
            get { return this.GetValue(AccountsProperty); }
            set { this.SetValue(AccountsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Accounts"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<AccountViewModel>> AccountsProperty = new ObservableProperty<ObservableCollection<AccountViewModel>, MainWindowViewModel>(o => o.Accounts);

        /// <summary>
        /// Gets or sets the visibility of the configuration editor.
        /// </summary>
        public Visibility ConfigurationEditorVisibility
        {
            get { return this.GetValue(ConfigurationEditorVisibilityProperty); }
            set { this.SetValue(ConfigurationEditorVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ConfigurationEditorVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> ConfigurationEditorVisibilityProperty = new ObservableProperty<Visibility, MainWindowViewModel>(o => o.ConfigurationEditorVisibility, Visibility.Collapsed);

        /// <summary>
        /// Gets or sets the visibility of the upload confirmation.
        /// </summary>
        public Visibility UploadConfirmationVisibility
        {
            get { return this.GetValue(UploadConfirmationVisibilityProperty); }
            set { this.SetValue(UploadConfirmationVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UploadConfirmationVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> UploadConfirmationVisibilityProperty = new ObservableProperty<Visibility, MainWindowViewModel>(o => o.UploadConfirmationVisibility, Visibility.Collapsed);

        /// <summary>
        /// Gets or sets the selected account.
        /// </summary>
        public AccountViewModel SelectedAccount
        {
            get { return this.GetValue<AccountViewModel>(SelectedAccountProperty); }
            set
            {
                Logger.Log("MainWindowViewModel - Setting selected account", TraceEventType.Verbose);
                this.SetValue<AccountViewModel>(SelectedAccountProperty, value);
                Logger.Log("MainWindowViewModel - Setting selected account - done", TraceEventType.Verbose);
            }
        }

        /// <summary>
        /// The definition for the <see cref="SelectedAccount"/> observable property.
        /// </summary>
        public static ObservableProperty<AccountViewModel> SelectedAccountProperty = new ObservableProperty<AccountViewModel, MainWindowViewModel>(o => o.SelectedAccount, OnSelectedAccountChanged);

        /// <summary>
        /// Gets or sets a value that determines if the account details are visible.
        /// </summary>
        public Visibility AccountDetailsVisibility
        {
            get { return this.GetValue(AccountDetailsVisibilityProperty); }
            set { this.SetValue(AccountDetailsVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="AccountDetailsVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> AccountDetailsVisibilityProperty = new ObservableProperty<Visibility, MainWindowViewModel>(o => o.AccountDetailsVisibility, Visibility.Collapsed);

        /// <summary>
        /// Gets or sets the title of the main window.
        /// </summary>
        public string MainWindowTitle
        {
            get { return this.GetValue(MainWindowTitleProperty); }
            set { this.SetValue(MainWindowTitleProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="MainWindowTitle"/> observable property.
        /// </summary>
        public static ObservableProperty<string> MainWindowTitleProperty = new ObservableProperty<string, MainWindowViewModel>(o => o.MainWindowTitle, App.MainWindowTitle);

        /// <summary>
        /// Gets or sets the taskbar item info of the main window.
        /// </summary>
        public TaskbarItemInfo TaskbarItemInfo
        {
            get { return this.GetValue(TaskbarItemInfoProperty); }
            private set { this.SetValue(TaskbarItemInfoProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="TaskbarItemInfo"/> observable property.
        /// </summary>
        public static ObservableProperty<TaskbarItemInfo> TaskbarItemInfoProperty = new ObservableProperty<TaskbarItemInfo, MainWindowViewModel>(o => o.TaskbarItemInfo);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.TaskbarItemInfo = new TaskbarItemInfo();
            this.Accounts = new ObservableCollection<AccountViewModel>();
            this.VisitHomepageCommand = new RelayCommand(VisitHomepage, CanVisitHomepage);
            this.ManageAccountsCommand = new RelayCommand(ManageAccounts, CanManageAccounts);
            this.UpdateAccountsCommand = new RelayCommand(UpdateAccounts, CanUpdateAccounts);
            Messenger.Register<ConfigurationChangedMessage>(OnConfigurationChanged);
            Messenger.Register<DialogCloseRequestedMessage>(OnDialogCloseRequestedMessage);
            Messenger.Register<AccountActionMessage>(OnAccountActionMessage);
            Messenger.Register<AddPicturesRequestedMessage>(OnAddPicturesRequestedMessage);
            Messenger.Register<UploadPicturesRequestedMessage>(OnUploadPicturesRequested);
        }

        #endregion

        #region Message Handlers

        private void OnConfigurationChanged(ConfigurationChangedMessage message)
        {
            Logger.Log("MainWindowViewModel - Configuration changed", TraceEventType.Verbose);
            this.Configuration = message.Configuration;
            Logger.Log("MainWindowViewModel - Configuration set", TraceEventType.Verbose);
            this.Accounts = new ObservableCollection<AccountViewModel>(this.Configuration.Accounts.Select(a => new AccountViewModel(a)));
            Logger.Log("MainWindowViewModel - Accounts set", TraceEventType.Verbose);
            this.SelectedAccount = GetDefaultAccount(this.Accounts);
            Logger.Log("MainWindowViewModel - Selected account set", TraceEventType.Verbose);
            if (this.SelectedAccount == null)
            {
                SetVisibility(VisibilityPurpose.ShowConfigurationEditor);
            }
            else
            {
                SetVisibility(VisibilityPurpose.ShowAccountDetails);
            }
            if (FlickrClient.IsOnline())
            {
                Logger.Log("MainWindowViewModel - Updating accounts", TraceEventType.Verbose);
                Tasks.UpdateAccounts(this.Accounts);
                Logger.Log("MainWindowViewModel - Updating accounts - done", TraceEventType.Verbose);
            }
        }

        private void OnDialogCloseRequestedMessage(DialogCloseRequestedMessage message)
        {
            if (message.Dialog == Dialog.ConfigurationEditor)
            {
                SetVisibility(VisibilityPurpose.HideConfigurationEditor);
            }
            else if (message.Dialog == Dialog.UploadConfirmation)
            {
                SetVisibility(VisibilityPurpose.HideUploadConfirmation);
            }
        }

        private void OnAccountActionMessage(AccountActionMessage message)
        {
            if (message.Action == ListAction.Added)
            {
                // Add the account and update.
                this.Configuration.Accounts.Add(message.Account);
                var accountViewModel = new AccountViewModel(message.Account);
                this.Accounts.Add(accountViewModel);
                Tasks.UpdateAccount(accountViewModel);
            }
            else if (message.Action == ListAction.Removed)
            {
                // Remove the account.
                var accountToRemove = this.Accounts.Where(a => a.Account == message.Account).First();
                this.Accounts.Remove(accountToRemove);
                this.Configuration.Accounts.Remove(message.Account);

                // Set a new default account if needed.
                if (this.Accounts.Count > 0 && !this.Accounts.Any(a => a.Account.IsDefaultAccount))
                {
                    this.Accounts[0].Account.IsDefaultAccount = true;
                }
            }

            if (message.Action != ListAction.CurrentChanged && this.SelectedAccount == null)
            {
                this.SelectedAccount = GetDefaultAccount(this.Accounts);
            }
        }

        private void OnAddPicturesRequestedMessage(AddPicturesRequestedMessage message)
        {
            Tasks.AddPicturesToQueue(this.SelectedAccount.Account, message.FileNames, message.Pictures, message.AddToSingleBatch);
        }

        private void OnUploadPicturesRequested(UploadPicturesRequestedMessage message)
        {
            this.ExecuteUIActionAsync(() =>
            {
                if (!message.UploadConfirmed)
                {
                    // Do not upload before the user has confirmed.
                    SetVisibility(VisibilityPurpose.ShowUploadConfirmation);
                }
                else
                {
                    var account = message.Account ?? this.SelectedAccount.Account;
                    Action postUploadCallback = () =>
                    {
                        // Update the account to refresh the user status.
                        var accountVM = this.Accounts.Where(a => a.Account == account).FirstOrDefault();
                        if (accountVM != null)
                        {
                            Tasks.UpdateAccount(accountVM);
                        }
                    };
                    if (message.Batch != null)
                    {
                        Tasks.UploadBatch(account, message.Batch, postUploadCallback);
                    }
                    else
                    {
                        Tasks.UploadPictures(account, message.Pictures, postUploadCallback);
                    }
                }
            });
        }

        #endregion

        #region Commands

        private bool CanManageAccounts(object parameter)
        {
            return true;
        }

        private void ManageAccounts(object parameter)
        {
            SetVisibility(VisibilityPurpose.ShowConfigurationEditor);
        }

        private bool CanUpdateAccounts(object parameter)
        {
            return (this.Accounts != null && this.Accounts.Count > 0);
        }

        private void UpdateAccounts(object parameter)
        {
            Tasks.UpdateAccounts(this.Accounts);
        }

        private bool CanVisitHomepage(object parameter)
        {
            return !string.IsNullOrEmpty(parameter as string);
        }

        private void VisitHomepage(object parameter)
        {
            Process.Start(new ProcessStartInfo(parameter as string));
        }

        #endregion

        #region Helper Methods

        private static void OnSelectedAccountChanged(ObservableObject sender, ObservablePropertyChangedEventArgs<AccountViewModel> e)
        {
            Messenger.Send<AccountActionMessage>(new AccountActionMessage((e.NewValue == null ? null : e.NewValue.Account), ListAction.CurrentChanged));
        }

        private static AccountViewModel GetDefaultAccount(IList<AccountViewModel> accounts)
        {
            AccountViewModel selectedAccount;
            if (App.Instance.Program.RequestedAccount != null)
            {
                selectedAccount = accounts.Where(a => a.Account == App.Instance.Program.RequestedAccount).FirstOrDefault();
            }
            else
            {
                selectedAccount = accounts.Where(a => a.Account.IsDefaultAccount).FirstOrDefault();
            }
            if (selectedAccount == null && accounts.Count > 0)
            {
                selectedAccount = accounts[0];
            }
            return selectedAccount;
        }

        private enum VisibilityPurpose
        {
            ShowAccountDetails,
            ShowConfigurationEditor,
            HideConfigurationEditor,
            ShowUploadConfirmation,
            HideUploadConfirmation
        }

        private void SetVisibility(VisibilityPurpose purpose)
        {
            switch (purpose)
            {
                case VisibilityPurpose.ShowConfigurationEditor:
                    this.ConfigurationEditorVisibility = Visibility.Visible;
                    this.UploadConfirmationVisibility = Visibility.Collapsed;
                    this.AccountDetailsVisibility = Visibility.Collapsed;
                    break;
                case VisibilityPurpose.ShowUploadConfirmation:
                    this.ConfigurationEditorVisibility = Visibility.Collapsed;
                    this.UploadConfirmationVisibility = Visibility.Visible;
                    this.AccountDetailsVisibility = Visibility.Collapsed;
                    break;
                case VisibilityPurpose.HideConfigurationEditor:
                case VisibilityPurpose.HideUploadConfirmation:
                default:
                    this.ConfigurationEditorVisibility = Visibility.Collapsed;
                    this.UploadConfirmationVisibility = Visibility.Collapsed;
                    this.AccountDetailsVisibility = Visibility.Visible;
                    break;
            }
        }

        #endregion
    }
}