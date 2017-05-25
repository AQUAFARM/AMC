
namespace Schedulr.Messages
{
    public class PictureQueueChangedMessage
    {
        public PictureQueueChangedReason Reason { get; private set; }

        public PictureQueueChangedMessage(PictureQueueChangedReason reason)
        {
            this.Reason = reason;
        }
    }
}