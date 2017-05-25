using System.Collections.Generic;
using Schedulr.Models;
using Schedulr.ViewModels;

namespace Schedulr.Messages
{
    internal class PictureQueueSelectionChangedMessage
    {
        public IList<Picture> SelectedPictures { get; private set; }
        public PictureQueueViewModel PictureQueue { get; private set; }

        public PictureQueueSelectionChangedMessage(IList<Picture> selectedPictures, PictureQueueViewModel pictureQueue)
        {
            this.SelectedPictures = selectedPictures;
            this.PictureQueue = pictureQueue;
        }
    }
}