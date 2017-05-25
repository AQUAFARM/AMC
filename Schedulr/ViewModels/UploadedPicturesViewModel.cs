using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using JelleDruyts.Windows;
using Schedulr.Extensibility;
using Schedulr.Messages;
using Schedulr.Models;
using Schedulr.Providers;

namespace Schedulr.ViewModels
{
    public class UploadedPicturesViewModel : PictureListViewModel
    {
        #region Overridden Members

        protected override ObservableCollection<Batch> GetBatchesForAccount(Account account)
        {
            return account.UploadedBatches;
        }

        protected override PictureDetailsUISettings GetUISettingsForAccount(Account account)
        {
            return account.Settings.UploadedPictureDetailsUISettings;
        }

        protected override bool IsDragDropSupportedOnQueue
        {
            get { return false; }
        }

        protected override IEnumerable<ICommand> GetQueueCommands()
        {
            return new RelayCommand[]
            {
                new RelayCommand(this.Enqueue, this.CanEnqueue, "En_queue", "Adds the selected files back to the upload queue, each in its own batch [INS]", new KeyGesture(Key.Insert)),
                new RelayCommand(this.EnqueueBatch, this.CanEnqueueBatch, "Enqueue _Batch", "Adds the selected files back to the upload queue, all in the same batch [ALT-INS]", new KeyGesture(Key.Insert, ModifierKeys.Alt)),
                this.ExpandAllBatchesCommand,
                this.CollapseAllBatchesCommand,
                this.RemoveCommand
            };
        }

        protected override bool ArePictureDetailsReadOnly
        {
            get { return true; }
        }

        protected override PictureEventType PictureRemovingEventType
        {
            get { return PictureEventType.RemovingFromUploads; }
        }

        protected override PictureEventType PictureRemovedEventType
        {
            get { return PictureEventType.RemovedFromUploads; }
        }

        #endregion

        #region Commands

        private bool CanEnqueue(object parameter)
        {
            return (this.SelectedPictures.Count > 0);
        }

        private void Enqueue(object parameter)
        {
            Enqueue(false);
        }

        private bool CanEnqueueBatch(object parameter)
        {
            return (this.SelectedPictures.Count > 1);
        }

        private void EnqueueBatch(object parameter)
        {
            Enqueue(true);
        }

        private void Enqueue(bool addToSingleBatch)
        {
            var pictures = new List<Picture>();
            foreach (var selectedPicture in this.SelectedPictures)
            {
                var clone = SerializationProvider.Clone<Picture>(selectedPicture);
                PictureProvider.InitializePicture(clone);
                pictures.Add(clone);
            }
            Messenger.Send<AddPicturesRequestedMessage>(new AddPicturesRequestedMessage(pictures, addToSingleBatch));
        }

        #endregion
    }
}