using System;
using System.Runtime.Serialization;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Represents an exception that was caused by a plugin.
    /// </summary>
    [Serializable]
    public class PluginException : Exception
    {
        /// <summary>
        /// Gets the plugin instance that caused the exception.
        /// </summary>
        public PluginInstance Plugin { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginException"/> class.
        /// </summary>
        /// <param name="plugin">The plugin that caused the exception.</param>
        public PluginException(PluginInstance plugin)
            : this(plugin, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginException"/> class.
        /// </summary>
        /// <param name="plugin">The plugin that caused the exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public PluginException(PluginInstance plugin, string message)
            : this(plugin, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginException"/> class.
        /// </summary>
        /// <param name="plugin">The plugin that caused the exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
        public PluginException(PluginInstance plugin, string message, Exception inner)
            : base(message, inner)
        {
            this.Plugin = plugin;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected PluginException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}