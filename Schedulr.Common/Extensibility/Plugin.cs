using System;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Provides a convenient base class for plugins, so that they only need to override the methods they are interested in.
    /// </summary>
    public abstract class Plugin : IPlugin
    {
        #region Properties

        /// <summary>
        /// Gets the host for the plugin.
        /// </summary>
        protected IPluginHost Host { get; private set; }

        #endregion

        #region IPlugin Implementation

        /// <summary>
        /// Initializes the plugin with its host.
        /// </summary>
        /// <param name="host">The host for the plugin.</param>
        void IPlugin.Initialize(IPluginHost host)
        {
            this.Host = host;
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="Plugin"/> is reclaimed by garbage collection.
        /// </summary>
        ~Plugin()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}