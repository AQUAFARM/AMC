using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using JelleDruyts.Windows;
using Schedulr.Messages;
using Schedulr.Models;
using Schedulr.ViewModels;

namespace Schedulr.Infrastructure
{
    public class BatchGroupDescription : GroupDescription, IDisposable
    {
        private Account account;
        private IList<Batch> batches;
        private IDictionary<Batch, BatchHeader> batchHeaders = new Dictionary<Batch, BatchHeader>();

        public BatchGroupDescription(Account account, IList<Batch> batches)
        {
            this.account = account;
            this.batches = batches;
            Messenger.Register<UploadScheduleChangedMessage>(OnUploadScheduleChanged);
            Messenger.Register<PictureQueueChangedMessage>(OnPictureQueueChanged);
            RefreshBatchHeaders();
        }

        public void Dispose()
        {
            Messenger.Unregister<UploadScheduleChangedMessage>(OnUploadScheduleChanged);
            Messenger.Unregister<PictureQueueChangedMessage>(OnPictureQueueChanged);
        }

        private void OnPictureQueueChanged(PictureQueueChangedMessage message)
        {
            RefreshGroupNames();
        }

        private void OnUploadScheduleChanged(UploadScheduleChangedMessage message)
        {
            RefreshGroupNames();
        }

        private void RefreshGroupNames()
        {
            RefreshBatchHeaders();
            this.OnPropertyChanged(new PropertyChangedEventArgs("GroupNames"));
        }

        private void RefreshBatchHeaders()
        {
            this.batchHeaders.Clear();
            if (this.batches != null && this.batches.Count > 0)
            {
                IList<DateTime> nextRunTimes = null;
                var batchId = 0;
                foreach (var batch in this.batches)
                {
                    // If no proper batch name could be constructed, just return a generic batch number.
                    var longName = "Batch " + (batchId + 1);
                    var shortName = longName;

                    if (batch.Pictures.All(p => p.DateUploaded.HasValue))
                    {
                        // These pictures have already been uploaded, name the batch according to the uploaded date.
                        // Assume that they have all been uploaded at the same time.
                        var uploaded = batch.Pictures.First().DateUploaded.Value;
                        longName = "Uploaded on " + uploaded.ToString("f");
                        shortName = uploaded.ToShortDateString();
                    }
                    else
                    {
                        if (nextRunTimes == null)
                        {
                            nextRunTimes = ScheduledTaskClient.GetNextRunTimesForAccount(this.account, this.batches.Count);
                        }
                        if (batchId < nextRunTimes.Count)
                        {
                            var runTime = nextRunTimes[batchId];
                            longName += string.Format(CultureInfo.CurrentCulture, " - Scheduled for {0}", runTime.ToString("f"));
                            shortName = runTime.ToShortDateString();
                        }
                    }
                    longName += batch.Pictures.Count.ToCountString("file", " (", ")");
                    shortName += string.Format(CultureInfo.CurrentCulture, " ({0})", batch.Pictures.Count);

                    if (!this.batchHeaders.ContainsKey(batch))
                    {
                        // There is no batch header for this batch yet, create a new one.
                        var batchHeader = new BatchHeader(batch, longName, shortName);
                        this.batchHeaders.Add(batch, batchHeader);
                    }
                    else
                    {
                        // Update an existing batch header with the new names (but keep the expanded state).
                        var batchHeader = this.batchHeaders[batch];
                        batchHeader.LongName = longName;
                        batchHeader.ShortName = shortName;
                    }
                    batchId++;
                }
            }
        }

        public override object GroupNameFromItem(object item, int level, CultureInfo culture)
        {
            var batch = ((PictureViewModel)item).GetBatch();
            if (batch!=null && !this.batchHeaders.ContainsKey(batch))
            {
                RefreshBatchHeaders();
            }
            if (batch==null || !this.batchHeaders.ContainsKey(batch))
            {
                return null;
            }
            else
            {
                return this.batchHeaders[batch];
            }
        }
    }
}