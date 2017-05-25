using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Infrastructure;
using Schedulr.Models;

namespace Schedulr.ViewModels
{
    /// <summary>
    /// The view model for a picture in a picture list.
    /// </summary>
    public class PictureViewModel : ObservableObject, IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the picture.
        /// </summary>
        public Picture Picture { get; private set; }

        /// <summary>
        /// Gets the account.
        /// </summary>
        public Account Account { get; private set; }

        /// <summary>
        /// Gets a value that determines if the picture has errors.
        /// </summary>
        public bool HasErrors { get; set; }

        /// <summary>
        /// Gets a value that determines if the file is actually a video.
        /// </summary>
        public bool IsVideo { get; private set; }

        /// <summary>
        /// Gets the picture list this picture belongs to.
        /// </summary>
        public PictureListViewModel PictureList { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets the URL of the image to display in the list.
        /// </summary>
        public string ImageUrl
        {
            get { return this.GetValue(ImageUrlProperty); }
            private set { this.SetValue(ImageUrlProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ImageUrl"/> observable property.
        /// </summary>
        public static ObservableProperty<string> ImageUrlProperty = new ObservableProperty<string, PictureViewModel>(o => o.ImageUrl);

        /// <summary>
        /// Gets the visibility of the title in the picture preview.
        /// </summary>
        public Visibility PreviewTitleVisibility
        {
            get { return this.GetValue(PreviewTitleVisibilityProperty); }
            set { this.SetValue(PreviewTitleVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PreviewTitleVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> PreviewTitleVisibilityProperty = new ObservableProperty<Visibility, PictureViewModel>(o => o.PreviewTitleVisibility);

        /// <summary>
        /// Gets the visibility of the upload progress
        /// </summary>
        public Visibility ProgressVisibility
        {
            get { return this.GetValue(ProgressVisibilityProperty); }
            set { this.SetValue(ProgressVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ProgressVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> ProgressVisibilityProperty = new ObservableProperty<Visibility, PictureViewModel>(o => o.ProgressVisibility, Visibility.Hidden);

        /// <summary>
        /// Gets the upload progress
        /// </summary>
        public short Progress
        {
            get { return this.GetValue(ProgressProperty); }
            set { this.SetValue(ProgressProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Progress"/> observable property.
        /// </summary>
        public static ObservableProperty<short> ProgressProperty = new ObservableProperty<short, PictureViewModel>(o => o.Progress, 0);

        /// <summary>
        /// Gets the visibility of the title in the picture preview.
        /// </summary>
        public Visibility CompletedVisibility
        {
            get { return this.GetValue(CompletedVisibilityProperty); }
            set { this.SetValue(CompletedVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="CompletedVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> CompletedVisibilityProperty = new ObservableProperty<Visibility, PictureViewModel>(o => o.CompletedVisibility, Visibility.Hidden);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureViewModel"/> class.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <param name="pictureList">The picture list this picture belongs to.</param>
        public PictureViewModel(Picture picture, PictureListViewModel pictureList)
        {
            this.Picture = picture;
            this.PictureList = pictureList;
            this.Account = this.PictureList.Account;
            this.IsVideo = App.IsVideoFile(this.Picture.FileName);
            this.HasErrors = !Validator.IsPictureValid(this.Picture, this.Account);

            UpdateDisplayModeProperties();

            this.Picture.PropertyChanged += OnPicturePropertyChanged;
            this.Account.Settings.PropertyChanged += OnSettingsPropertyChanged;
        }

        #endregion

        #region Property Changed Event Handlers

        private void OnPicturePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Picture.PreviewUrlProperty.Name)
            {
                UpdateDisplayModeProperties();
            }
            else if (e.PropertyName == Picture.UploadProgressProperty.Name)
            {
                switch (Picture.UploadProgress)
                {
                    case 0:
                        ProgressVisibility = Visibility.Hidden;
                        CompletedVisibility = Visibility.Hidden;
                        break;

                    case 100:
                        ProgressVisibility = Visibility.Hidden;
                        CompletedVisibility = Visibility.Visible;
                        break;
                   
                    default:
                        ProgressVisibility = Visibility.Visible;
                        CompletedVisibility = Visibility.Hidden;
                        break;
                }
                Progress = Picture.UploadProgress;
            }
        }

        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == AccountSettings.PicturePreviewDisplayModeProperty.Name)
            {
                UpdateDisplayModeProperties();
            }
        }

        #endregion

        #region Helper Methods

        public Batch GetBatch()
        {
            return this.Picture.GetBatch(this.PictureList.BatchList);
        }

        private void UpdateDisplayModeProperties()
        {
            var mode = this.Account.Settings.PicturePreviewDisplayMode;

            var fileExists = File.Exists(this.Picture.FileName);
            if (FlickrClient.IsOnline() && (this.IsVideo || !fileExists) && !string.IsNullOrWhiteSpace(this.Picture.PreviewUrl) && mode == PicturePreviewDisplayMode.Thumbnail)
            {
                // There is an online preview URL (of the uploaded picture) and the local file
                // is a video (which we can't render) or doesn't exist anymore so use the online preview URL.
                this.ImageUrl = this.Picture.PreviewUrl;
                this.PreviewTitleVisibility = Visibility.Collapsed;
            }
            else
            {
                if (this.IsVideo)
                {
                    this.ImageUrl = App.GetResourceUrl("/Resources/Movie.png");
                    this.PreviewTitleVisibility = Visibility.Visible;
                }
                else
                {
                    if (fileExists && mode == PicturePreviewDisplayMode.Thumbnail)
                    {
                        this.ImageUrl = this.Picture.FileName;
                        this.PreviewTitleVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.ImageUrl = App.GetResourceUrl("/Resources/Picture.png");
                        this.PreviewTitleVisibility = Visibility.Visible;
                    }
                }
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Picture.PropertyChanged -= OnPicturePropertyChanged;
            this.Account.Settings.PropertyChanged -= OnSettingsPropertyChanged;
        }

        #endregion
    }
}