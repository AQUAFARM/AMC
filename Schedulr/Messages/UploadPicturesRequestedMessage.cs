using System.Collections.Generic;
using Schedulr.Models;

namespace Schedulr.Messages
{
    public class UploadPicturesRequestedMessage
    {
        public IList<Picture> Pictures { get; private set; }
        public Account Account { get; private set; }
        public UploadPicturesRequestReason Reason { get; private set; }
        public Batch Batch { get; private set; }
        public bool UploadConfirmed { get; set; }

        public UploadPicturesRequestedMessage(IList<Picture> pictures, Account account, UploadPicturesRequestReason reason)
            : this(pictures, null, account, reason)
        {
        }

        public UploadPicturesRequestedMessage(Batch batch, Account account, UploadPicturesRequestReason reason)
            : this(batch == null ? null : batch.Pictures, batch, account, reason)
        {
        }

        private UploadPicturesRequestedMessage(IList<Picture> pictures, Batch batch, Account account, UploadPicturesRequestReason reason)
        {
            this.Pictures = pictures;
            this.Account = account;
            this.Reason = reason;
            this.Batch = batch;
        }

        public bool IsEquivalent(UploadPicturesRequestedMessage message)
        {
            var isEquivalent = false;
            if (this.Account == message.Account && this.Batch == message.Batch)
            {
                if (this.Pictures != null && message.Pictures != null && this.Pictures.Count == message.Pictures.Count)
                {
                    var samePictures = true;
                    for (var i = 0; i < this.Pictures.Count; i++)
                    {
                        if (this.Pictures[i] != message.Pictures[i])
                        {
                            samePictures = false;
                            break;
                        }
                    }
                    if (samePictures)
                    {
                        isEquivalent = true;
                    }
                }
            }
            return isEquivalent;
        }
    }
}