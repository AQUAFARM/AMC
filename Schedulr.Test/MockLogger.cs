using System;
using System.Diagnostics;

namespace Schedulr.Test
{
    public class MockLogger : ILogger
    {
        public void Log(string message, TraceEventType eventType)
        {
            Log(message, null, eventType);
        }

        public void Log(string message, Exception exception)
        {
            Log(message, null, TraceEventType.Error);
        }

        public void Log(string message, Exception exception, TraceEventType eventType)
        {
            if (eventType < TraceEventType.Information)
            {
                throw new Exception("There should be no errors or warnings.");
            }
        }
    }
}