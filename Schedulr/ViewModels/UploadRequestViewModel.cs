using JelleDruyts.Windows;
using Schedulr.Messages;

namespace Schedulr.ViewModels
{
    public class UploadRequestViewModel
    {
        public UploadPicturesRequestedMessage Request { get; private set; }
        public string PictureCount { get; private set; }
        public bool IsBatch { get; private set; }

        public UploadRequestViewModel(UploadPicturesRequestedMessage request)
        {
            this.Request = request;
            this.IsBatch = (request.Batch != null);
            var pictureCount = request.Pictures.Count.ToCountString("file");
            if (this.IsBatch)
            {
                pictureCount = "1 batch of " + pictureCount;
            }
            this.PictureCount = pictureCount;
        }
    }
}