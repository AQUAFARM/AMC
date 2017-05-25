using System;

namespace Schedulr.Messages
{
    public class ApplicationClosingMessage
    {
        private bool cancel;
        public bool Cancel
        {
            get
            {
                return this.cancel;
            }
            set
            {
                if (value == false && this.cancel)
                {
                    throw new ArgumentException("Cannot undo cancellation by setting Cancel back to false when it has already been set to true.", "value");
                }
                this.cancel = value;
            }
        }
    }
}