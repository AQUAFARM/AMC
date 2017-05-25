using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using JelleDruyts.Windows;
using Schedulr.Extensibility;
using Schedulr.Infrastructure;
using Schedulr.Models;

namespace Schedulr.ViewModels
{
    public class AccountViewModel
    {
        #region Fields

        private List<AccountUserMessageViewModel> tempUserMessages;
        private UserInfo tempUserInfo;
        private object updateLock = new object();

        #endregion

        #region Properties

        public ObservableCollection<AccountUserMessageViewModel> UserMessages { get; private set; }
        public Account Account { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountViewModel"/> class.
        /// </summary>
        /// <param name="account">The account.</param>
        public AccountViewModel(Account account)
        {
            this.Account = account;
            this.UserMessages = new ObservableCollection<AccountUserMessageViewModel>();
            this.tempUserMessages = new List<AccountUserMessageViewModel>();
        }

        #endregion

        #region Update

        /// <summary>
        /// Prepares the update.
        /// </summary>
        /// <param name="updatingAccountsTask">The task that tracks updating the accounts.</param>
        public void UpdatePrepare(ApplicationTask updatingAccountsTask)
        {
            lock (updateLock)
            {
                Logger.Log("Retrieving account information for " + this.Account.Name, TraceEventType.Information);
                this.tempUserMessages.Clear();
                this.tempUserInfo = null;
#if OFFLINE
                if (Math.Max(0, 1) == 1) // Outsmart the compiler so it doesn't complain about unreachable code below the return statement.
                {
                    var message = "Working in offline debugging mode";
                    updatingAccountsTask.Status = message;
                    this.tempUserMessages.Add(new AccountUserMessageViewModel(message));
                    this.tempUserMessages.Add(new AccountUserMessageViewModel("Click here to try again.", new RelayCommand(o => Tasks.UpdateAccount(this))));
                    return;
                }
#endif
                if (!FlickrClient.IsOnline())
                {
                    var message = App.Info.Name + " couldn't connect to Flickr and is working offline";
                    updatingAccountsTask.Status = message;
                    Logger.Log(message, TraceEventType.Information);
                    this.tempUserMessages.Add(new AccountUserMessageViewModel(message));
                    this.tempUserMessages.Add(new AccountUserMessageViewModel("Click here to try again.", new RelayCommand(o => Tasks.UpdateAccount(this))));
                }
                else
                {
                    try
                    {
                        var flickr = new FlickrClient(this.Account.AuthenticationToken, this.Account.TokenSecret);

                        // Get user and status information.
                        this.tempUserInfo = flickr.GetUserInfo();
                        var status = flickr.GetUserStatus();
                        var uploadLimitMessage = string.Format(CultureInfo.CurrentCulture, "You've uploaded {0} of your {1} photo limit ({2})", status.BandwidthUsed.ToDisplayString(), status.BandwidthMax.ToDisplayString(), status.BandwidthUsedPercentage.ToPercentageString());
                        if (status.VideosRemaining.HasValue && status.VideosUploaded.HasValue)
                        {
                            uploadLimitMessage += string.Format(CultureInfo.CurrentCulture, " and {0} of your {1} video limit", status.VideosUploaded.Value, status.VideosUploaded.Value + status.VideosRemaining.Value);
                        }
                        uploadLimitMessage += " this month.";
                        this.tempUserMessages.Add(new AccountUserMessageViewModel(uploadLimitMessage));
                        this.tempUserMessages.Add(new AccountUserMessageViewModel(string.Format(CultureInfo.CurrentCulture, "The maximum file size you are allowed to upload is {0} for pictures and {1} for videos.", tempUserInfo.UploadLimit.ToDisplayString(), tempUserInfo.UploadLimitVideo.ToDisplayString())));

                        // Attempt to refresh picture information that wasn't previously available.
                        var picturesToRefresh = this.Account.UploadedBatches.SelectMany(b => b.Pictures).Where(p => p.ShouldRefreshWebInfo).ToList();
                        if (picturesToRefresh.Count > 0)
                        {
                            updatingAccountsTask.Status = string.Format(CultureInfo.CurrentCulture, "Refreshing online information for {0}", picturesToRefresh.Count.ToCountString("file"));
                            var substep = 0;
                            foreach (var pictureToRefresh in picturesToRefresh)
                            {
                                updatingAccountsTask.Status = string.Format(CultureInfo.CurrentCulture, "Refreshing online information for \"{0}\"", pictureToRefresh.Title);
                                updatingAccountsTask.SetProgressForCurrentStep((double)substep / (double)picturesToRefresh.Count);
                                substep++;
                                flickr.RefreshWebInfo(pictureToRefresh);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        var errorMessage = "Failed to retrieve the account information for " + this.Account.Name;
                        Logger.Log(errorMessage, exc);
                        updatingAccountsTask.SetError(errorMessage, exc);
                        this.tempUserMessages.Add(new AccountUserMessageViewModel("Failed to retrieve the account information: " + exc.Message));
                        this.tempUserMessages.Add(new AccountUserMessageViewModel("Click here to try again.", new RelayCommand(o => Tasks.UpdateAccount(this))));
                    }
                }
            }
        }

        /// <summary>
        /// Commits the update.
        /// </summary>
        /// <param name="updatingAccountsTask">The task that tracks updating the accounts.</param>
        /// <returns><see langword="true"/> if the update succeeded, <see langword="false"/> otherwise.</returns>
        public bool UpdateCommit(ApplicationTask updatingAccountsTask)
        {
            lock (updateLock)
            {
                Logger.Log("Updating account information for " + this.Account.Name, TraceEventType.Information);

                // Update user messages.
                this.UserMessages.ReplaceItems(this.tempUserMessages);

                // Update user information.
                var success = false;
                if (this.tempUserInfo != null)
                {
                    PluginManager.OnAccountEvent(new AccountEventArgs(AccountEventType.Refreshing, App.Info, this.Account));
                    this.Account.UserInfo = tempUserInfo;
                    PluginManager.OnAccountEvent(new AccountEventArgs(AccountEventType.Refreshed, App.Info, this.Account));
                    success = true;
                }

                return success;
            }
        }

        #endregion
    }
}