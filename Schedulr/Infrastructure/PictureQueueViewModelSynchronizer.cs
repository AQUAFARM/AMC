using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Schedulr.Models;
using Schedulr.ViewModels;

namespace Schedulr.Infrastructure
{
    /// <summary>
    /// Synchronizes a collection of batches with a flattened collection of pictures.
    /// </summary>
    public sealed class PictureQueueViewModelSynchronizer : IDisposable
    {
        #region Fields

        private PictureListViewModel pictureList;

        #endregion

        #region Constructors

        public PictureQueueViewModelSynchronizer(PictureListViewModel pictureList)
        {
            this.pictureList = pictureList;
            ResetViewModels();

            this.pictureList.BatchList.CollectionChanged += new NotifyCollectionChangedEventHandler(OnBatchesCollectionChanged);
            foreach (var batch in this.pictureList.BatchList)
            {
                batch.Pictures.CollectionChanged += new NotifyCollectionChangedEventHandler(OnBatchPicturesCollectionChanged);
            }
        }

        #endregion

        #region Event Handlers

        private void OnBatchesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ResetViewModels();
            }
            else
            {
                if (e.NewItems != null)
                {
                    foreach (Batch newBatch in e.NewItems)
                    {
                        AddPictures(newBatch, newBatch.Pictures);
                        newBatch.Pictures.CollectionChanged += new NotifyCollectionChangedEventHandler(OnBatchPicturesCollectionChanged);
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (Batch oldBatch in e.OldItems)
                    {
                        oldBatch.Pictures.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnBatchPicturesCollectionChanged);
                        RemovePictures(oldBatch.Pictures);
                    }
                }
            }
        }

        private void OnBatchPicturesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var batch in this.pictureList.BatchList.ToArray())
                {
                    Tasks.RemoveBatchIfEmpty(this.pictureList.Account, this.pictureList.BatchList, batch);
                }
                ResetViewModels();
            }
            else
            {
                var batch = this.pictureList.BatchList.Single(b => b.Pictures == sender);
                if (e.NewItems != null)
                {
                    AddPictures(batch, e.NewItems.Cast<Picture>().ToList());
                }
                if (e.OldItems != null)
                {
                    RemovePictures(e.OldItems.Cast<Picture>().ToList());
                }
                Tasks.RemoveBatchIfEmpty(this.pictureList.Account, this.pictureList.BatchList, batch);
                if (batch.Pictures.Count > 0)
                {
                    // If the primary picture of the batch's PhotoSet was removed, set a new one.
                    if (!batch.Pictures.Any(p => p.FileName == batch.Photoset.PrimaryPictureId))
                    {
                        batch.Photoset.PrimaryPictureId = batch.Pictures.First().FileName;
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        private void ResetViewModels()
        {
            this.pictureList.PictureListViewModels.Clear();
            this.pictureList.PictureListViewModels.AddRange(
                this.pictureList.BatchList.SelectMany(
                    b => b.Pictures.Select(p => new PictureViewModel(p, this.pictureList))).ToList());
        }

        private void RemovePictures(IEnumerable<Picture> pictures)
        {
            this.pictureList.PictureListViewModels.RemoveRange(
                this.pictureList.PictureListViewModels.Where(plvm => pictures.Contains(plvm.Picture)).ToList());
        }

        private void AddPictures(Batch targetBatch, IEnumerable<Picture> pictures)
        {
            var picturesList = pictures.ToList();
            if (picturesList.Any())
            {
                // Find the previous picture in the batch, or if it's the first one, the last picture in the previous batch.
                Picture previousPicture = null;
                var pictureInBatchIndex = targetBatch.Pictures.IndexOf(picturesList.First());
                if (pictureInBatchIndex > 0)
                {
                    previousPicture = targetBatch.Pictures[pictureInBatchIndex - 1];
                }
                else
                {
                    var batchIndex = this.pictureList.BatchList.IndexOf(targetBatch);
                    if (batchIndex > 0)
                    {
                        previousPicture = this.pictureList.BatchList[batchIndex - 1].Pictures.Last();
                    }
                }

                // Find the index within the flat list to insert the picture.
                var pictureIndex = 0;
                if (previousPicture != null)
                {
                    // Find the index of the previous picture in the flat list.
                    var previousPictureViewModel = this.pictureList.PictureListViewModels.First(p => p.Picture == previousPicture);
                    var previousIndex = this.pictureList.PictureListViewModels.IndexOf(previousPictureViewModel);
                    if (previousIndex >= 0)
                    {
                        // Insert the picture right after the previous one.
                        pictureIndex = previousIndex + 1;
                    }
                }

                // Insert a new viewmodel for the picture at the determined index.
                this.pictureList.PictureListViewModels.InsertRange(pictureIndex, picturesList.Select(pic => new PictureViewModel(pic, this.pictureList)));
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            foreach (var batch in this.pictureList.BatchList)
            {
                batch.Pictures.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnBatchPicturesCollectionChanged);
            }
            this.pictureList.BatchList.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnBatchesCollectionChanged);
        }

        #endregion
    }
}