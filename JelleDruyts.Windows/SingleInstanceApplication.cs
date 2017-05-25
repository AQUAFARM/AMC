using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Windows;

namespace JelleDruyts.Windows
{
    // This class is an adaptation of the single-instance mechanism in Fishbowl (http://fishbowl.codeplex.com/).
    // All credit goes to Joe Castro.

    /// <summary>
    /// Provides a base class for applications that should only run a single instance at a time.
    /// </summary>
    public abstract class SingleInstanceApplication
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value that determines if the single instance is system-wide.
        /// </summary>
        /// <remarks>
        /// By default, a single instance is created per user session (e.g. a Remote Desktop or Terminal Services session,
        /// but a scheduled task also runs as a separate session). By making the single instance system-wide, it is even
        /// enforced across these different sessions.
        /// </remarks>
        protected bool IsSystemWideSingleInstance { get; set; }

        #endregion

        #region Run

        /// <summary>
        /// Runs the single-instance application.
        /// </summary>
        public void Run()
        {
            Run(null, null);
        }

        /// <summary>
        /// Runs the single-instance application with the specified command-line arguments.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public void Run(IList<string> args)
        {
            Run(args, null);
        }

        /// <summary>
        /// Runs the single-instance application with the specified command-line arguments.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        /// <param name="applicationName">The unique application name used to determine the instance.</param>
        public void Run(IList<string> args, string applicationName)
        {
            IpcServerChannel channel = null;
            Mutex singleInstanceMutex = null;
            
            try
            {
                // Subscribe to unhandled exception events.
                AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs appDomainExceptionEventArgs)
                {
                    OnUnhandledException(appDomainExceptionEventArgs.ExceptionObject as Exception);
                };
                
                // Build a repeatable machine-unique name for the channel.
                if (string.IsNullOrEmpty(applicationName))
                {
                    applicationName = Marshal.GetTypeLibGuidForAssembly(Assembly.GetEntryAssembly()).ToString();
                }
                var instanceId = applicationName + Environment.UserName;

                if (this.IsSystemWideSingleInstance)
                {
                    // For a mutex to be visible across Terminal Services sessions (and so be system-wide for the user),
                    // the name should be prefixed with "Global\".
                    // See http://msdn.microsoft.com/en-us/library/system.threading.mutex.aspx
                    instanceId = @"Global\" + instanceId;
                }

                // Try to take ownership of the single-instance mutex.
                bool isFirstInstance;
                singleInstanceMutex = new Mutex(true, instanceId, out isFirstInstance);

                // Depending on the outcome, startup the first instance or notify the already running instance.
                IList<string> commandLineArgs = args ?? new string[0];
                var channelName = instanceId + ":SingleInstanceIPCChannel";
                var remoteServiceName = "SingleInstanceApplicationService";
                if (isFirstInstance)
                {
                    channel = CreateRemoteService(channelName, remoteServiceName);
                    OnStartupFirstInstance(commandLineArgs);
                }
                else
                {
                    NotifyFirstInstance(channelName, remoteServiceName, commandLineArgs);
                }
            }
            catch (Exception exc)
            {
                OnUnhandledException(exc);
            }
            finally
            {
                // Clean up the mutex.
                if (singleInstanceMutex != null)
                {
                    singleInstanceMutex.Close();
                    singleInstanceMutex = null;
                }

                // Clean up the remoting channel.
                if (channel != null)
                {
                    ChannelServices.UnregisterChannel(channel);
                    channel = null;
                }
            }
        }

        #endregion

        #region Private Helper Classes

        private class IpcRemoteService : MarshalByRefObject
        {
            private SingleInstanceApplication application;

            public IpcRemoteService(SingleInstanceApplication application)
            {
                this.application = application;
            }

            public int GetProcessId()
            {
                return Process.GetCurrentProcess().Id;
            }

            /// <summary>Activate the first instance of the application.</summary>
            /// <param name="args">Command line arguemnts to proxy.</param>
            public void InvokeFirstInstance(IList<string> args)
            {
                if (Application.Current != null && !Application.Current.Dispatcher.HasShutdownStarted)
                {
                    Application.Current.Dispatcher.BeginInvoke((Action<object>)((arg) => application.OnStartupNextInstance((IList<string>)arg)), args);
                }
            }

            /// <summary>Overrides the default lease lifetime of 5 minutes so that it never expires.</summary>
            public override object InitializeLifetimeService()
            {
                return null;
            }
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll", EntryPoint = "AllowSetForegroundWindow", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllowSetForegroundWindow(int dwProcessId);
        }

        #endregion

        #region Helper Methods

        private IpcServerChannel CreateRemoteService(string channelName, string serviceName)
        {
            var channel = new IpcServerChannel(
                new Dictionary<string, string>
                {
                    { "name", channelName },
                    { "portName", channelName },
                    { "exclusiveAddressUse", "false" },
                },
                new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full }
            );

            ChannelServices.RegisterChannel(channel, true);
            RemotingServices.Marshal(new IpcRemoteService(this), serviceName);
            return channel;
        }

        private void NotifyFirstInstance(string channelName, string serviceName, IList<string> args)
        {
            var secondInstanceChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(secondInstanceChannel, true);

            var remotingServiceUrl = "ipc://" + channelName + "/" + serviceName;

            // Obtain a reference to the remoting service exposed by the first instance of the application
            var firstInstanceRemoteServiceReference = (IpcRemoteService)RemotingServices.Connect(typeof(IpcRemoteService), remotingServiceUrl);

            // Pass along the current arguments to the first instance if it's up and accepting requests.
            if (firstInstanceRemoteServiceReference != null)
            {
                // Allow the first instance to give itself user focus.
                // This could be done with ASFW_ANY if the IPC call is expensive.
                int procId = firstInstanceRemoteServiceReference.GetProcessId();
                NativeMethods.AllowSetForegroundWindow(procId);

                firstInstanceRemoteServiceReference.InvokeFirstInstance(args);
            }
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Called when the first instance of the application is started.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        protected virtual void OnStartupFirstInstance(IList<string> args)
        {
        }

        /// <summary>
        /// Called when a subsequent instance of the application is started.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        protected virtual void OnStartupNextInstance(IList<string> args)
        {
        }

        /// <summary>
        /// Called when an unhandled exception has occurred.
        /// </summary>
        /// <param name="exception">The unhandled exception that has occurred.</param>
        protected virtual void OnUnhandledException(Exception exception)
        {
        }

        #endregion
    }
}