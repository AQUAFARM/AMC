using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using JelleDruyts.Windows;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Extensibility;
using Schedulr.Infrastructure;
using Schedulr.Messages;
using Schedulr.Models;

namespace Schedulr.ViewModels
{
    public abstract class PictureListViewModel : ViewModel
    {
        #region Fields

        private PictureQueueViewModelSynchronizer synchronizer;
        private BatchGroupDescription batchGroupDescription;

        #endregion

        #region Properties

        protected RelayCommand RemoveCommand { get; private set; }
        protected RelayCommand ExpandAllBatchesCommand { get; private set; }
        protected RelayCommand CollapseAllBatchesCommand { get; private set; }
        public Account Account { get; private set; }
        public ObservableCollection<Batch> BatchList { get; private set; }
        public BulkObservableCollection<PictureViewModel> PictureListViewModels { get; private set; }
        protected IList<Picture> SelectedPictures { get; private set; }
        public PictureQueueViewModel PictureQueue { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets the picture details view model.
        /// </summary>
        public PictureDetailsViewModel PictureDetails
        {
            get { return this.GetValue(PictureDetailsProperty); }
            set { this.SetValue(PictureDetailsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PictureDetails"/> observable property.
        /// </summary>
        public static ObservableProperty<PictureDetailsViewModel> PictureDetailsProperty = new ObservableProperty<PictureDetailsViewModel, PictureListViewModel>(o => o.PictureDetails);

        #endregion

        #region Constructors

        protected PictureListViewModel()
        {
            this.RemoveCommand = new RelayCommand(this.Remove, this.CanRemove, "_Remove", "Removes the currently selected files from the queue [DEL]", new KeyGesture(Key.Delete));
            this.ExpandAllBatchesCommand = new RelayCommand(this.ExpandAllBatches, this.CanExpandAllBatches, "E_xpand All", "Expands all batches in the queue");
            this.CollapseAllBatchesCommand = new RelayCommand(this.CollapseAllBatches, this.CanCollapseAllBatches, "Co_llapse All", "Collapses all batches in the queue");
            this.PictureQueue = new PictureQueueViewModel(this.IsDragDropSupportedOnQueue, this.GetQueueCommands());
            this.PictureListViewModels = new BulkObservableCollection<PictureViewModel>();
            this.SelectedPictures = new Picture[0];
            Messenger.Register<AccountActionMessage>(OnAccountActionMessage);
            Messenger.Register<PictureQueueSelectionChangedMessage>(OnPictureQueueSelectionChanged);
        }

        #endregion

        #region Message Handlers

        private void OnAccountActionMessage(AccountActionMessage message)
        {
            if (message.Action == ListAction.CurrentChanged)
            {
                // Determine batches.
                Logger.Log("PictureListViewModel - Current account changed", TraceEventType.Verbose);
                this.Account = message.Account;
                if (this.Account != null)
                {
                    this.BatchList = GetBatchesForAccount(this.Account);
                }
                else
                {
                    this.BatchList = new ObservableCollection<Batch>();
                }
                this.PictureQueue.Account = this.Account;
                Logger.Log("PictureListViewModel - BatchList created", TraceEventType.Verbose);

                // Dispose previous view models.
                var groupedPicturesView = new CollectionViewSource();
                if (this.PictureListViewModels != null)
                {
                    foreach (var pictureListViewModel in this.PictureListViewModels)
                    {
                        pictureListViewModel.Dispose();
                    }
                    Logger.Log("PictureListViewModel - Disposed previous PictureListViewModels", TraceEventType.Verbose);
                }

                // Dispose the previous synchronizer if there was one (to avoid leaking event handlers).
                if (this.synchronizer != null)
                {
                    this.synchronizer.Dispose();
                    this.synchronizer = null;
                }

                // Synchronize the picture view models with the source batches.
                this.synchronizer = new PictureQueueViewModelSynchronizer(this);
                Logger.Log("PictureListViewModel - Set up synchronizer", TraceEventType.Verbose);

                // Determine pictures.
                Logger.Log("PictureListViewModel - Set current PictureListViewModels", TraceEventType.Verbose);
                groupedPicturesView.Source = this.PictureListViewModels;
                Logger.Log("PictureListViewModel - Set Source for GroupedPicturesView", TraceEventType.Verbose);
                if (this.batchGroupDescription != null)
                {
                    this.batchGroupDescription.Dispose();
                    this.batchGroupDescription = null;
                    Logger.Log("PictureListViewModel - Disposed previous BatchIdGroupDescription", TraceEventType.Verbose);
                }
                if (this.Account != null)
                {
                    this.batchGroupDescription = new BatchGroupDescription(this.Account, this.BatchList);
                    groupedPicturesView.GroupDescriptions.Add(this.batchGroupDescription);
                    Logger.Log("PictureListViewModel - Set GroupDescription for GroupedPicturesView", TraceEventType.Verbose);
                }
                this.PictureQueue.Pictures = groupedPicturesView.View;
                Logger.Log("PictureListViewModel - Set final GroupedPicturesView to queue", TraceEventType.Verbose);

                SetPictureDetails();
            }
        }

        private void SetPictureDetails()
        {
            var selectedBatches = this.SelectedPictures.Select(p => p.GetBatch(this.BatchList)).Distinct();
            var batch = (selectedBatches.Count() == 1 ? selectedBatches.First() : null);
            this.PictureDetails = new PictureDetailsViewModel(this.Account, batch, this.SelectedPictures, GetUISettingsForAccount(this.Account), Visibility.Visible, Visibility.Visible, this.ArePictureDetailsReadOnly);
        }

        private void OnPictureQueueSelectionChanged(PictureQueueSelectionChangedMessage message)
        {
            if (message.PictureQueue == this.PictureQueue)
            {
                this.SelectedPictures = message.SelectedPictures;
                SetPictureDetails();
            }
        }

        #endregion

        #region Commands

        private bool CanRemove(object parameter)
        {
            return (this.SelectedPictures.Count > 0);
        }

        private void Remove(object parameter)
        {
            var picturesToRemove = this.SelectedPictures;
            var removingPicturesTask = new ApplicationTask("Removing files", picturesToRemove.Count);
            var step = 0;
            Messenger.Send<TaskStatusMessage>(new TaskStatusMessage(removingPicturesTask));
            try
            {
                var i = 0;

                foreach (var picture in picturesToRemove)
                {
                    removingPicturesTask.SetProgress(step, string.Format(CultureInfo.CurrentCulture, "Removing \"{0}\" ({1} of {2})", picture.Title, ++i, removingPicturesTask.TotalSteps));
                    step++;
                    var batch = picture.GetBatch(this.BatchList);
                    PluginManager.OnPictureEvent(new PictureEventArgs(this.PictureRemovingEventType, App.Info, this.Account, batch, picture, App.IsVideoFile(picture.FileName)));
                }
                foreach (var batch in this.BatchList.ToList())
                {
                    batch.Pictures.RemoveRange(picturesToRemove.Where(ptr=>batch.Pictures.Contains(ptr)));
                    Tasks.RemoveBatchIfEmpty(this.Account, this.BatchList, batch);
                }
                Messenger.Send<PictureQueueChangedMessage>(new PictureQueueChangedMessage(PictureQueueChangedReason.PicturesRemoved));
            }
            finally
            {
                foreach (var picture in picturesToRemove)
                {
                    var batch = picture.GetBatch(this.BatchList);
                    PluginManager.OnPictureEvent(new PictureEventArgs(this.PictureRemovedEventType, App.Info, this.Account, batch, picture, App.IsVideoFile(picture.FileName)));
                }
                removingPicturesTask.SetComplete(string.Format(CultureInfo.CurrentCulture, "{0} removed", removingPicturesTask.TotalSteps.Value.ToCountString("file")));
            }
        }

        private bool CanExpandAllBatches(object parameter)
        {
            return this.BatchList.Any(b => !b.IsExpanded);
        }

        private void ExpandAllBatches(object parameter)
        {
            SetBatchesExpanded(true);
        }

        private bool CanCollapseAllBatches(object parameter)
        {
            return this.BatchList.Any(b => b.IsExpanded);
        }

        private void CollapseAllBatches(object parameter)
        {
            SetBatchesExpanded(false);
        }

        private void SetBatchesExpanded(bool expanded)
        {
            foreach (var batch in this.BatchList)
            {
                batch.IsExpanded = expanded;
            }
        }

        #endregion

        #region Abstract Members

        protected abstract ObservableCollection<Batch> GetBatchesForAccount(Account account);
        protected abstract PictureDetailsUISettings GetUISettingsForAccount(Account account);
        protected abstract bool IsDragDropSupportedOnQueue { get; }
        protected abstract IEnumerable<ICommand> GetQueueCommands();
        protected abstract bool ArePictureDetailsReadOnly { get; }
        protected abstract PictureEventType PictureRemovingEventType { get; }
        protected abstract PictureEventType PictureRemovedEventType { get; }

        #endregion
    }
}