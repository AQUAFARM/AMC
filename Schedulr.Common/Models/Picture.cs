using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// A picture that can be uploaded to Flickr.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class Picture : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        [DataMember]
        public virtual string FileName
        {
            get { return this.GetValue(FileNameProperty); }
            set { this.SetValue(FileNameProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="FileName"/> observable property.
        /// </summary>
        public static ObservableProperty<string> FileNameProperty = new ObservableProperty<string, Picture>(o => o.FileName);

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        [DataMember]
        public virtual string Tags
        {
            get { return this.GetValue(TagsProperty); }
            set { this.SetValue(TagsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Tags"/> observable property.
        /// </summary>
        public static ObservableProperty<string> TagsProperty = new ObservableProperty<string, Picture>(o => o.Tags);

        /// <summary>
        /// Gets or sets a value indicating whether this picture is public.
        /// </summary>
        [DataMember]
        public virtual bool? VisibilityIsPublic
        {
            get { return this.GetValue(VisibilityIsPublicProperty); }
            set { this.SetValue(VisibilityIsPublicProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="VisibilityIsPublic"/> observable property.
        /// </summary>
        public static ObservableProperty<bool?> VisibilityIsPublicProperty = new ObservableProperty<bool?, Picture>(o => o.VisibilityIsPublic, true);

        /// <summary>
        /// Gets or sets a value indicating whether this picture is visible to family.
        /// </summary>
        [DataMember]
        public virtual bool? VisibilityIsFamily
        {
            get { return this.GetValue(VisibilityIsFamilyProperty); }
            set { this.SetValue(VisibilityIsFamilyProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="VisibilityIsFamily"/> observable property.
        /// </summary>
        public static ObservableProperty<bool?> VisibilityIsFamilyProperty = new ObservableProperty<bool?, Picture>(o => o.VisibilityIsFamily, true);

        /// <summary>
        /// Gets or sets a value indicating whether this picture is visible to friends.
        /// </summary>
        [DataMember]
        public virtual bool? VisibilityIsFriend
        {
            get { return this.GetValue(VisibilityIsFriendProperty); }
            set { this.SetValue(VisibilityIsFriendProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="VisibilityIsFriend"/> observable property.
        /// </summary>
        public static ObservableProperty<bool?> VisibilityIsFriendProperty = new ObservableProperty<bool?, Picture>(o => o.VisibilityIsFriend, true);

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [DataMember]
        public virtual string Title
        {
            get { return this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Title"/> observable property.
        /// </summary>
        public static ObservableProperty<string> TitleProperty = new ObservableProperty<string, Picture>(o => o.Title);

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public virtual string Description
        {
            get { return this.GetValue(DescriptionProperty); }
            set { this.SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Description"/> observable property.
        /// </summary>
        public static ObservableProperty<string> DescriptionProperty = new ObservableProperty<string, Picture>(o => o.Description);

        /// <summary>
        /// Gets or sets the ID's of the groups the picture should be added to.
        /// </summary>
        [DataMember]
        public virtual ObservableCollection<string> GroupIds
        {
            get { return this.GetValue(GroupIdsProperty); }
            set { this.SetValue(GroupIdsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="GroupIds"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<string>> GroupIdsProperty = new ObservableProperty<ObservableCollection<string>, Picture>(o => o.GroupIds);

        /// <summary>
        /// Gets or sets the ID's of the sets the picture should be added to.
        /// </summary>
        [DataMember]
        public virtual ObservableCollection<string> SetIds
        {
            get { return this.GetValue(SetIdsProperty); }
            set { this.SetValue(SetIdsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="SetIds"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<string>> SetIdsProperty = new ObservableProperty<ObservableCollection<string>, Picture>(o => o.SetIds);

        /// <summary>
        /// Gets or sets the safety rating of this picture.
        /// </summary>
        [DataMember]
        public virtual Safety? Safety
        {
            get { return this.GetValue(SafetyProperty); }
            set { this.SetValue(SafetyProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Safety"/> observable property.
        /// </summary>
        public static ObservableProperty<Safety?> SafetyProperty = new ObservableProperty<Safety?, Picture>(o => o.Safety, Models.Safety.None);

        /// <summary>
        /// Gets or sets the content type of this picture.
        /// </summary>
        [DataMember]
        public virtual ContentType? ContentType
        {
            get { return this.GetValue(ContentTypeProperty); }
            set { this.SetValue(ContentTypeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ContentType"/> observable property.
        /// </summary>
        public static ObservableProperty<ContentType?> ContentTypeProperty = new ObservableProperty<ContentType?, Picture>(o => o.ContentType, Models.ContentType.None);

        /// <summary>
        /// Gets or sets the license of this picture.
        /// </summary>
        [DataMember]
        public virtual License? License
        {
            get { return this.GetValue(LicenseProperty); }
            set { this.SetValue(LicenseProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="License"/> observable property.
        /// </summary>
        public static ObservableProperty<License?> LicenseProperty = new ObservableProperty<License?, Picture>(o => o.License, Models.License.None);

        /// <summary>
        /// Gets or sets the date and time the picture was uploaded, or <see langword="null"/> if the picture hasn't been uploaded yet.
        /// </summary>
        [DataMember]
        public virtual DateTime? DateUploaded
        {
            get { return this.GetValue(DateUploadedProperty); }
            set { this.SetValue(DateUploadedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="DateUploaded"/> observable property.
        /// </summary>
        public static ObservableProperty<DateTime?> DateUploadedProperty = new ObservableProperty<DateTime?, Picture>(o => o.DateUploaded);

        /// <summary>
        /// Gets or sets the ID of the picture on Flickr if it has been uploaded.
        /// </summary>
        [DataMember]
        public string PictureId
        {
            get { return this.GetValue(PictureIdProperty); }
            set { this.SetValue(PictureIdProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PictureId"/> observable property.
        /// </summary>
        public static ObservableProperty<string> PictureIdProperty = new ObservableProperty<string, Picture>(o => o.PictureId);

        /// <summary>
        /// Gets or sets the URL of the preview (small) sized picture on Flickr if it has been uploaded.
        /// </summary>
        [DataMember]
        public string PreviewUrl
        {
            get { return this.GetValue(PreviewUrlProperty); }
            set { this.SetValue(PreviewUrlProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PreviewUrl"/> observable property.
        /// </summary>
        public static ObservableProperty<string> PreviewUrlProperty = new ObservableProperty<string, Picture>(o => o.PreviewUrl);

        /// <summary>
        /// Gets or sets the URL of the page where the picture can be viewed on Flickr if it has been uploaded.
        /// </summary>
        [DataMember]
        public string WebUrl
        {
            get { return this.GetValue(WebUrlProperty); }
            set { this.SetValue(WebUrlProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="WebUrl"/> observable property.
        /// </summary>
        public static ObservableProperty<string> WebUrlProperty = new ObservableProperty<string, Picture>(o => o.WebUrl);

        /// <summary>
        /// Gets or sets a value that determines if the web information should be refreshed.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public bool ShouldRefreshWebInfo
        {
            get { return this.GetValue(ShouldRefreshWebInfoProperty); }
            set { this.SetValue(ShouldRefreshWebInfoProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ShouldRefreshWebInfo"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> ShouldRefreshWebInfoProperty = new ObservableProperty<bool, Picture>(o => o.ShouldRefreshWebInfo);

        /// <summary>
        /// Gets or sets the geographic location where the picture was taken.
        /// </summary>
        [DataMember]
        public virtual GeoLocation Location
        {
            get { return this.GetValue(LocationProperty); }
            set { this.SetValue(LocationProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Location"/> observable property.
        /// </summary>
        public static ObservableProperty<GeoLocation> LocationProperty = new ObservableProperty<GeoLocation, Picture>(o => o.Location);

        /// <summary>
        /// Gets or sets the visibility of the picture in the global search results.
        /// </summary>
        [DataMember]
        public virtual SearchVisibility? SearchVisibility
        {
            get { return this.GetValue(SearchVisibilityProperty); }
            set { this.SetValue(SearchVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="SearchVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<SearchVisibility?> SearchVisibilityProperty = new ObservableProperty<SearchVisibility?, Picture>(o => o.SearchVisibility, Models.SearchVisibility.None);

        /// <summary>
        /// Used to show the upload progress of the file
        /// </summary>
        [IgnoreDataMember]
        public bool IsUploading
        {
            get { return this.GetValue(IsUploadingProperty); }
            set { this.SetValue(IsUploadingProperty, value); }
        }
        /// <summary>
        /// The definition for the <see cref="IsUploading"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> IsUploadingProperty = new ObservableProperty<bool, Picture>(o => o.IsUploading);

        /// <summary>
        /// Used to show the upload progress of the file
        /// </summary>
        [IgnoreDataMember]
        public short UploadProgress
        {
            get { return this.GetValue(UploadProgressProperty); }
            set { this.SetValue(UploadProgressProperty, value); }
        }
        /// <summary>
        /// The definition for the <see cref="UploadProgress"/> observable property.
        /// </summary>
        public static ObservableProperty<short> UploadProgressProperty = new ObservableProperty<short, Picture>(o => o.UploadProgress, 0);

        #endregion

        #region Obsolete Properties

#pragma warning disable 618 // Disable warning about obsolete members

        /// <summary>
        /// Gets or sets the ID of the upload batch this picture belongs to.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        [Obsolete("This property is obsolete and has been replaced by the Batch class.")]
        public virtual string BatchId
        {
            get { return this.GetValue(BatchIdProperty); }
            set { this.SetValue(BatchIdProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="BatchId"/> observable property.
        /// </summary>
        [Obsolete("This property is obsolete and has been replaced by the Batch class.")]
        public static ObservableProperty<string> BatchIdProperty = new ObservableProperty<string, Picture>(o => o.BatchId);

#pragma warning disable 618 // Disable warning about obsolete members

        #endregion

        #region Derived Properties

        /// <summary>
        /// Gets the short name of the file.
        /// </summary>
        public virtual string ShortFileName
        {
            get
            {
                if (string.IsNullOrEmpty(this.FileName))
                {
                    return this.FileName;
                }
                else
                {
                    return Path.GetFileName(this.FileName);
                }
            }
        }

        /// <summary>
        /// Gets the size of the file in bytes.
        /// </summary>
        public virtual long FileSize
        {
            get
            {
                if (string.IsNullOrEmpty(this.FileName) || !File.Exists(this.FileName))
                {
                    return -1;
                }
                else
                {
                    return new FileInfo(this.FileName).Length;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Picture"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Picture()
        {
            this.SetIds = new ObservableCollection<string>();
            this.GroupIds = new ObservableCollection<string>();
        }

        #endregion
    }
}