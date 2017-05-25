using System.Windows;
using JelleDruyts.Windows;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Infrastructure;
using Schedulr.Messages;
using Schedulr.Models;

namespace Schedulr.ViewModels
{
    public class OptionsViewModel : ViewModel
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the picture defaults.
        /// </summary>
        public PictureDetailsViewModel PictureDefaults
        {
            get { return this.GetValue(PictureDefaultsProperty); }
            set { this.SetValue(PictureDefaultsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PictureDefaults"/> observable property.
        /// </summary>
        public static ObservableProperty<PictureDetailsViewModel> PictureDefaultsProperty = new ObservableProperty<PictureDetailsViewModel, OptionsViewModel>(o => o.PictureDefaults);

        /// <summary>
        /// Gets or sets the account settings.
        /// </summary>
        public AccountSettings AccountSettings
        {
            get { return this.GetValue(AccountSettingsProperty); }
            set { this.SetValue(AccountSettingsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="AccountSettings"/> observable property.
        /// </summary>
        public static ObservableProperty<AccountSettings> AccountSettingsProperty = new ObservableProperty<AccountSettings, OptionsViewModel>(o => o.AccountSettings);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsViewModel"/> class.
        /// </summary>
        public OptionsViewModel()
        {
            Messenger.Register<AccountActionMessage>(OnAccountActionMessage);
        }

        #endregion

        #region Message Handlers

        private void OnAccountActionMessage(AccountActionMessage message)
        {
            if (message.Action == ListAction.CurrentChanged)
            {
                var account = message.Account;
                if (account != null)
                {
                    this.AccountSettings = account.Settings;
                    this.PictureDefaults = new PictureDetailsViewModel(account, null, new Picture[] { account.Settings.PictureDefaults }, account.Settings.PictureDefaultsDetailsUISettings, Visibility.Collapsed, Visibility.Collapsed, false);
                }
                else
                {
                    this.AccountSettings = null;
                    this.PictureDefaults = null;
                }
            }
        }

        #endregion
    }
}