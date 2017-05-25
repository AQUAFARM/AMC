using System;
using System.Diagnostics;
using System.Globalization;
using JelleDruyts.Windows;
using Schedulr.Infrastructure;
using Schedulr.Messages;

namespace Schedulr.Extensibility
{
    public class PluginLogger : ILogger
    {
        private PluginInstance plugin;
        private string logPrefix;
        private ILogger logger;

        public PluginLogger(PluginInstance plugin)
        {
            this.plugin = plugin;
            this.logPrefix = string.Format(CultureInfo.CurrentCulture, "[Plugin \"{0}:{1}\"] ", this.plugin.Type.Id, this.plugin.Id);
            this.logger = Logger.SchedulrPluginsLogger;
        }

        public void Log(string message, TraceEventType eventType)
        {
            this.logger.Log(this.logPrefix + message, eventType);
            if (eventType <= TraceEventType.Warning)
            {
                Messenger.Send<StatusMessage>(new PluginStatusMessage(this.plugin, message, null, eventType));
            }
        }

        public void Log(string message, Exception exception)
        {
            this.logger.Log(this.logPrefix + message, exception);
            Messenger.Send<StatusMessage>(new PluginStatusMessage(this.plugin, exception.Message, exception, TraceEventType.Error));
        }

        public void Log(string message, Exception exception, TraceEventType eventType)
        {
            this.logger.Log(this.logPrefix + message, exception, eventType);
            Messenger.Send<StatusMessage>(new PluginStatusMessage(this.plugin, exception.Message, exception, eventType));
        }
    }
}