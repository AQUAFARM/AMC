using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// Contains all the interesting information about the user.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class UserInfo : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        [DataMember]
        public string UserId
        {
            get { return this.GetValue(UserIdProperty); }
            set { this.SetValue(UserIdProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UserId"/> observable property.
        /// </summary>
        public static ObservableProperty<string> UserIdProperty = new ObservableProperty<string, UserInfo>(o => o.UserId);

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [DataMember]
        public string UserName
        {
            get { return this.GetValue(UserNameProperty); }
            set { this.SetValue(UserNameProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UserName"/> observable property.
        /// </summary>
        public static ObservableProperty<string> UserNameProperty = new ObservableProperty<string, UserInfo>(o => o.UserName);

        /// <summary>
        /// Gets or sets the URL of the user's photo stream.
        /// </summary>
        [DataMember]
        public string PicturesUrl
        {
            get { return this.GetValue(PicturesUrlProperty); }
            set { this.SetValue(PicturesUrlProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PicturesUrl"/> observable property.
        /// </summary>
        public static ObservableProperty<string> PicturesUrlProperty = new ObservableProperty<string, UserInfo>(o => o.PicturesUrl);

        /// <summary>
        /// Gets or sets the URL of the user's buddy icon.
        /// </summary>
        [DataMember]
        public string BuddyIconUrl
        {
            get { return this.GetValue(BuddyIconUrlProperty); }
            set { this.SetValue(BuddyIconUrlProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="BuddyIconUrl"/> observable property.
        /// </summary>
        public static ObservableProperty<string> BuddyIconUrlProperty = new ObservableProperty<string, UserInfo>(o => o.BuddyIconUrl);

        /// <summary>
        /// Gets or sets a value that determines if the user has a pro account.
        /// </summary>
        [DataMember]
        public bool IsProUser
        {
            get { return this.GetValue(IsProUserProperty); }
            set { this.SetValue(IsProUserProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="IsProUser"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> IsProUserProperty = new ObservableProperty<bool, UserInfo>(o => o.IsProUser);

        /// <summary>
        /// Gets or sets the maximum file size (in bytes) that the user is allowed to upload.
        /// </summary>
        [DataMember]
        public long UploadLimit
        {
            get { return this.GetValue(UploadLimitProperty); }
            set { this.SetValue(UploadLimitProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UploadLimit"/> observable property.
        /// </summary>
        public static ObservableProperty<long> UploadLimitProperty = new ObservableProperty<long, UserInfo>(o => o.UploadLimit);

        /// <summary>
        /// Gets or sets the photo sets the user has created.
        /// </summary>
        [DataMember]
        public ObservableCollection<PictureCollection> Sets
        {
            get { return this.GetValue(SetsProperty); }
            set { this.SetValue(SetsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Sets"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<PictureCollection>> SetsProperty = new ObservableProperty<ObservableCollection<PictureCollection>, UserInfo>(o => o.Sets);

        /// <summary>
        /// Gets or sets the groups the user is a member of.
        /// </summary>
        [DataMember]
        public ObservableCollection<PictureCollection> Groups
        {
            get { return this.GetValue(GroupsProperty); }
            set { this.SetValue(GroupsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Groups"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<PictureCollection>> GroupsProperty = new ObservableProperty<ObservableCollection<PictureCollection>, UserInfo>(o => o.Groups);

        /// <summary>
        /// Gets or sets the maximum file size (in bytes) that the user is allowed to upload for videos.
        /// </summary>
        [DataMember]
        public long UploadLimitVideo
        {
            get { return this.GetValue(UploadLimitVideoProperty); }
            set { this.SetValue(UploadLimitVideoProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UploadLimitVideo"/> observable property.
        /// </summary>
        public static ObservableProperty<long> UploadLimitVideoProperty = new ObservableProperty<long, UserInfo>(o => o.UploadLimitVideo);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfo"/> class.
        /// </summary>
        public UserInfo()
        {
            this.Sets = new ObservableCollection<PictureCollection>();
            this.Groups = new ObservableCollection<PictureCollection>();
        }

        #endregion
    }
}