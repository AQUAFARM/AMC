using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using JelleDruyts.Windows;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Messages;

namespace Schedulr.Infrastructure
{
    /// <summary>
    /// A base class for view models.
    /// </summary>
    public abstract class ViewModel : ObservableObject, IViewModel
    {
        #region Fields

        /// <summary>
        /// The dispatcher to use when executing actions on the UI thread.
        /// </summary>
        private Dispatcher dispatcher;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        protected ViewModel()
        {
            Messenger.Register<ApplicationLoadedMessage>(m => this.InitializeInternal());
            Messenger.Register<ApplicationClosedMessage>(m => this.Dispose());
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <remarks>
        /// At the time this method is called, the application is fully available and running.
        /// </remarks>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Synchronously executes an action on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        protected void ExecuteUIAction(Action action)
        {
            if (this.dispatcher == null)
            {
                Logger.Log("The UI action cannot be executed because the dispatcher was not initialized.", TraceEventType.Warning);
            }
            else
            {
                // Do not go through the dispatcher unless it's really necessary.
                if (this.dispatcher.Thread != Thread.CurrentThread)
                {
                    this.dispatcher.Invoke(new ThreadStart(() => { action(); }));
                }
                else
                {
                    action();
                }
            }
        }

        /// <summary>
        /// Asynchronously executes an action on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        protected void ExecuteUIActionAsync(Action action)
        {
            if (this.dispatcher == null)
            {
                Logger.Log("The UI action cannot be executed asynchronously because the dispatcher was not initialized.", TraceEventType.Warning);
            }
            else
            {
                // Do not go through the dispatcher unless it's really necessary.
                if (this.dispatcher.Thread != Thread.CurrentThread)
                {
                    this.dispatcher.BeginInvoke(new ThreadStart(() => { action(); }));
                }
                else
                {
                    action();
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Initializes the view model.
        /// </summary>
        private void InitializeInternal()
        {
            this.dispatcher = App.Instance.Dispatcher;
            Initialize();
        }

        #endregion
    }
}