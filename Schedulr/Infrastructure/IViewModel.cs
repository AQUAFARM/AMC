using System;

namespace Schedulr.Infrastructure
{
    /// <summary>
    /// Represents a view model.
    /// </summary>
    public interface IViewModel : IDisposable
    {
        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <remarks>
        /// At the time this method is called, the application is fully available and running.
        /// </remarks>
        void Initialize();
    }
}