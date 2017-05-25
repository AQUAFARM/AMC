using System;
using System.Collections.Generic;
using System.Linq;

namespace JelleDruyts.Windows
{
    /// <summary>
    /// Sends messages of any type to registered receivers.
    /// </summary>
    public static class Messenger
    {
        #region Fields

        private static readonly object registeredListenersLock = new object();
        private static readonly Dictionary<Type, List<object>> registeredListenersForMessageType = new Dictionary<Type, List<object>>();

        #endregion

        #region Methods

        /// <summary>
        /// Registers a receiver callback for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message the receiver needs to be notified of.</typeparam>
        /// <param name="callback">The callback method to invoke when a message of the specified type is sent.</param>
        public static void Register<TMessage>(Action<TMessage> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            lock (registeredListenersLock)
            {
                var messageType = typeof(TMessage);
                if (!registeredListenersForMessageType.ContainsKey(messageType))
                {
                    registeredListenersForMessageType.Add(messageType, new List<object>());
                }
                registeredListenersForMessageType[messageType].Add(callback);
            }
        }

        /// <summary>
        /// Unregisters a receiver callback for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message the receiver was being notified of.</typeparam>
        /// <param name="callback">The callback method to remove.</param>
        public static void Unregister<TMessage>(Action<TMessage> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            lock (registeredListenersLock)
            {
                var messageType = typeof(TMessage);
                if (registeredListenersForMessageType.ContainsKey(messageType))
                {
                    var item = registeredListenersForMessageType[messageType].Where(r => r.Equals(callback)).FirstOrDefault();
                    if (item != null)
                    {
                        registeredListenersForMessageType[messageType].Remove(item);
                    }
                }
            }
        }

        /// <summary>
        /// Sends the specified message to all registered receivers.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message to send.</param>
        public static void Send<TMessage>(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            lock (registeredListenersLock)
            {
                var liveListeners = from listeners in registeredListenersForMessageType
                                    where listeners.Key.IsAssignableFrom(typeof(TMessage))
                                    from listener in listeners.Value
                                    select listener;
                foreach (Action<TMessage> listener in liveListeners.ToList())
                {
                    listener(message);
                }
            }
        }

        #endregion
    }
}