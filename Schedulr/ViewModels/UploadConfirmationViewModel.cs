using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using JelleDruyts.Windows;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Infrastructure;
using Schedulr.Messages;

namespace Schedulr.ViewModels
{
    public class UploadConfirmationViewModel : ViewModel
    {
        #region Fields

        private Timer timeoutTimer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialog commands that are available.
        /// </summary>
        public IEnumerable<ICommand> DialogCommands { get; private set; }

        /// <summary>
        /// Gets the input bindings for the commands.
        /// </summary>
        public IEnumerable<InputBinding> InputBindings { get; private set; }

        /// <summary>
        /// Gets the total seconds available before the confirmation times out.
        /// </summary>
        public int TotalTimeoutSeconds { get { return 30; } }

        /// <summary>
        /// Gets the collection of queued upload requests.
        /// </summary>
        public ObservableCollection<UploadRequestViewModel> UploadRequests { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets the remaining seconds available before the confirmation times out as a string.
        /// </summary>
        public string RemainingTimeoutMessage
        {
            get { return this.GetValue(TimeoutMessageProperty); }
            private set { this.SetValue(TimeoutMessageProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="RemainingTimeoutMessage"/> observable property.
        /// </summary>
        public static ObservableProperty<string> TimeoutMessageProperty = new ObservableProperty<string, UploadConfirmationViewModel>(o => o.RemainingTimeoutMessage);

        /// <summary>
        /// Gets the remaining seconds available before the confirmation times out.
        /// </summary>
        public int RemainingTimeoutSeconds
        {
            get { return this.GetValue(TimeoutSecondsProperty); }
            private set { this.SetValue(TimeoutSecondsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="RemainingTimeoutSeconds"/> observable property.
        /// </summary>
        public static ObservableProperty<int> TimeoutSecondsProperty = new ObservableProperty<int, UploadConfirmationViewModel>(o => o.RemainingTimeoutSeconds);

        /// <summary>
        /// Gets the elapsed seconds from the start of the timeout period.
        /// </summary>
        public int ElapsedTimeoutSeconds
        {
            get { return this.GetValue(ElapsedTimeoutSecondsProperty); }
            private set { this.SetValue(ElapsedTimeoutSecondsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ElapsedTimeoutSeconds"/> observable property.
        /// </summary>
        public static ObservableProperty<int> ElapsedTimeoutSecondsProperty = new ObservableProperty<int, UploadConfirmationViewModel>(o => o.ElapsedTimeoutSeconds);

        /// <summary>
        /// Gets or sets the informational message to display.
        /// </summary>
        public string InfoMessage
        {
            get { return this.GetValue(InfoMessageProperty); }
            set { this.SetValue(InfoMessageProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="InfoMessage"/> observable property.
        /// </summary>
        public static ObservableProperty<string> InfoMessageProperty = new ObservableProperty<string, UploadConfirmationViewModel>(o => o.InfoMessage);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadConfirmationViewModel"/> class.
        /// </summary>
        public UploadConfirmationViewModel()
        {
            this.timeoutTimer = new Timer(1000); // Update the timer every second.
            this.timeoutTimer.Elapsed += new ElapsedEventHandler(timeoutTimer_Elapsed);
            this.UploadRequests = new ObservableCollection<UploadRequestViewModel>();

            this.DialogCommands = new ICommand[]
            {
                new RelayCommand(Upload, CanUpload, "_Upload Now", "Uploads the requested files now [ENTER]", new KeyGesture(Key.Enter)),
                new RelayCommand(Cancel, CanCancel, "_Cancel", "Cancels all upload requests [ESC]", new KeyGesture(Key.Escape))
            };

            this.InputBindings = this.DialogCommands.OfType<RelayCommand>().SelectMany(r => r.InputGestures.Select(g => new InputBinding(r, g)));
            Messenger.Register<UploadPicturesRequestedMessage>(OnUploadPicturesRequested);
        }

        private void timeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SetTimeout(this.RemainingTimeoutSeconds - 1);
        }

        #endregion

        #region Message Handlers

        private void OnUploadPicturesRequested(UploadPicturesRequestedMessage message)
        {
            if (!message.UploadConfirmed)
            {
                // See if another request already exists for the same account and with the same pictures.
                var alreadyRequested = false;
                foreach (var request in this.UploadRequests)
                {
                    if (message.IsEquivalent(request.Request))
                    {
                        alreadyRequested = true;
                        break;
                    }
                }
                if (!alreadyRequested)
                {
                    this.UploadRequests.Add(new UploadRequestViewModel(message));
                }

                // Set the information message.
                var infoMessage = "The following uploads were requested";
                if (message.Reason == UploadPicturesRequestReason.CommandLineBackground)
                {
                    infoMessage += " in the background (possibly because of a scheduled task)";
                }
                this.InfoMessage = infoMessage + ".";

                // Reset the timer on each request.
                SetTimeout(this.TotalTimeoutSeconds);
            }
        }

        private void SetTimeout(int timeout)
        {
            this.RemainingTimeoutSeconds = timeout;
            this.RemainingTimeoutMessage = timeout.ToCountString("second");
            this.ElapsedTimeoutSeconds = this.TotalTimeoutSeconds - this.RemainingTimeoutSeconds;
            this.timeoutTimer.Enabled = (this.RemainingTimeoutSeconds > 0);
            if (this.RemainingTimeoutSeconds <= 0)
            {
                Upload(null);
            }
        }

        #endregion

        #region Dialog Commands

        private bool CanUpload(object parameter)
        {
            return true;
        }

        private void Upload(object parameter)
        {
            this.timeoutTimer.Enabled = false;
            Messenger.Send<DialogCloseRequestedMessage>(new DialogCloseRequestedMessage(Dialog.UploadConfirmation));
            foreach (var request in this.UploadRequests)
            {
                request.Request.UploadConfirmed = true;
                Messenger.Send<UploadPicturesRequestedMessage>(request.Request);
            }
            this.ExecuteUIActionAsync(() => { this.UploadRequests.Clear(); });
        }

        private bool CanCancel(object parameter)
        {
            return true;
        }

        private void Cancel(object parameter)
        {
            this.timeoutTimer.Enabled = false;
            this.ExecuteUIActionAsync(() => { this.UploadRequests.Clear(); });
            Messenger.Send<DialogCloseRequestedMessage>(new DialogCloseRequestedMessage(Dialog.UploadConfirmation));
        }

        #endregion
    }
}