using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using JelleDruyts.Windows;
using Schedulr.Extensibility;
using Schedulr.Messages;
using Schedulr.Models;
using Schedulr.Providers;
using Schedulr.ViewModels;

namespace Schedulr.Infrastructure
{
    /// <summary>
    /// Performs common tasks.
    /// </summary>
    public static class Tasks
    {
        #region Fields

        private static object uploadingLockObject = new object();

        #endregion

        #region Upload Pictures

        /// <summary>
        /// Uploads the specified pictures asynchronously.
        /// </summary>
        /// <param name="account">The account for which to upload the pictures.</param>
        /// <param name="batch">The batch to upload.</param>
        /// <param name="postUploadCallback">The method to call when the upload has finished.</param>
        /// <returns><see langword="true"/> if pictures are being uploaded in the background upon return, <see langword="false"/> otherwise.</returns>
        public static bool UploadBatch(Account account, Batch batch, Action postUploadCallback)
        {
            return UploadPictures(account, batch, batch.Pictures, postUploadCallback);
        }

        /// <summary>
        /// Uploads the specified pictures asynchronously.
        /// </summary>
        /// <param name="account">The account for which to upload the pictures.</param>
        /// <param name="pictures">The pictures to upload.</param>
        /// <param name="postUploadCallback">The method to call when the upload has finished.</param>
        /// <returns><see langword="true"/> if pictures are being uploaded in the background upon return, <see langword="false"/> otherwise.</returns>
        public static bool UploadPictures(Account account, ICollection<Picture> pictures, Action postUploadCallback)
        {
            return UploadPictures(account, null, pictures, postUploadCallback);
        }

        /// <summary>
        /// Uploads the specified pictures asynchronously.
        /// </summary>
        /// <param name="account">The account for which to upload the pictures.</param>
        /// <param name="batch">The batch to upload, or <see langword="null"/> if no batch is being uploaded (but pictures are uploaded separately).</param>
        /// <param name="pictures">The pictures to upload.</param>
        /// <param name="postUploadCallback">The method to call when the upload has finished.</param>
        /// <returns><see langword="true"/> if pictures are being uploaded in the background upon return, <see langword="false"/> otherwise.</returns>
        private static bool UploadPictures(Account account, Batch batch, ICollection<Picture> pictures, Action postUploadCallback)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            if (pictures == null || pictures.Count == 0)
            {
                Logger.Log("An upload was requested but no files were found to upload.", TraceEventType.Information);
                return false;
            }

            if (!FlickrClient.IsOnline())
            {
                Logger.Log("An upload was requested but Flickr is offline or cannot be reached.", TraceEventType.Warning);
                return false;
            }

            var totalSteps = pictures.Count * 10;

            // See if a photoset needs to be created.
            Photoset photosetToCreate = null;
            Picture photosetPrimaryPicture = null;
            if (batch != null && batch.CreatePhotosetForBatch && batch.Photoset != null && batch.Photoset.PrimaryPictureId != null && pictures.Any(p => p.FileName == batch.Photoset.PrimaryPictureId))
            {
                photosetPrimaryPicture = batch.Pictures.FirstOrDefault(p => p.FileName == batch.Photoset.PrimaryPictureId);
                if (photosetPrimaryPicture != null)
                {
                    photosetToCreate = batch.Photoset;
                }
                else
                {
                    Logger.Log("The primary picture for the photoset of the batch being uploaded is not part of the batch.", TraceEventType.Warning);
                }
                totalSteps += 10;
            }

            Logger.Log(string.Format(CultureInfo.CurrentCulture, "Uploading {0}", pictures.Count.ToCountString("file")), TraceEventType.Information);
            var uploadingPicturesTask = new ApplicationTask("Uploading files", totalSteps);
            Messenger.Send(new TaskStatusMessage(uploadingPicturesTask));
            var step = 0;

            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                lock (uploadingLockObject)
                {
                    var uploadedPictures = new List<PictureUploadResult>();
                    var i = 0;
                    var flickr = new FlickrClient(account.AuthenticationToken, account.TokenSecret);

                    // Upload all pictures.
                    foreach (var picture in pictures)
                    {
                        
                        uploadingPicturesTask.SetProgress(step, string.Format(CultureInfo.CurrentCulture, "Uploading \"{0}\" ({1} of {2})", picture.Title, ++i, pictures.Count));
                        step += 9;
                        var warningBefore = uploadingPicturesTask.IsWarning;
                        var counter = 0;
                        while (counter < account.Settings.UploadRetryAttempts)
                        {
                            if (counter > 0)
                            {
                                uploadingPicturesTask.SetWarning(string.Format(CultureInfo.CurrentCulture, "Retrying to upload \"{0}\" ({1} of {2}) attempt {3}", picture.Title, i, pictures.Count, counter + 1));
                            }
                            try
                            {
                                PictureUploadResult result;
                                Stream fileToRender = null;
                                if (File.Exists(picture.FileName))
                                {
                                    fileToRender = File.OpenRead(picture.FileName);
                                }
                                var isVideo = App.IsVideoFile(picture.FileName);
                                var pictureBatch = picture.GetBatch(account.QueuedBatches);
                                PluginManager.OnPictureEvent(new PictureEventArgs(PictureEventType.Uploading, App.Info,
                                    account, pictureBatch, picture, isVideo));

                                try
                                {
                                    var modifiedFileToRender =
                                        PluginManager.OnRenderingFile(
                                            new RenderingEventArgs(App.Info, picture, isVideo), fileToRender);
                                    result = flickr.Upload(uploadingPicturesTask, account, picture, modifiedFileToRender,
                                        bytesUploaded =>
                                        {
                                            var percent = (double) bytesUploaded/picture.FileSize;
                                            picture.UploadProgress = (short) Math.Round(percent*100, 0);
                                            uploadingPicturesTask.SetProgressForCurrentStep(percent);
                                        });
                                }
                                finally
                                {
                                    PluginManager.OnRenderingFileCompleted(new RenderingEventArgs(App.Info, picture,
                                        isVideo));
                                    if (fileToRender != null)
                                    {
                                        try
                                        {
                                            fileToRender.Dispose();
                                        }
                                        catch (ObjectDisposedException)
                                        {
                                            // Ignore exceptions from already disposed streams.
                                        }
                                    }
                                }
                                uploadedPictures.Add(result);

                                if (result.Error == null)
                                {
                                    picture.UploadProgress = 100;
                                    counter = account.Settings.UploadRetryAttempts;

                                    //if before this step there was no warning but now it is in warning state
                                    //and the process finished after retry, we reset the warning state
                                    if (!warningBefore && uploadingPicturesTask.IsWarning)
                                        uploadingPicturesTask.ResetWarning();
                                }
                                else
                                {
                                    counter++;
                                }
                            }
                            catch (Exception exc)
                            {
                                var errorMessage = string.Format(CultureInfo.CurrentCulture,
                                    "An error occurred while uploading \"{0}\"", picture.Title);
                                Logger.Log(errorMessage, exc);
                                uploadingPicturesTask.SetError(errorMessage, exc);
                                picture.UploadProgress = 0;
                                counter++;
                            }
                        }
                    }

                    // See if a photoset needs to be created.
                    if (photosetToCreate != null && !string.IsNullOrEmpty(photosetPrimaryPicture.PictureId))
                    {
                        uploadingPicturesTask.SetProgress(step, string.Format(CultureInfo.CurrentCulture, "Creating photoset \"{0}\"", photosetToCreate.Name));
                        step += 1;

                        try
                        {
                            var setId = flickr.CreatePhotoset(photosetToCreate.Name, photosetToCreate.Description, photosetPrimaryPicture.PictureId);
                            photosetToCreate.Id = setId;
                            uploadingPicturesTask.SetProgressForCurrentStep(0.5);
                        }
                        catch (Exception exc)
                        {
                            uploadingPicturesTask.SetWarning(string.Format(CultureInfo.CurrentCulture, "Could not create photoset \"{0}\" for batch", photosetToCreate.Name), exc);
                        }

                        uploadingPicturesTask.SetProgress(step, string.Format(CultureInfo.CurrentCulture, "Adding {0} to photoset \"{1}\"", pictures.Count.ToCountString("file"), photosetToCreate.Name));
                        step += 9;

                        // Update all pictures in this batch to belong to the photoset.
                        i = 0;
                        foreach (var batchPicture in pictures)
                        {
                            i++;
                            if (!batchPicture.SetIds.Contains(photosetToCreate.Id))
                            {
                                try
                                {
                                    if (batchPicture.FileName != batch.Photoset.PrimaryPictureId)
                                    {
                                        // The primary picture is already made part of the set when it is created.
                                        flickr.AddToPhotoset(batchPicture, photosetToCreate.Id);
                                    }
                                    batchPicture.SetIds.Add(photosetToCreate.Id);
                                    uploadingPicturesTask.SetProgressForCurrentStep(0.5 + ((double)i / ((double)2 * pictures.Count)));
                                }
                                catch (Exception exc)
                                {
                                    uploadingPicturesTask.SetWarning(string.Format(CultureInfo.CurrentCulture, "Could not add \"{0}\" to photoset \"{1}\"", batchPicture.Title, photosetToCreate.Name), exc);
                                }
                            }
                        }
                    }

                    e.Result = uploadedPictures;
                }
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    Logger.Log("An unexpected exception occurred while uploading files", e.Error);
                    uploadingPicturesTask.SetError(e.Error);
                    uploadingPicturesTask.SetComplete("An unexpected exception occurred");
                }
                else
                {
                    var uploadResults = (List<PictureUploadResult>)e.Result;
                    try
                    {
                        Batch uploadBatch;
                        if (batch != null)
                        {
                            // Create a *copy* of the source batch, so that for possible partial failures
                            // the source batch is still available (containing the pictures that failed).
                            uploadBatch = SerializationProvider.Clone<Batch>(batch);
                            uploadBatch.Pictures.Clear();
                            uploadBatch.Photoset.PrimaryPictureId = null;
                        }
                        else
                        {
                            uploadBatch = new Batch();
                        }
                        var i = 0;
                        foreach (var uploadResult in uploadResults)
                        {
                            if (uploadResult.Status == PictureUploadStatus.Succeeded)
                            {
                                var picture = uploadResult.Picture;
                                uploadingPicturesTask.SetProgress(step, string.Format(CultureInfo.CurrentCulture, "Dequeueing \"{0}\" ({1} of {2})", picture.Title, ++i, uploadResults.Count));
                                picture.DateUploaded = uploadResult.Date;
                            }
                            step++;
                        }

                        //we create a list with all successfully uploaded pictures
                        var successfullyUploaded =
                            uploadResults.Where(uploadResult => uploadResult.Status == PictureUploadStatus.Succeeded).ToList();

                        //we add the uploaded pictures to the uploade batch
                        uploadBatch.Pictures.AddRange(successfullyUploaded.Select(up => up.Picture));

                        //we remove the uploaded pictures from the batch
                        foreach (var queuedBatch in account.QueuedBatches.Where(
                            qb => qb.Pictures.Any(pic => successfullyUploaded.Select(up => up.Picture).Contains(pic))).ToList())
                        {
                            queuedBatch.Pictures.RemoveRange(queuedBatch.Pictures.Where(pic => successfullyUploaded.Any(up => up.Picture == pic)));
                            RemoveBatchIfEmpty(account, account.QueuedBatches, batch);
                        }

                        // If any picture uploaded successfully, add the upload batch.
                        if (uploadBatch.Pictures.Any())
                        {
                            if (uploadBatch.Pictures.Contains(photosetPrimaryPicture))
                            {
                                // If the primary photoset picture was uploaded successfully, use it as the primary picture of the uploaded batch.
                                uploadBatch.Photoset.PrimaryPictureId = batch.Photoset.PrimaryPictureId;
                            }
                            account.UploadedBatches.Insert(0, uploadBatch);
                        }
                    }
                    finally
                    {
                        Messenger.Send<PictureQueueChangedMessage>(new PictureQueueChangedMessage(PictureQueueChangedReason.PicturesUploaded));
                        foreach (var uploadResult in uploadResults)
                        {
                            // Assume the batch that contains the picture has uploaded.
                            Batch pictureBatch = uploadResult.Picture.GetBatch(account.UploadedBatches);
                            if (pictureBatch == null)
                            {
                                // If not, the picture upload probably failed so the batch should still be queued.
                                pictureBatch = uploadResult.Picture.GetBatch(account.QueuedBatches);
                            }
                            PluginManager.OnPictureEvent(new PictureEventArgs(PictureEventType.Uploaded, App.Info, account, pictureBatch, uploadResult.Picture, App.IsVideoFile(uploadResult.Picture.FileName), uploadResult));
                        }
                        uploadingPicturesTask.SetComplete(uploadResults.Count.ToCountString("file", null, " uploaded"));
                    }
                }

                if (postUploadCallback != null)
                {
                    postUploadCallback();
                }
            };

            worker.RunWorkerAsync();

            return true;
        }

        #endregion

        #region Add Pictures To Queue

        /// <summary>
        /// Adds the specified pictures to the upload queue asynchronously.
        /// </summary>
        /// <param name="account">The account for which to add the pictures.</param>
        /// <param name="fileNames">The file names of the pictures to load and add.</param>
        /// <param name="pictures">The pictures to add.</param>
        /// <param name="addToSingleBatch">A value that determines if all the pictures should be added to a single batch or if each should go in its own batch.</param>
        public static void AddPicturesToQueue(Account account, ICollection<string> fileNames, ICollection<Picture> pictures, bool addToSingleBatch)
        {
            if (fileNames == null)
            {
                fileNames = new string[0];
            }
            if (pictures == null)
            {
                pictures = new Picture[0];
            }
            var addingPicturesTask = new ApplicationTask("Adding files", (fileNames.Count + pictures.Count) * 10);
            Messenger.Send(new TaskStatusMessage(addingPicturesTask));
            var step = 0;

            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                addingPicturesTask.Status = string.Format(CultureInfo.CurrentCulture, "Adding {0}{1} for account \"{2}\"", (fileNames.Count + pictures.Count).ToCountString("picture"), (addToSingleBatch ? " in a single batch" : string.Empty), account.Name);
                var allPictures = new List<Picture>();
                var i = 0;
                // Take a copy of the queue so it cannot be modified by another thread while looping over it to find already queued pictures.
                var queuedPictures = account.QueuedBatches.SelectMany(b => b.Pictures);
                foreach (var fileName in fileNames)
                {
                    if (queuedPictures.Any(p => string.Equals(Path.GetFullPath(fileName), Path.GetFullPath(p.FileName), StringComparison.OrdinalIgnoreCase)))
                    {
                        addingPicturesTask.SetProgress(step, string.Format(CultureInfo.CurrentCulture, "Skipping \"{0}\" because it is already queued ({1} of {2})", Path.GetFileName(fileName), ++i, fileNames.Count));
                        Logger.Log(string.Format(CultureInfo.CurrentCulture, "Skipping \"{0}\" because it is already queued", Path.GetFileName(fileName)), TraceEventType.Information);
                    }
                    else
                    {
                        try
                        {
                            addingPicturesTask.SetProgress(step, string.Format(CultureInfo.CurrentCulture, "Loading \"{0}\" ({1} of {2})", Path.GetFileName(fileName), ++i, fileNames.Count));
                            var picture = PictureProvider.GetPicture(fileName, account.Settings.PictureDefaults);
                            allPictures.Add(picture);
                        }
                        catch (Exception exc)
                        {
                            var errorMessage = string.Format(CultureInfo.CurrentCulture, "An error occurred while loading \"{0}\"", fileName);
                            Logger.Log(errorMessage, exc);
                            addingPicturesTask.SetError(errorMessage, exc);
                        }
                    }
                    step += 9;
                }
                foreach (var picture in pictures)
                {
                    addingPicturesTask.SetProgress(step, string.Format(CultureInfo.CurrentCulture, "Loading \"{0}\" ({1} of {2})", picture.Title, ++i, pictures.Count));
                    step += 9;
                    allPictures.Add(picture);
                }
                e.Result = allPictures;
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    Logger.Log("An unexpected exception occurred while adding files", e.Error);
                    addingPicturesTask.SetError(e.Error);
                    addingPicturesTask.SetComplete("An unexpected exception occurred");
                }
                else
                {
                    var allPictures = (IList<Picture>)e.Result;
                    try
                    {
                        Batch batch = null;
                        var i = 0;
                        foreach (var picture in allPictures)
                        {
                            addingPicturesTask.SetProgress(step, string.Format(CultureInfo.CurrentCulture, "Adding \"{0}\" ({1} of {2})", picture.Title, ++i, allPictures.Count));
                            step++;
                            if (batch == null || !addToSingleBatch)
                            {
                                batch = new Batch(picture);
                                PluginManager.OnBatchEvent(new BatchEventArgs(BatchEventType.Adding, App.Info, account, batch));
                                account.QueuedBatches.Add(batch);
                                PluginManager.OnBatchEvent(new BatchEventArgs(BatchEventType.Added, App.Info, account, batch));
                            }
                            PluginManager.OnPictureEvent(new PictureEventArgs(PictureEventType.Adding, App.Info, account, batch, picture, App.IsVideoFile(picture.FileName)));
                            if (!addToSingleBatch)
                            {
                                batch.Pictures.Add(picture);
                                PluginManager.OnPictureEvent(new PictureEventArgs(PictureEventType.Added, App.Info,
                                    account, batch, picture, App.IsVideoFile(picture.FileName)));
                            }
                        }
                        if (batch != null && addToSingleBatch)
                        {
                            batch.Pictures.AddRange(allPictures);

                            foreach (var picture in allPictures)
                            {
                                PluginManager.OnPictureEvent(new PictureEventArgs(PictureEventType.Added, App.Info,
                                    account, batch, picture, App.IsVideoFile(picture.FileName)));
                            }
                        }
                    }
                    finally
                    {
                        Messenger.Send<PictureQueueChangedMessage>(new PictureQueueChangedMessage(PictureQueueChangedReason.PicturesAdded));
                        addingPicturesTask.SetComplete(allPictures.Count.ToCountString("file", null, " added"));
                    }
                }
            };
            worker.RunWorkerAsync();
        }

        #endregion

        #region Update Accounts

        /// <summary>
        /// Updates the specified account.
        /// </summary>
        /// <param name="account">The account to update.</param>
        public static void UpdateAccount(AccountViewModel account)
        {
            UpdateAccounts(new AccountViewModel[] { account });
        }

        /// <summary>
        /// Updates the specified accounts.
        /// </summary>
        /// <param name="accounts">The accounts to update.</param>
        public static void UpdateAccounts(IList<AccountViewModel> accounts)
        {
            if (accounts == null || accounts.Count == 0)
            {
                return;
            }

            // Create a copy of the accounts collection so that it cannot be modified while foreaching below.
            accounts = accounts.ToArray();

            // Start a task for updating the accounts.
            var updatingAccountsTask = new ApplicationTask("Updating accounts", accounts.Count * 10);
            Messenger.Send(new TaskStatusMessage(updatingAccountsTask));
            var step = 0;

            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                foreach (var account in accounts)
                {
                    updatingAccountsTask.SetProgress(step, "Loading account information for " + account.Account.Name);
                    step += 9;
                    account.UpdatePrepare(updatingAccountsTask);
                }
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    Logger.Log("An unexpected exception occurred while updating the accounts", e.Error);
                    updatingAccountsTask.SetError(e.Error);
                    updatingAccountsTask.SetComplete("An unexpected exception occurred");
                }
                else
                {
                    var succeededCount = 0;
                    try
                    {
                        foreach (var account in accounts)
                        {
                            updatingAccountsTask.SetProgress(step, "Processing account information for " + account.Account.Name);
                            step++;
                            var updateSucceeded = account.UpdateCommit(updatingAccountsTask);
                            if (updateSucceeded)
                            {
                                succeededCount++;
                            }
                        }
                    }
                    finally
                    {
                        var message = succeededCount.ToCountString("account", null, " updated");
                        if (succeededCount != accounts.Count)
                        {
                            message += string.Format(CultureInfo.CurrentCulture, ", {0} failed", (accounts.Count - succeededCount));
                        }
                        updatingAccountsTask.SetComplete(message);
                    }
                }
            };
            worker.RunWorkerAsync();
        }

        #endregion

        #region Picture Batch Handling

        /// <summary>
        /// Changes the batch of a group of selected pictures.
        /// </summary>
        /// <param name="selectedPictures">The selected pictures for which to set the new batch.</param>
        /// <param name="pictureList">The list to which the selected pictures belong.</param>
        /// <param name="targetBatch">The batch to change the selected pictures to.</param>
        /// <param name="newBatchRequestedBefore"><see langword="true"/> if a new batch is requested before the target batch, <see langword="false"/> otherwise.</param>
        /// <param name="newBatchRequestedAfter"><see langword="true"/> if a new batch is requested after the target batch, <see langword="false"/> otherwise.</param>
        public static void ChangeBatch(IEnumerable<Picture> selectedPictures, PictureListViewModel pictureList, Batch targetBatch, bool newBatchRequestedBefore, bool newBatchRequestedAfter)
        {
            if (newBatchRequestedBefore || newBatchRequestedAfter)
            {
                var newBatchIndex = pictureList.BatchList.IndexOf(targetBatch);
                if (newBatchRequestedAfter)
                {
                    newBatchIndex += 1;
                }

                // Don't create a new batch if all pictures of an entire batch are selected (check only for the batch of the first selected picture).
                // There may be more pictures selected but at least we maintain the batch of the first picture's batch.
                var firstPicture = selectedPictures.First();
                var candidateNewBatch = firstPicture.GetBatch(pictureList.BatchList);
                if (!candidateNewBatch.Pictures.Except(selectedPictures).Any())
                {
                    var targetIndex = pictureList.BatchList.IndexOf(targetBatch);
                    var currentIndex = pictureList.BatchList.IndexOf(candidateNewBatch);
                    if (currentIndex < targetIndex)
                    {
                        // If the batch comes from a position before the target batch, then take into account that
                        // its current spot will become empty and everything will move down.
                        newBatchIndex -= 1;
                    }

                    // Undo adding 1 if a batch was requested after the current batch since it will be moved to its current location.
                    if (newBatchRequestedAfter && candidateNewBatch == targetBatch)
                    {
                        newBatchIndex -= 1;
                    }
                    pictureList.BatchList.Move(currentIndex, newBatchIndex);
                    targetBatch = candidateNewBatch;
                }
                else
                {
                    targetBatch = new Batch(firstPicture);
                    PluginManager.OnBatchEvent(new BatchEventArgs(BatchEventType.Adding, App.Info, pictureList.Account, targetBatch));
                    pictureList.BatchList.Insert(newBatchIndex, targetBatch);
                    PluginManager.OnBatchEvent(new BatchEventArgs(BatchEventType.Added, App.Info, pictureList.Account, targetBatch));
                }
            }

            // Change the batch.
            foreach (var picture in selectedPictures)
            {
                // Find the index within the batch to insert the picture.
                var pictureViewModel = pictureList.PictureListViewModels.First(p => p.Picture == picture);
                var newIndexInBatch = 0;
                var currentIndexInList = pictureList.PictureListViewModels.IndexOf(pictureViewModel);
                if (currentIndexInList > 0)
                {
                    var currentIndexInBatch = targetBatch.Pictures.IndexOf(picture);
                    var previousPictureInList = pictureList.PictureListViewModels[currentIndexInList - 1];
                    var previousPictureIndexInBatch = targetBatch.Pictures.IndexOf(previousPictureInList.Picture);
                    if (previousPictureIndexInBatch >= 0)
                    {
                        // The picture isn't the first one in the batch, position it after the previous one.
                        newIndexInBatch = previousPictureIndexInBatch + 1;
                        if (currentIndexInBatch >= 0 && currentIndexInBatch < previousPictureIndexInBatch)
                        {
                            // If the picture comes from the same batch in a position before the previous picture in the batch,
                            // then take into account that its current spot will become empty and everything will move down.
                            newIndexInBatch = previousPictureIndexInBatch;
                        }
                    }
                }

                var previousBatch = picture.GetBatch(pictureList.BatchList);
                if (previousBatch != targetBatch)
                {
                    if (previousBatch != null)
                    {
                        previousBatch.Pictures.Remove(picture);
                        RemoveBatchIfEmpty(pictureList.Account, pictureList.BatchList, previousBatch);
                    }
                    targetBatch.Pictures.Insert(newIndexInBatch, picture);
                }
                else
                {
                    targetBatch.Pictures.Move(targetBatch.Pictures.IndexOf(picture), newIndexInBatch);
                }
            }

            Messenger.Send<PictureQueueChangedMessage>(new PictureQueueChangedMessage(PictureQueueChangedReason.PicturesMoved));
        }

        /// <summary>
        /// Removes the batch from the list if it is empty.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="batchList">The list that the batch is part of.</param>
        /// <param name="batch">The batch to check.</param>
        public static void RemoveBatchIfEmpty(Account account, IList<Batch> batchList, Batch batch)
        {
            if (batch != null && batch.Pictures.Count == 0)
            {
                // If the last picture was removed from a batch, remove it entirely.
                PluginManager.OnBatchEvent(new BatchEventArgs(BatchEventType.Removing, App.Info, account, batch));
                batchList.Remove(batch);
                PluginManager.OnBatchEvent(new BatchEventArgs(BatchEventType.Removed, App.Info, account, batch));
            }
        }

        #endregion

        #region Shuffle

        /// <summary>
        /// Shuffles the specified pictures.
        /// </summary>
        /// <param name="account">The account that contains the pictures.</param>
        /// <param name="batches">The batches containing the pictures to shuffle.</param>
        /// <param name="addToSingleBatch">Determines if the shuffled pictures should be added to a single batch or if they should each be placed in their own batch.</param>
        public static void Shuffle(Account account, ObservableCollection<Batch> batches, bool addToSingleBatch)
        {
            // Select all pictures to a flat list.
            var pictures = batches.SelectMany(b => b.Pictures).ToList();

            // Clear all batches individually so they can be removed and the plugin event can still be raised.
            foreach (var batch in batches.ToArray())
            {
                batch.Pictures.Clear();
                RemoveBatchIfEmpty(account, batches, batch);
            }

            // Shuffle all pictures.
            var totalCount = pictures.Count;
            var shuffleTask = new ApplicationTask("Shuffling files", totalCount);
            Messenger.Send<TaskStatusMessage>(new TaskStatusMessage(shuffleTask));
            var step = 0;
            try
            {
                var random = new Random();
                Batch batch = null;
                while (pictures.Any())
                {
                    shuffleTask.SetProgress(step, string.Format(CultureInfo.CurrentCulture, "Shuffling file {0} of {1}", step + 1, totalCount));
                    var picture = pictures[random.Next(pictures.Count)];
                    if (batch == null || !addToSingleBatch)
                    {
                        batch = new Batch(picture);
                        PluginManager.OnBatchEvent(new BatchEventArgs(BatchEventType.Adding, App.Info, account, batch));
                        batches.Add(batch);
                        PluginManager.OnBatchEvent(new BatchEventArgs(BatchEventType.Added, App.Info, account, batch));
                    }
                    batch.Pictures.Add(picture);
                    pictures.Remove(picture);
                    step++;
                }
            }
            finally
            {
                shuffleTask.SetComplete("Shuffled " + totalCount.ToCountString("file"));
            }
            Messenger.Send<PictureQueueChangedMessage>(new PictureQueueChangedMessage(PictureQueueChangedReason.PicturesShuffled));
        }

        #endregion

        #region Check For Updates

        /// <summary>
        /// Checks for application updates.
        /// </summary>
        /// <param name="newVersionAvailableCallback">The callback method to invoke if there is a new version available, passing in the latest version and the download URL.</param>
        public static void CheckForUpdates(Action<Version, Uri> newVersionAvailableCallback)
        {
            // Don't bother checking if nobody wants to know.
            if (newVersionAvailableCallback != null)
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, e) =>
                {
                    // Check for the latest released version on a background thread.
                    if (CodePlexClient.IsOnline())
                    {
                        Uri downloadUrl;
                        var latestVersion = CodePlexClient.GetLatestReleasedVersion("schedulr", out downloadUrl);
                        if (latestVersion != null && latestVersion > App.FullVersion)
                        {
                            // If the latest released version is newer than the current, return the version and URL.
                            e.Result = new Tuple<Version, Uri>(latestVersion, downloadUrl);
                        }
                    }
                };
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        Logger.Log("An unexpected exception occurred while checking for updates", e.Error);
                    }
                    else
                    {
                        if (e.Result != null)
                        {
                            var result = (Tuple<Version, Uri>)e.Result;
                            newVersionAvailableCallback(result.Item1, result.Item2);
                        }
                    }
                };

                worker.RunWorkerAsync();
            }
        }

        #endregion
    }
}