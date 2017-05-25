using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JelleDruyts.Windows.Test
{
    [TestClass]
    public class MessengerTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterThrowsOnNullCallback()
        {
            Messenger.Register<string>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnregisterThrowsOnNullCallback()
        {
            Messenger.Unregister<string>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendThrowsOnNullMessage()
        {
            Messenger.Send<string>(null);
        }

        [TestMethod]
        public void MessengerSendsMessageToListener()
        {
            var receiver = new RecordingListener<string>();
            var message = "Test";
            Messenger.Register<string>(receiver.Receive);
            Messenger.Send<string>(message);
            Assert.AreEqual<int>(1, receiver.ReceivedMessages.Count);
            Assert.AreEqual<string>(message, receiver.ReceivedMessages[0]);
        }

        [TestMethod]
        public void MessengerSendsMessageToListenerAndStopsWhenUnregistered()
        {
            var receiver = new RecordingListener<string>();
            var message = "Test";

            // Register & send.
            Messenger.Register<string>(receiver.Receive);
            Messenger.Send<string>(message);
            Assert.AreEqual<int>(1, receiver.ReceivedMessages.Count);
            Assert.AreEqual<string>(message, receiver.ReceivedMessages[0]);

            // Unregister & send.
            Messenger.Unregister<string>(receiver.Receive);
            Messenger.Send<string>(message);
            Assert.AreEqual<int>(1, receiver.ReceivedMessages.Count);
        }

        [TestMethod]
        public void MessengerSendsMessageToListeners()
        {
            var receiver1 = new RecordingListener<string>();
            var receiver2 = new RecordingListener<string>();
            var message = "Test";

            // Register & send.
            Messenger.Register<string>(receiver1.Receive);
            Messenger.Register<string>(receiver2.Receive);
            Messenger.Send<string>(message);
            Assert.AreEqual<int>(1, receiver1.ReceivedMessages.Count);
            Assert.AreEqual<string>(message, receiver1.ReceivedMessages[0]);
            Assert.AreEqual<int>(1, receiver2.ReceivedMessages.Count);
            Assert.AreEqual<string>(message, receiver2.ReceivedMessages[0]);
        }

        [TestMethod]
        public void MessengerSendsMessageToListenersAndStopsWhenUnregistered()
        {
            var receiver1 = new RecordingListener<string>();
            var receiver2 = new RecordingListener<string>();
            var message = "Test";

            // Register & send.
            Messenger.Register<string>(receiver1.Receive);
            Messenger.Register<string>(receiver2.Receive);
            Messenger.Send<string>(message);
            Assert.AreEqual<int>(1, receiver1.ReceivedMessages.Count);
            Assert.AreEqual<string>(message, receiver1.ReceivedMessages[0]);
            Assert.AreEqual<int>(1, receiver2.ReceivedMessages.Count);
            Assert.AreEqual<string>(message, receiver2.ReceivedMessages[0]);

            // Unregister first & send.
            Messenger.Unregister<string>(receiver1.Receive);
            Messenger.Send<string>(message);
            Assert.AreEqual<int>(1, receiver1.ReceivedMessages.Count);
            Assert.AreEqual<int>(2, receiver2.ReceivedMessages.Count);
            Assert.AreEqual<string>(message, receiver2.ReceivedMessages[1]);

            // Unregister second & send.
            Messenger.Unregister<string>(receiver2.Receive);
            Messenger.Send<string>(message);
            Assert.AreEqual<int>(1, receiver1.ReceivedMessages.Count);
            Assert.AreEqual<int>(2, receiver2.ReceivedMessages.Count);
            Assert.AreEqual<string>(message, receiver2.ReceivedMessages[1]);
        }

        [TestMethod]
        public void MessengerSendsDerivedMessageToListeners()
        {
            var receiver = new RecordingListener<EventArgs>();
            var message = new UnhandledExceptionEventArgs(new Exception(), false);
            Messenger.Register<EventArgs>(receiver.Receive);
            Messenger.Send(message);

            Assert.AreEqual<int>(1, receiver.ReceivedMessages.Count);
            Assert.AreSame(message, receiver.ReceivedMessages[0]);
        }
    }
}