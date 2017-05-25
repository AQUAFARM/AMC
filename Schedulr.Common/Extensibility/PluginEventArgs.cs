using System;
using System.Collections.Generic;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides data for events handled by plugins.
    /// </summary>
    public abstract class PluginEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the information about the application.
        /// </summary>
        public ApplicationInfo ApplicationInfo { get; private set; }

        /// <summary>
        /// Gets the text template token values that can be used to process text templates.
        /// </summary>
        public IDictionary<string, object> TemplateTokenValues { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginEventArgs"/> class.
        /// </summary>
        /// <param name="applicationInfo">The information about the application.</param>
        internal PluginEventArgs(ApplicationInfo applicationInfo)
        {
            this.ApplicationInfo = applicationInfo;
            this.TemplateTokenValues = new Dictionary<string, object>();
        }
    }
}