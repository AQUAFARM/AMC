using System;
using System.Diagnostics;
using System.Globalization;
using Schedulr.Extensibility;

namespace Schedulr.Messages
{
    public class PluginStatusMessage : StatusMessage
    {
        public PluginInstance Plugin { get; private set; }

        public PluginStatusMessage(PluginInstance plugin, string message, Exception exception, TraceEventType eventType)
            : base(string.Format(CultureInfo.CurrentCulture, "[{0}] {1}", plugin.Type.Name, message), null, exception, eventType)
        {
            this.Plugin = plugin;
        }
    }
}