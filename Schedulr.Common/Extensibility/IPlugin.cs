using System;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Represents a plugin.
    /// </summary>
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// Initializes the plugin with its host.
        /// </summary>
        /// <param name="host">The host for the plugin.</param>
        void Initialize(IPluginHost host);
    }
}