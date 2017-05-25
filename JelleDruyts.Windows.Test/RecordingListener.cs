using System.Collections.Generic;

namespace JelleDruyts.Windows.Test
{
    public class RecordingListener<T>
    {
        public List<T> ReceivedMessages { get; private set; }

        public RecordingListener()
        {
            this.ReceivedMessages = new List<T>();
        }

        public void Receive(T message)
        {
            this.ReceivedMessages.Add(message);
        }
    }
}