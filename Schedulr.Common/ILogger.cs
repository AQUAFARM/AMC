using System;
using System.Diagnostics;

namespace Schedulr
{
    /// <summary>
    /// Represents a logger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to trace.</param>
        /// <param name="eventType">The type of event that caused the trace.</param>
        void Log(string message, TraceEventType eventType);

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception that occurred.</param>
        void Log(string message, Exception exception);

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="eventType">The type of event that caused the trace.</param>
        void Log(string message, Exception exception, TraceEventType eventType);
    }
}