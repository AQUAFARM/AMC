using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JelleDruyts.Windows;
using Microsoft.Win32;
using Schedulr.Extensibility;
using Schedulr.Infrastructure;
using Schedulr.Messages;
using Schedulr.Models;

namespace Schedulr.ViewModels
{
    public class QueuedPicturesViewModel : PictureListViewModel
    {
        #region Overridden Members

        protected override ObservableCollection<Batch> GetBatchesForAccount(Account account)
        {
            return account.QueuedBatches;
        }

        protected override PictureDetailsUISettings GetUISettingsForAccount(Account account)
        {
            return account.Settings.QueuedPictureDetailsUISettings;
        }

        protected override bool IsDragDropSupportedOnQueue
        {
            get { return true; }
        }

        protected override IEnumerable<ICommand> GetQueueCommands()
        {
            return new RelayCommand[]
            {
                new RelayCommand(this.Add, this.CanAdd, "_Add...", "Adds files to the queue, each in its own batch [INS]", new KeyGesture(Key.Insert)),
                new RelayCommand(this.AddBatch, this.CanAddBatch, "Add _Batch...", "Adds files to the queue, all in the same batch [ALT-INS]", new KeyGesture(Key.Insert, ModifierKeys.Alt)),
                new RelayCommand(this.UploadSelection, this.CanUploadSelection, "_Upload...", "Uploads the currently selected files to Flickr"),
                new RelayCommand(this.UploadBatch, this.CanUploadBatch, "U_pload Batch...", "Uploads the selected batches of files to Flickr (or simply the first batch if nothing is selected)"),
                new RelayCommand(this.Shuffle, this.CanShuffle, "Shu_ffle", "Shuffles all files in the queue, each file becoming its own batch"),
                new RelayCommand(this.ShuffleBatch, this.CanShuffleBatch, "Shuff_le Batch", "Shuffles all files in the queue, all into the same batch"),
                this.ExpandAllBatchesCommand,
                this.CollapseAllBatchesCommand,
                this.RemoveCommand
            };
        }

        protected override bool ArePictureDetailsReadOnly
        {
            get { return false; }
        }

        protected override PictureEventType PictureRemovingEventType
        {
            get { return PictureEventType.RemovingFromQueue; }
        }

        protected override PictureEventType PictureRemovedEventType
        {
            get { return PictureEventType.RemovedFromQueue; }
        }

        #endregion

        #region Commands

        private bool CanAdd(object parameter)
        {
            return true;
        }

        private void Add(object parameter)
        {
            Add(false);
        }

        private bool CanAddBatch(object parameter)
        {
            return true;
        }

        private void AddBatch(object parameter)
        {
            Add(true);
        }

        private void Add(bool addToSingleBatch)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Title = (addToSingleBatch ? "Please select the files to add as a single batch" : "Please select the files to add as individual batches");
            if (openFileDialog.ShowDialog() == true)
            {
                Messenger.Send<AddPicturesRequestedMessage>(new AddPicturesRequestedMessage(openFileDialog.FileNames, addToSingleBatch));
            }
        }

        private bool CanUploadSelection(object parameter)
        {
            return (this.SelectedPictures.Count > 0 && FlickrClient.IsOnline());
        }

        private void UploadSelection(object parameter)
        {
            Messenger.Send<UploadPicturesRequestedMessage>(new UploadPicturesRequestedMessage(this.SelectedPictures, this.Account, UploadPicturesRequestReason.Interactive));
        }

        private bool CanUploadBatch(object parameter)
        {
            return (this.BatchList.Count > 0 && FlickrClient.IsOnline());
        }

        private void UploadBatch(object parameter)
        {
            IEnumerable<Batch> batches;
            if (this.SelectedPictures.Count > 0)
            {
                // If there's a selection, take all batches of the selected pictures.
                batches = this.SelectedPictures.Select(p => p.GetBatch(this.BatchList)).Distinct();
            }
            else
            {
                // If there's no selection, take the first batch.
                batches = new Batch[] { this.BatchList[0] };
            }
            foreach (var batch in batches)
            {
                Messenger.Send<UploadPicturesRequestedMessage>(new UploadPicturesRequestedMessage(batch, this.Account, UploadPicturesRequestReason.Interactive));
            }
        }

        private bool CanShuffle(object parameter)
        {
            return (this.PictureListViewModels.Count > 1);
        }

        private void Shuffle(object parameter)
        {
            Shuffle(false);
        }

        private bool CanShuffleBatch(object parameter)
        {
            return (this.PictureListViewModels.Count > 1);
        }

        private void ShuffleBatch(object parameter)
        {
            Shuffle(true);
        }

        private void Shuffle(bool addToSingleBatch)
        {
            var result = MessageBox.Show("All the files in the queue will be shuffled and all batch settings will be reset. Are you sure you want to continue?", "Confirm Shuffle", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                Tasks.Shuffle(this.Account, this.BatchList, addToSingleBatch);
            }
        }

        #endregion
    }
}