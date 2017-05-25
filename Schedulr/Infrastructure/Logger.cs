using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Schedulr.Providers;

namespace Schedulr.Infrastructure
{
    /// <summary>
    /// Provides logging services.
    /// </summary>
    public class Logger : ILogger
    {
        #region Fields

        /// <summary>
        /// The trace source to log to.
        /// </summary>
        private TraceSource tracer;

        /// <summary>
        /// The synchronisation object to lock on.
        /// </summary>
        private object lockObject;

        #endregion

        #region Static Helpers

        /// <summary>
        /// Initializes the <see cref="Logger"/> class.
        /// </summary>
        static Logger()
        {
            var sharedListener = new TextWriterTraceListener(PathProvider.LogFilePath);
            Logger.SchedulrLogger = new Logger("Schedulr", sharedListener);
            Logger.SchedulrPluginsLogger = new Logger("SchedulrPlugins", sharedListener);
        }

        /// <summary>
        /// Gets the main logger instance.
        /// </summary>
        public static ILogger SchedulrLogger { get; private set; }

        /// <summary>
        /// Gets the plugin logger instance.
        /// </summary>
        public static ILogger SchedulrPluginsLogger { get; private set; }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception that occurred.</param>
        public static void Log(string message, Exception exception)
        {
            Logger.SchedulrLogger.Log(message, exception);
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="eventType">The type of event that caused the trace.</param>
        public static void Log(string message, Exception exception, TraceEventType eventType)
        {
            Logger.SchedulrLogger.Log(message, exception, eventType);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to trace.</param>
        /// <param name="eventType">The type of event that caused the trace.</param>
        public static void Log(string message, TraceEventType eventType)
        {
            Logger.SchedulrLogger.Log(message, eventType);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="sourceName">The name of the trace source.</param>
        /// <param name="listener">The listener to add.</param>
        /// <remarks>This constructor is private so no instances can be created from the outside.</remarks>
        private Logger(string sourceName, TraceListener listener)
        {
            this.lockObject = new object();
            this.tracer = new TraceSource(sourceName);
            this.tracer.Listeners.Add(listener);
        }

        #endregion

        #region ILogger Members

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception that occurred.</param>
        void ILogger.Log(string message, Exception exception)
        {
            ((ILogger)this).Log(message, exception, TraceEventType.Error);
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="message">The message to trace.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="eventType">The type of event that caused the trace.</param>
        void ILogger.Log(string message, Exception exception, TraceEventType eventType)
        {
            var errorMessage = message;
            if (exception != null)
            {
                errorMessage += string.Format(CultureInfo.CurrentCulture, ": {0}", exception.ToString());
            }
            ((ILogger)this).Log(errorMessage, eventType);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to trace.</param>
        /// <param name="eventType">The type of event that caused the trace.</param>
        void ILogger.Log(string message, TraceEventType eventType)
        {
            lock (this.lockObject)
            {
                var fullMessage = string.Format(CultureInfo.CurrentCulture, "{0}[T{1:00}] [{2}] {3}", new string(' ', 11 - eventType.ToString().Length), Thread.CurrentThread.ManagedThreadId, DateTime.Now, message);
                this.tracer.TraceEvent(eventType, 0, fullMessage);
                this.tracer.Flush();
            }
        }

        #endregion
    }
}