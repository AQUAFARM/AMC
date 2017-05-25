using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JelleDruyts.Windows;
using JelleDruyts.Windows.ObjectModel;
using Microsoft.Win32;
using Schedulr.Infrastructure;
using Schedulr.Models;
using Schedulr.Providers;
using Schedulr.Views.Dialogs;

namespace Schedulr.ViewModels
{
    /// <summary>
    /// The view model for the details panel of a selected picture or aggregation of multiple pictures.
    /// </summary>
    public class PictureDetailsViewModel : ObservableObject
    {
        #region Properties

        /// <summary>
        /// Gets the account.
        /// </summary>
        public Account Account { get; private set; }

        /// <summary>
        /// Gets the informational message.
        /// </summary>
        public string InfoMessage { get; private set; }

        /// <summary>
        /// Gets the picture to display.
        /// </summary>
        public Picture Picture { get; private set; }

        /// <summary>
        /// Gets the visibility of the information message.
        /// </summary>
        public Visibility InfoMessageVisibility { get; private set; }

        /// <summary>
        /// Gets the visibility of the file details.
        /// </summary>
        public Visibility FileDetailsVisibility { get; private set; }

        /// <summary>
        /// Gets the label for the tags input control.
        /// </summary>
        public string TagsLabel { get; private set; }

        /// <summary>
        /// Gets the size of the file as a string.
        /// </summary>
        public string FileSizeDescription { get; private set; }

        /// <summary>
        /// Gets the visibility of the file size error.
        /// </summary>
        public Visibility FileSizeErrorVisibility { get; private set; }

        /// <summary>
        /// Gets the visibility of the online details.
        /// </summary>
        public Visibility OnlineDetailsVisibility { get; private set; }

        /// <summary>
        /// Gets a value that determines if the picture details should be read-only.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets a value that determines if the picture details should be editable.
        /// </summary>
        public bool IsEditable { get; private set; }

        /// <summary>
        /// Gets a value that determines if the picture is an aggregate.
        /// </summary>
        private bool IsAggregate { get; set; }

        /// <summary>
        /// Gets the location commands that are available for this picture.
        /// </summary>
        public IEnumerable<ICommand> LocationCommands { get; private set; }

        /// <summary>
        /// Gets the visibility of the location commands.
        /// </summary>
        public Visibility LocationCommandsVisibility { get; private set; }

        /// <summary>
        /// Gets a value that determines if the file name should be a link.
        /// </summary>
        public bool FileNameLinkEnabled { get; private set; }

        /// <summary>
        /// Gets the UI settings.
        /// </summary>
        public PictureDetailsUISettings UISettings { get; private set; }

        /// <summary>
        /// Gets the batch.
        /// </summary>
        public Batch Batch { get; private set; }

        /// <summary>
        /// Gets the visibility of the batch details.
        /// </summary>
        public Visibility BatchDetailsVisibility { get; private set; }

        /// <summary>
        /// Gets the pictures in the same batch as this picture.
        /// </summary>
        public ICollection<Picture> BatchPictures { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets the Flickr Sets to be displayed.
        /// </summary>
        public FlickrCollectionList Sets
        {
            get { return this.GetValue(SetsProperty); }
            set { this.SetValue(SetsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Sets"/> observable property.
        /// </summary>
        public static ObservableProperty<FlickrCollectionList> SetsProperty = new ObservableProperty<FlickrCollectionList, PictureDetailsViewModel>(o => o.Sets);

        /// <summary>
        /// Gets the Flickr Groups to be displayed.
        /// </summary>
        public FlickrCollectionList Groups
        {
            get { return this.GetValue(GroupsProperty); }
            set { this.SetValue(GroupsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Groups"/> observable property.
        /// </summary>
        public static ObservableProperty<FlickrCollectionList> GroupsProperty = new ObservableProperty<FlickrCollectionList, PictureDetailsViewModel>(o => o.Groups);

        /// <summary>
        /// Gets or sets the style to use for the Flickr collection lists.
        /// </summary>
        public object FlickrCollectionListStyle
        {
            get { return this.GetValue(FlickrCollectionListStyleProperty); }
            set { this.SetValue(FlickrCollectionListStyleProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="FlickrCollectionListStyle"/> observable property.
        /// </summary>
        public static ObservableProperty<object> FlickrCollectionListStyleProperty = new ObservableProperty<object, PictureDetailsViewModel>(o => o.FlickrCollectionListStyle);

        /// <summary>
        /// Gets or sets the visibility of the geographic location.
        /// </summary>
        public Visibility LocationVisibility
        {
            get { return this.GetValue(LocationVisibilityProperty); }
            set { this.SetValue(LocationVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="LocationVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> LocationVisibilityProperty = new ObservableProperty<Visibility, PictureDetailsViewModel>(o => o.LocationVisibility);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureDetailsViewModel"/> class.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="batch">The batch that contains the picture.</param>
        /// <param name="selectedPictures">The selected pictures.</param>
        /// <param name="uiSettings">The UI settings.</param>
        /// <param name="infoMessageVisibility">The visibility of the information message.</param>
        /// <param name="fileDetailsVisibility">The visibility of the file name.</param>
        /// <param name="isReadOnly">A value that determines if the picture details should be read-only.</param>
        public PictureDetailsViewModel(Account account, Batch batch, IList<Picture> selectedPictures, PictureDetailsUISettings uiSettings, Visibility infoMessageVisibility, Visibility fileDetailsVisibility, bool isReadOnly)
        {
            Picture picture = null;
            var isAggregate = false;
            var detailsPanelVisible = false;
            string infoMessage = null;
            string tagsLabel = "Ta_gs";
            var fileSizeErrorVisibility = Visibility.Collapsed;
            var onlineDetailsVisibility = Visibility.Collapsed;
            long fileSize = 0;
            var fileNameLinkEnabled = false;
            var batchDetailsVisibility = Visibility.Collapsed;
            ICollection<Picture> batchPictures = null;
            if (selectedPictures != null && selectedPictures.Count > 0)
            {
                infoMessage = selectedPictures.Count.ToCountString("file", "Editing ");
                detailsPanelVisible = true;
                if (selectedPictures.Count == 1)
                {
                    var onlyPicture = selectedPictures[0];
                    picture = onlyPicture;
                    fileSize = picture.FileSize;
                    if (!Validator.IsPictureFileSizeValid(onlyPicture, account))
                    {
                        fileSizeErrorVisibility = Visibility.Visible;
                    }
                    onlineDetailsVisibility = (picture.DateUploaded.HasValue ? Visibility.Visible : Visibility.Collapsed);
                    fileNameLinkEnabled = File.Exists(onlyPicture.FileName);
                }
                else
                {
                    var aggregatePicture = new AggregatePicture(selectedPictures);
                    isAggregate = true;
                    picture = aggregatePicture;
                    fileSize = picture.FileSize;
                    tagsLabel = "Add the following ta_gs to all selected files";
                    fileDetailsVisibility = Visibility.Collapsed; // Always hide the file details for aggregate pictures.
                }
                batchPictures = (batch == null ? null : batch.Pictures);
                batchDetailsVisibility = (batch == null ? Visibility.Collapsed : Visibility.Visible);
            }
            this.Account = account;
            this.Picture = picture;
            this.IsAggregate = isAggregate;
            if (!uiSettings.PictureDetailsPanelIsLocked)
            {
                uiSettings.PictureDetailsPanelIsExpanded = detailsPanelVisible;
            }
            this.InfoMessage = infoMessage;
            this.InfoMessageVisibility = infoMessageVisibility;
            this.FileDetailsVisibility = fileDetailsVisibility;
            this.TagsLabel = tagsLabel;
            if (fileSize >= 0)
            {
                this.FileSizeDescription = fileSize.ToDisplayString();
            }
            this.FileSizeErrorVisibility = fileSizeErrorVisibility;
            this.OnlineDetailsVisibility = onlineDetailsVisibility;
            this.IsReadOnly = isReadOnly;
            this.IsEditable = !this.IsReadOnly;
            this.FileNameLinkEnabled = fileNameLinkEnabled;
            this.UISettings = uiSettings;
            this.Batch = batch;
            this.BatchDetailsVisibility = batchDetailsVisibility;
            this.BatchPictures = batchPictures;

            // Determine the sets and groups.
            DetermineFlickrCollectionListStyle();
            UpdateSetsAndGroups();

            // When the User Info changes, update the sets and groups.
            if (this.Account != null)
            {
                this.Account.PropertyChanged += (sender, e) => { if (e.PropertyName == Account.UserInfoProperty.Name) { UpdateSetsAndGroups(); } };
                this.Account.Settings.PropertyChanged += (sender, e) => { if (e.PropertyName == AccountSettings.PictureCollectionDisplayModeProperty.Name) { DetermineFlickrCollectionListStyle(); } };
            }

            this.LocationCommands = new ICommand[]
            {
                new RelayCommand(SetLocation, CanSetLocation, "Set Location...", "Sets the geographic location where the picture or video was taken"),
                new RelayCommand(ImportLocation, CanImportLocation, "Import Location...", "Imports the geographic location from another file (i.e. a picture or sidecar file)"),
                new RelayCommand(ClearLocation, CanClearLocation, "Clear Location", "Clears the geographic location")
            };
            this.LocationCommandsVisibility = (this.IsReadOnly ? Visibility.Collapsed : Visibility.Visible);

            UpdateLocationVisibility();
        }

        #endregion

        #region Commands

        private bool CanSetLocation(object parameter)
        {
            return (this.Picture != null);
        }

        private void SetLocation(object parameter)
        {
            var dialog = new GeoLocationDialog();
            dialog.Location = this.Picture.Location ?? this.Account.Settings.LastLocation;
            dialog.GeoLocationMapProvider = this.Account.Settings.GeoLocationMapProvider;
            dialog.Owner = App.Current.MainWindow;
            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.Picture.Location = dialog.Location;
                this.Account.Settings.LastLocation = this.Picture.Location;
                this.Account.Settings.GeoLocationMapProvider = dialog.GeoLocationMapProvider;
            }
            UpdateLocationVisibility();
        }

        private bool CanClearLocation(object parameter)
        {
            // Always allow aggregate pictures to be cleared.
            return (this.Picture != null && (this.IsAggregate || this.Picture.Location != null));
        }

        private void ClearLocation(object parameter)
        {
            this.Picture.Location = null;
            UpdateLocationVisibility();
        }

        private bool CanImportLocation(object parameter)
        {
            return (this.Picture != null);
        }

        private void ImportLocation(object parameter)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Please select a picture or sidecar file that contains the geo location to import";
            if (openFileDialog.ShowDialog() == true)
            {
                var metadata = PictureMetadataProvider.RetrieveMetadataFromFile(openFileDialog.FileName, Logger.SchedulrLogger);
                if (metadata.GeoLocation != null)
                {
                    this.Picture.Location = metadata.GeoLocation;
                    UpdateLocationVisibility();
                }
            }
        }

        #endregion

        #region Helper Methods

        private void UpdateSetsAndGroups()
        {
            IList<FlickrCollectionListItem> setsData = null;
            IList<FlickrCollectionListItem> groupsData = null;

            if (this.Picture != null)
            {
                var aggregatePicture = this.Picture as AggregatePicture;
                if (aggregatePicture == null)
                {
                    setsData = this.Account.UserInfo.Sets.Select(s => new FlickrCollectionListItem(s, this.Picture.SetIds, this.Picture.SetIds.Contains(s.Id), PathProvider.PlaceHolderImagePathFlickrSet)).ToList();
                    groupsData = this.Account.UserInfo.Groups.Select(g => new FlickrCollectionListItem(g, this.Picture.GroupIds, this.Picture.GroupIds.Contains(g.Id), PathProvider.PlaceHolderImagePathFlickrGroup)).ToList();
                }
                else
                {
                    setsData = this.Account.UserInfo.Sets.Select(s => new FlickrCollectionListItem(s, aggregatePicture.SetIds, aggregatePicture.SetIdsContains(s.Id), PathProvider.PlaceHolderImagePathFlickrSet)).ToList();
                    groupsData = this.Account.UserInfo.Groups.Select(g => new FlickrCollectionListItem(g, aggregatePicture.GroupIds, aggregatePicture.GroupIdsContains(g.Id), PathProvider.PlaceHolderImagePathFlickrGroup)).ToList();
                }
            }

            if (this.Sets != null)
            {
                this.Sets.Dispose();
            }
            if (this.Groups != null)
            {
                this.Groups.Dispose();
            }
            this.Sets = new FlickrCollectionList(setsData, "Sets", this.Account.Settings, AccountSettings.SetsSortModeProperty);
            this.Groups = new FlickrCollectionList(groupsData, "Groups", this.Account.Settings, AccountSettings.GroupsSortModeProperty);
        }

        private void DetermineFlickrCollectionListStyle()
        {
            var displayMode = AccountSettings.PictureCollectionDisplayModeProperty.DefaultValue;
            if (this.Account != null)
            {
                displayMode = this.Account.Settings.PictureCollectionDisplayMode;
            }
            var styleName = "FlickrCollectionListStyle" + displayMode.ToString();
            this.FlickrCollectionListStyle = App.Current.FindResource(styleName);
        }

        private void UpdateLocationVisibility()
        {
            this.LocationVisibility = ((this.Picture != null && this.Picture.Location == null) ? Visibility.Collapsed : Visibility.Visible);
        }

        #endregion
    }
}