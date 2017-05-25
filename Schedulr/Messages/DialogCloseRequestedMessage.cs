
namespace Schedulr.Messages
{
    public class DialogCloseRequestedMessage
    {
        public Dialog Dialog { get; private set;}

        public DialogCloseRequestedMessage(Dialog dialog)
        {
            this.Dialog = dialog;
        }
    }
}