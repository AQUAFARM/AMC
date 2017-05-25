using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// Represents a Flickr account.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class Account : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the ID of the account.
        /// </summary>
        [DataMember]
        public string Id
        {
            get { return this.GetValue(IdProperty); }
            set { this.SetValue(IdProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Id"/> observable property.
        /// </summary>
        public static ObservableProperty<string> IdProperty = new ObservableProperty<string, Account>(o => o.Id);

        /// <summary>
        /// Gets or sets the name of the account.
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return this.GetValue(NameProperty); }
            set { this.SetValue(NameProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Name"/> observable property.
        /// </summary>
        public static ObservableProperty<string> NameProperty = new ObservableProperty<string, Account>(a => a.Name);

        /// <summary>
        /// Gets or sets a value that determines if this is the default account.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public bool IsDefaultAccount
        {
            get { return this.GetValue(IsDefaultAccountProperty); }
            set { this.SetValue(IsDefaultAccountProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="IsDefaultAccount"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> IsDefaultAccountProperty = new ObservableProperty<bool, Account>(a => a.IsDefaultAccount);

        /// <summary>
        /// Gets or sets the token used to authenticate with Flickr.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public string AuthenticationToken
        {
            get { return this.GetValue(AuthenticationTokenProperty); }
            set { this.SetValue(AuthenticationTokenProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="AuthenticationToken"/> observable property.
        /// </summary>
        public static ObservableProperty<string> AuthenticationTokenProperty = new ObservableProperty<string, Account>(a => a.AuthenticationToken);

        /// <summary>
        /// Gets or sets the tokensecret used to authenticate with Flickr.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public string TokenSecret
        {
            get { return this.GetValue(TokenSecretProperty); }
            set { this.SetValue(TokenSecretProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="TokenSecret"/> observable property.
        /// </summary>
        public static ObservableProperty<string> TokenSecretProperty = new ObservableProperty<string, Account>(a => a.TokenSecret);

        /// <summary>
        /// Gets or sets the information about the user.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public UserInfo UserInfo
        {
            get { return this.GetValue(UserInfoProperty); }
            set { this.SetValue(UserInfoProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UserInfo"/> observable property.
        /// </summary>
        public static ObservableProperty<UserInfo> UserInfoProperty = new ObservableProperty<UserInfo, Account>(o => o.UserInfo);

        /// <summary>
        /// Gets or sets the settings for the account.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public AccountSettings Settings
        {
            get { return this.GetValue(SettingsProperty); }
            set { this.SetValue(SettingsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Settings"/> observable property.
        /// </summary>
        public static ObservableProperty<AccountSettings> SettingsProperty = new ObservableProperty<AccountSettings, Account>(o => o.Settings);

        /// <summary>
        /// Gets or sets the upload schedule.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public UploadSchedule UploadSchedule
        {
            get { return this.GetValue(UploadScheduleProperty); }
            set { this.SetValue(UploadScheduleProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UploadSchedule"/> observable property.
        /// </summary>
        public static ObservableProperty<UploadSchedule> UploadScheduleProperty = new ObservableProperty<UploadSchedule, Account>(o => o.UploadSchedule);

        /// <summary>
        /// Gets or sets the account-level plugins.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public ObservableCollection<PluginConfiguration> Plugins
        {
            get { return this.GetValue(PluginsProperty); }
            set { this.SetValue(PluginsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Plugins"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<PluginConfiguration>> PluginsProperty = new ObservableProperty<ObservableCollection<PluginConfiguration>, Account>(o => o.Plugins);

        /// <summary>
        /// Gets or sets the queued batches.
        /// </summary>
        [DataMember]
        public ObservableCollection<Batch> QueuedBatches
        {
            get { return this.GetValue(QueuedBatchesProperty); }
            set { this.SetValue(QueuedBatchesProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="QueuedBatches"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<Batch>> QueuedBatchesProperty = new ObservableProperty<ObservableCollection<Batch>, Account>(o => o.QueuedBatches);

        /// <summary>
        /// Gets or sets the uploaded batches.
        /// </summary>
        [DataMember]
        public ObservableCollection<Batch> UploadedBatches
        {
            get { return this.GetValue(UploadedBatchesProperty); }
            set { this.SetValue(UploadedBatchesProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UploadedBatches"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<Batch>> UploadedBatchesProperty = new ObservableProperty<ObservableCollection<Batch>, Account>(o => o.UploadedBatches);

        #endregion

        #region Obsolete Properties

#pragma warning disable 618 // Disable warning about obsolete members

        /// <summary>
        /// Gets or sets the queued pictures.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        [Obsolete("This property is obsolete and has been replaced by the QueuedBatches property.")]
        public ObservableCollection<Picture> QueuedPictures
        {
            get { return this.GetValue(QueuedPicturesProperty); }
            set { this.SetValue(QueuedPicturesProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="QueuedPictures"/> observable property.
        /// </summary>
        [Obsolete("This property is obsolete and has been replaced by the QueuedBatches property.")]
        public static ObservableProperty<ObservableCollection<Picture>> QueuedPicturesProperty = new ObservableProperty<ObservableCollection<Picture>, Account>(o => o.QueuedPictures);

        /// <summary>
        /// Gets or sets the uploaded pictures.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        [Obsolete("This property is obsolete and has been replaced by the UploadedBatches property.")]
        public ObservableCollection<Picture> UploadedPictures
        {
            get { return this.GetValue(UploadedPicturesProperty); }
            set { this.SetValue(UploadedPicturesProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UploadedPictures"/> observable property.
        /// </summary>
        [Obsolete("This property is obsolete and has been replaced by the UploadedBatches property.")]
        public static ObservableProperty<ObservableCollection<Picture>> UploadedPicturesProperty = new ObservableProperty<ObservableCollection<Picture>, Account>(o => o.UploadedPictures);

#pragma warning disable 618 // Disable warning about obsolete members

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account()
        {
#pragma warning disable 618 // Disable warning about obsolete members
            this.QueuedPictures = new ObservableCollection<Picture>();
            this.UploadedPictures = new ObservableCollection<Picture>();
#pragma warning restore 618 // Restore warning about obsolete members
            this.UserInfo = new UserInfo();
            this.Settings = new AccountSettings();
            this.UploadSchedule = new UploadSchedule();
            this.Plugins = new ObservableCollection<PluginConfiguration>();
            this.QueuedBatches = new ObservableCollection<Batch>();
            this.UploadedBatches = new ObservableCollection<Batch>();
        }

        #endregion
    }
}