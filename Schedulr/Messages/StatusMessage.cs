using System;
using System.Diagnostics;

namespace Schedulr.Messages
{
    public class StatusMessage
    {
        public string Message { get; private set; }
        public string Details { get; private set; }
        public TraceEventType EventType { get; private set; }
        public Exception Exception { get; private set; }

        public StatusMessage(string message)
            : this(message, null, null, TraceEventType.Information)
        {
        }

        public StatusMessage(string message, string details)
            : this(message, details, null, TraceEventType.Information)
        {
        }

        public StatusMessage(string message, TraceEventType eventType)
            : this(message, null, null, eventType)
        {
        }

        public StatusMessage(string message, string details, TraceEventType eventType)
            : this(message, details, null, eventType)
        {
        }

        public StatusMessage(string message, Exception exception)
            : this(message, null, exception, TraceEventType.Error)
        {
        }

        public StatusMessage(string message, string details, Exception exception)
            : this(message, details, exception, TraceEventType.Error)
        {
        }

        public StatusMessage(string message, Exception exception, TraceEventType eventType)
            : this(message, null, exception, TraceEventType.Error)
        {
        }

        public StatusMessage(string message, string details, Exception exception, TraceEventType eventType)
        {
            this.Message = message;
            this.Details = details;
            this.Exception = exception;
            this.EventType = eventType;
        }
    }
}