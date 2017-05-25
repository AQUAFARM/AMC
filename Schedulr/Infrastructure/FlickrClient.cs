using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using FlickrNet;
using JelleDruyts.Windows;
using Schedulr.Models;

namespace Schedulr.Infrastructure
{
    public class FlickrClient
    {
        #region Constants

        private const string ApiKey = "c296ee49a0e9a56020a6f7ec77891d23";
        private const string SharedSecret = "341dd12cf0ed29d5";

        #endregion

        #region Fields

        private Flickr flickr;
        private OAuthRequestToken requestToken;
        private object uploadingLock = new object();
        private Action<long> bytesUploadedCallback;
        private static TimeSpan uploadTimeout;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="FlickrClient"/> class.
        /// </summary>
        static FlickrClient()
        {
            var uploadTimeoutSecondsOverrideValue = ConfigurationManager.AppSettings["UploadTimeoutSeconds"];
            int uploadTimeoutSeconds;
            if (!int.TryParse(uploadTimeoutSecondsOverrideValue, out uploadTimeoutSeconds))
            {
                uploadTimeoutSeconds = 600;
            }
            FlickrClient.uploadTimeout = TimeSpan.FromSeconds(uploadTimeoutSeconds);

            // Disable the cache entirely, e.g. because it doesn't show new photosets after they were created.
            Flickr.CacheDisabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlickrClient"/> class.
        /// </summary>
        public FlickrClient()
            : this(null,null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlickrClient"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="tokensecret">The token secret.</param>
        public FlickrClient(string token, string tokensecret)
        {
            this.flickr = new Flickr(ApiKey, SharedSecret)
            {
                HttpTimeout = (int) Math.Min(FlickrClient.uploadTimeout.TotalMilliseconds, int.MaxValue),
                OAuthAccessToken = token,
                OAuthAccessTokenSecret = tokensecret
            };
            this.flickr.OnUploadProgress += OnUploadProgress;
        }

        #endregion

        #region IsOnline

        /// <summary>
        /// Gets a value that determines if Flickr is available.
        /// </summary>
        /// <returns><see langword="true"/> if a network connection is available, <see langword="false"/> otherwise.</returns>
        public static bool IsOnline()
        {
#if OFFLINE
            return true;
#else
            return Network.IsAvailable();
#endif
        }

        #endregion

        #region Authentication

        /// <summary>
        /// Begins the authentication process with Flickr and returns the user's authentication URL.
        /// </summary>
        /// <returns>The URL where the user should go next to authenticate online.</returns>
        /// <remarks>
        /// A so-called 'requestToken' will be requested and stored, and when the user has authenticated
        /// with Flickr through the returned authentication URL, call <see cref="EndAuthentication"/>
        /// to complete the process.
        /// </remarks>
        public string BeginAuthentication()
        {
            this.requestToken = flickr.OAuthGetRequestToken("oob");
            return flickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);
        }

        /// <summary>
        /// Ends the authentication process with Flickr and returns the user's authentication token.
        /// </summary>
        /// <returns>The user's authentication token.</returns>
        /// <remarks>
        /// The 'requestToken' will be used to verify that the user has now authenticated with Flickr.
        /// </remarks>
        public OAuthAccessToken EndAuthentication(string verifictionCode)
        {
            if (this.requestToken==null || string.IsNullOrEmpty(this.requestToken.Token))
            {
                throw new InvalidOperationException("Cannot end the authentication process because there is no so-called 'requestToken' that is created as the first step of the authentication process. Call BeginAuthentication first.");
            }
            var accessToken = flickr.OAuthGetAccessToken(this.requestToken, verifictionCode);
            this.flickr.OAuthAccessToken = accessToken.Token;
            this.flickr.OAuthAccessTokenSecret = accessToken.TokenSecret;
            this.requestToken = null;
            return accessToken;
        }

        #endregion

        #region User Info

        /// <summary>
        /// Gets general information about the user.
        /// </summary>
        /// <returns>General information about the user.</returns>
        public UserInfo GetUserInfo()
        {
            // Retrieve information.
            var authentication = this.flickr.AuthOAuthCheckToken();
            var user = this.flickr.PeopleGetInfo(authentication.User.UserId);
            var status = this.flickr.PeopleGetUploadStatus();
            var groups = new List<MemberGroupInfo>();
            var groupsPageIndex = 1;
            while (true)
            {
                var groupsPage = this.flickr.GroupsPoolsGetGroups(groupsPageIndex, 0);
                groups.AddRange(groupsPage);
                if (groupsPage.Page >= groupsPage.Pages)
                {
                    break;
                }
                groupsPageIndex++;
            }

            var sets = this.flickr.PhotosetsGetList();

            // Populate user information.
            var userInfo = new UserInfo();
            userInfo.BuddyIconUrl = user.BuddyIconUrl.ToString();
            userInfo.IsProUser = status.IsPro;
            userInfo.PicturesUrl = user.PhotosUrl.ToString();
            userInfo.UploadLimit = status.FileSizeMax;
            userInfo.UploadLimitVideo = status.VideoSizeMax;
            userInfo.UserName = user.UserName;
            userInfo.UserId = user.UserId;
            foreach (var set in sets)
            {
                userInfo.Sets.Add(new PictureCollection { Id = set.PhotosetId, ImageUrl = set.PhotosetSquareThumbnailUrl.ToString(), Name = set.Title, Size = set.NumberOfPhotos + set.NumberOfVideos });
            }
            foreach (var group in groups)
            {
                userInfo.Groups.Add(new PictureCollection { Id = group.GroupId, ImageUrl = group.GroupIconUrl.ToString(), Name = group.GroupName, Size = group.Photos });
            }
            return userInfo;
        }

        /// <summary>
        /// Gets the current status information about the user.
        /// </summary>
        /// <returns>The current status information about the user.</returns>
        public FlickrUserStatusInfo GetUserStatus()
        {
            var status = flickr.PeopleGetUploadStatus();
            return new FlickrUserStatusInfo(status.BandwidthUsed, status.BandwidthMax, status.PercentageUsed, status.VideosUploaded, status.VideosRemaining);
        }

        #endregion

        #region Upload

        /// <summary>
        /// Uploads the given picture.
        /// </summary>
        /// <param name="uploadingPicturesTask">The task that tracks uploading pictures.</param>
        /// <param name="account">The account.</param>
        /// <param name="picture">The picture to upload.</param>
        /// <param name="fileToUpload">The file to upload.</param>
        /// <param name="bytesUploadedCallback">The method that is called when the number of uploaded bytes changed.</param>
        /// <returns><see langword="true"/> if the picture was uploaded, <see langword="false"/> otherwise.</returns>
        public PictureUploadResult Upload(ApplicationTask uploadingPicturesTask, Account account, Picture picture, Stream fileToUpload, Action<long> bytesUploadedCallback)
        {
            var dateUploaded = DateTime.Now;
            var succeededOptionalSteps = new List<PictureUploadOptionalStep>();
            var failedOptionalSteps = new List<PictureUploadOptionalStep>();
            if (fileToUpload == null)
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, "The file to be uploaded does not exist or a plugin replaced it with an empty stream: \"{0}\"", picture.FileName);
                Logger.Log(errorMessage, TraceEventType.Error);
                uploadingPicturesTask.SetError(errorMessage);
                return new PictureUploadResult(picture, dateUploaded, PictureUploadStatus.FileDoesNotExist, succeededOptionalSteps, failedOptionalSteps, null);
            }
            else
            {
                lock (this.uploadingLock)
                {
                    try
                    {
                        Logger.Log(string.Format(CultureInfo.CurrentCulture, "Uploading file: \"{0}\"", picture.FileName), TraceEventType.Information);
                        this.bytesUploadedCallback = bytesUploadedCallback;

                        // Convert some properties to their corresponding Flickr types.
                        var isPublic = picture.VisibilityIsPublic.ValueOr(Picture.VisibilityIsPublicProperty.DefaultValue.ValueOr(true));
                        var isFamily = picture.VisibilityIsFamily.ValueOr(Picture.VisibilityIsFamilyProperty.DefaultValue.ValueOr(true));
                        var isFriend = picture.VisibilityIsFriend.ValueOr(Picture.VisibilityIsFriendProperty.DefaultValue.ValueOr(true));
                        var contentType = (FlickrNet.ContentType)picture.ContentType;
                        var safetyLevel = (SafetyLevel)picture.Safety;
                        var hiddenFromSearch = (HiddenFromSearch)picture.SearchVisibility;

                        // Upload the picture.
                        Exception exceptionToLog = null;
                        try
                        {
#if OFFLINE
                            picture.PictureId = "PICTURE-" + Guid.NewGuid().ToString() + "-" + picture.ShortFileName;
                            var debugOutputFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), picture.PictureId);
                            fileToUpload.CopyTo(debugOutputFileName);
#else
                            picture.PictureId = this.flickr.UploadPicture(fileToUpload, picture.FileName, picture.Title, picture.Description, picture.Tags, isPublic, isFamily, isFriend, contentType, safetyLevel, hiddenFromSearch);
#endif
                            dateUploaded = DateTime.Now;
                        }
                        catch (WebException e)
                        {
                            if (e.Status == WebExceptionStatus.RequestCanceled)
                            {
                                var message = string.Format(CultureInfo.CurrentCulture, "The file upload did not complete within the timeout period of {0}.", FlickrClient.uploadTimeout.ToString());
                                Logger.Log(message, TraceEventType.Error);
                                uploadingPicturesTask.SetError(message);
                                return new PictureUploadResult(picture, dateUploaded, PictureUploadStatus.Timeout, succeededOptionalSteps, failedOptionalSteps, null);
                            }
                            else
                            {
                                exceptionToLog = e;
                            }
                        }
                        catch (Exception e)
                        {
                            exceptionToLog = e;
                        }
                        if (exceptionToLog != null)
                        {
                            var message = string.Format(CultureInfo.CurrentCulture, "An exception occurred during uploading: {0}", exceptionToLog.Message);
                            Logger.Log(message, exceptionToLog);
                            uploadingPicturesTask.SetError(message);
                            return new PictureUploadResult(picture, dateUploaded, PictureUploadStatus.ExceptionOccurred, succeededOptionalSteps, failedOptionalSteps, exceptionToLog);
                        }

                        // Something went wrong during upload and no picture ID is present; abort.
                        if (string.IsNullOrEmpty(picture.PictureId))
                        {
                            return new PictureUploadResult(picture, dateUploaded, PictureUploadStatus.NoPictureId, succeededOptionalSteps, failedOptionalSteps, null);
                        }

                        // Add to sets.
                        var stepSucceeded = true;
                        foreach (string setId in picture.SetIds)
                        {
                            try
                            {
                                AddToPhotoset(picture, setId);
                            }
                            catch (Exception exc)
                            {
                                stepSucceeded = false;
                                uploadingPicturesTask.SetWarning(string.Format(CultureInfo.CurrentCulture, "Could not add file \"{0}\" to photo set \"{1}\"", picture.ShortFileName, setId), exc);
                            }
                        }
                        (stepSucceeded ? succeededOptionalSteps : failedOptionalSteps).Add(PictureUploadOptionalStep.AddToSets);

                        // Add to groups.
                        stepSucceeded = true;
                        foreach (string groupId in picture.GroupIds)
                        {
                            exceptionToLog = null;
                            try
                            {
                                Logger.Log("Adding file to group: " + groupId, TraceEventType.Information);
#if !OFFLINE
                                flickr.GroupsPoolsAdd(picture.PictureId, groupId);
#endif
                            }
                            catch (FlickrApiException fae)
                            {
                                if (fae.Code == 6) // Your Photo has been added to the Pending Queue for this Pool
                                {
                                    Logger.Log("The file has been added to the Pending Queue for the group", TraceEventType.Information);
                                }
                                else
                                {
                                    exceptionToLog = fae;
                                }
                            }
                            catch (Exception exc)
                            {
                                exceptionToLog = exc;
                            }
                            if (exceptionToLog != null)
                            {
                                var errorMessage = string.Format(CultureInfo.CurrentCulture, "Could not add file \"{0}\" to group {1}", picture.ShortFileName, groupId);
                                Logger.Log(errorMessage, exceptionToLog, TraceEventType.Warning);
                                uploadingPicturesTask.SetWarning(errorMessage, exceptionToLog);
                            }
                        }
                        (stepSucceeded ? succeededOptionalSteps : failedOptionalSteps).Add(PictureUploadOptionalStep.AddToGroups);

                        // Set the license if requested.
                        stepSucceeded = true;
                        if (picture.License.HasValue && picture.License != Schedulr.Models.License.None)
                        {
                            try
                            {
                                Logger.Log("Setting license on file: " + picture.License.Value.ToString(), TraceEventType.Information);
#if !OFFLINE
                                this.flickr.PhotosLicensesSetLicense(picture.PictureId, (LicenseType)picture.License.Value);
#endif
                            }
                            catch (Exception exc)
                            {
                                var errorMessage = string.Format(CultureInfo.CurrentCulture, "Could not set license on file \"{0}\" to {1}", picture.ShortFileName, picture.License.Value.ToString());
                                Logger.Log(errorMessage, exc, TraceEventType.Warning);
                                uploadingPicturesTask.SetWarning(errorMessage, exc);
                            }
                        }
                        (stepSucceeded ? succeededOptionalSteps : failedOptionalSteps).Add(PictureUploadOptionalStep.SetLicense);

                        // Set the location if requested.
                        stepSucceeded = true;
                        if (picture.Location != null)
                        {
                            // http://www.flickr.com/services/api/flickr.photos.geo.setLocation.html
                            // Sets the geo data (latitude and longitude and, optionally, the accuracy level) for a photo.
                            // Before users may assign location data to a photo they must define who, by default, may view that information.
                            // Users can edit this preference at http://www.flickr.com/account/geo/privacy/.
                            // If a user has not set this preference, the API method will return an error.
                            try
                            {
                                Logger.Log("Setting geo location on file: " + picture.Location.ToString(), TraceEventType.Information);
                                var accuracy = (GeoAccuracy)picture.Location.Accuracy;
#if !OFFLINE
                                flickr.PhotosGeoSetLocation(picture.PictureId, picture.Location.Latitude, picture.Location.Longitude, accuracy);
#endif
                            }
                            catch (Exception exc)
                            {
                                var errorMessage = string.Format(CultureInfo.CurrentCulture, "Could not set geo location on file \"{0}\" to {1}", picture.ShortFileName, picture.Location.ToString());
                                Logger.Log(errorMessage, exc, TraceEventType.Warning);
                                uploadingPicturesTask.SetWarning(errorMessage, exc);
                            }
                        }
                        (stepSucceeded ? succeededOptionalSteps : failedOptionalSteps).Add(PictureUploadOptionalStep.SetGeoLocation);

                        // Get some details.
                        picture.ShouldRefreshWebInfo = true;
                        RefreshWebInfo(picture);

                        return new PictureUploadResult(picture, dateUploaded, PictureUploadStatus.Succeeded, succeededOptionalSteps, failedOptionalSteps, null);
                    }
                    finally
                    {
                        this.bytesUploadedCallback = null;
                    }
                }
            }
        }

        /// <summary>
        /// Called when the upload progress changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FlickrNet.UploadProgressEventArgs"/> instance containing the event data.</param>
        private void OnUploadProgress(object sender, UploadProgressEventArgs e)
        {
            if (this.bytesUploadedCallback != null)
            {
                this.bytesUploadedCallback(e.BytesSent);
            }
        }

        #endregion

        #region Photosets

        /// <summary>
        /// Creates a photoset.
        /// </summary>
        /// <param name="title">The title of the photoset.</param>
        /// <param name="description">The description of the photoset.</param>
        /// <param name="primaryPictureId">The ID of the primary picture for the photoset.</param>
        /// <returns>The ID of the photoset that was created.</returns>
        public string CreatePhotoset(string title, string description, string primaryPictureId)
        {
            try
            {
                Logger.Log(string.Format(CultureInfo.CurrentCulture, "Creating photoset \"{0}\"", title), TraceEventType.Information);
#if OFFLINE
                return "SET-" + Guid.NewGuid().ToString() + "-" + title;
#else
                var set = this.flickr.PhotosetsCreate(title, description, primaryPictureId);
                return set.PhotosetId;
#endif
            }
            catch (Exception exc)
            {
                Logger.Log(string.Format(CultureInfo.CurrentCulture, "Could not create a photoset with title \"{0}\".", title), exc, TraceEventType.Warning);
                throw;
            }
        }

        /// <summary>
        /// Adds a picture to a photoset.
        /// </summary>
        /// <param name="picture">The picture to add to the photoset.</param>
        /// <param name="photosetId">The ID of the photoset.</param>
        public void AddToPhotoset(Picture picture, string photosetId)
        {
            try
            {
                Logger.Log(string.Format(CultureInfo.CurrentCulture, "Adding file \"{0}\" to photoset \"{1}\"", picture.Title, photosetId), TraceEventType.Information);
#if !OFFLINE
                flickr.PhotosetsAddPhoto(photosetId, picture.PictureId);
#endif
            }
            catch (Exception exc)
            {
                Logger.Log(string.Format(CultureInfo.CurrentCulture, "Could not add file \"{0}\" to photoset \"{1}\"", picture.ShortFileName, photosetId), exc, TraceEventType.Warning);
                throw;
            }
        }

        /// <summary>
        /// Sets a picture as a photoset's main photo
        /// </summary>
        /// <param name="picture">The picture to set as the main photo.</param>
        /// <param name="photosetId">The ID of the photoset.</param>
        public void SetPhotosetPrimaryPhoto(Picture picture, string photosetId)
        {
            try
            {
                Logger.Log(string.Format(CultureInfo.CurrentCulture, "Changing photoset \"{0}\" primary photo to \"{1}\"", photosetId, picture.Title), TraceEventType.Information);
#if !OFFLINE
                flickr.PhotosetsSetPrimaryPhoto(photosetId, picture.PictureId);
#endif
            }
            catch (Exception exc)
            {
                Logger.Log(string.Format(CultureInfo.CurrentCulture, "Could not change primary photo of photoset \"{0}\" to photoset \"{1}\"", photosetId, picture.Title), exc, TraceEventType.Warning);
                throw;
            }
        }

        #endregion

        #region RefreshWebInfo

        /// <summary>
        /// Refreshes the web information of the specified picture.
        /// </summary>
        /// <param name="picture">The picture to refresh.</param>
        public void RefreshWebInfo(Picture picture)
        {
            try
            {
#if !OFFLINE
                var info = flickr.PhotosGetInfo(picture.PictureId);
                picture.PreviewUrl = info.SmallUrl;
                picture.WebUrl = info.WebUrl;
                picture.ShouldRefreshWebInfo = (info.VideoInfo != null && info.VideoInfo.Pending);
#endif
            }
            catch (Exception exc)
            {
                var flickrException = exc as FlickrApiException;
                if (flickrException != null && flickrException.Code == 1)
                {
                    // Error Code 1 = Photo not found, so do not refresh again.
                    picture.ShouldRefreshWebInfo = false;
                }
                else
                {
                    var errorMessage = string.Format(CultureInfo.CurrentCulture, "Could not get web details for picture with ID \"{0}\". This is probably because it's a video that is still being processed, in which case it should be finished in a couple of minutes.", picture.PictureId);
                    Logger.Log(errorMessage, exc, TraceEventType.Warning);
                }
            }
        }

        #endregion
    }
}