using System;
using System.Collections.Generic;
using Schedulr.Infrastructure;

namespace Schedulr.ViewModels
{
    /// <summary>
    /// A locator for view models.
    /// </summary>
    public class ViewModelLocator
    {
        #region Static Fields

        private static readonly Dictionary<Type, IViewModel> viewModels;

        #endregion

        #region Static Constructor

        /// <summary>
        /// Initializes the <see cref="ViewModelLocator"/> class.
        /// </summary>
        static ViewModelLocator()
        {
            // Add all known view models.
            viewModels = new Dictionary<Type, IViewModel>();
            viewModels.Add(typeof(LogoViewModel), new LogoViewModel());
            viewModels.Add(typeof(StatusPanelViewModel), new StatusPanelViewModel());
            viewModels.Add(typeof(MainWindowViewModel), new MainWindowViewModel());
            viewModels.Add(typeof(QueuedPicturesViewModel), new QueuedPicturesViewModel());
            viewModels.Add(typeof(UploadedPicturesViewModel), new UploadedPicturesViewModel());
            viewModels.Add(typeof(ConfigurationEditorViewModel), new ConfigurationEditorViewModel());
            viewModels.Add(typeof(OptionsViewModel), new OptionsViewModel());
            viewModels.Add(typeof(UploadConfirmationViewModel), new UploadConfirmationViewModel());
            viewModels.Add(typeof(ScheduleEditorViewModel), new ScheduleEditorViewModel());

            // Initialize the singleton.
            Instance = new ViewModelLocator();
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets the singleton <see cref="ViewModelLocator"/> instance.
        /// </summary>
        public static ViewModelLocator Instance { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLocator"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is private so an instance is only available through the <see cref="Instance"/> singleton.
        /// </remarks>
        private ViewModelLocator()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the logo view model.
        /// </summary>
        public LogoViewModel LogoViewModel
        {
            get { return GetViewModel<LogoViewModel>(); }
        }

        /// <summary>
        /// Gets the status panel view model.
        /// </summary>
        public StatusPanelViewModel StatusPanelViewModel
        {
            get { return GetViewModel<StatusPanelViewModel>(); }
        }

        /// <summary>
        /// Gets the main window view model.
        /// </summary>
        public MainWindowViewModel MainWindowViewModel
        {
            get { return GetViewModel<MainWindowViewModel>(); }
        }

        /// <summary>
        /// Gets the queued pictures view model.
        /// </summary>
        public QueuedPicturesViewModel QueuedPicturesViewModel
        {
            get { return GetViewModel<QueuedPicturesViewModel>(); }
        }

        /// <summary>
        /// Gets the uploaded pictures view model.
        /// </summary>
        public UploadedPicturesViewModel UploadedPicturesViewModel
        {
            get { return GetViewModel<UploadedPicturesViewModel>(); }
        }

        /// <summary>
        /// Gets the accounts editor view model.
        /// </summary>
        public ConfigurationEditorViewModel ConfigurationEditorViewModel
        {
            get { return GetViewModel<ConfigurationEditorViewModel>(); }
        }

        /// <summary>
        /// Gets the options view model.
        /// </summary>
        public OptionsViewModel OptionsViewModel
        {
            get { return GetViewModel<OptionsViewModel>(); }
        }

        /// <summary>
        /// Gets the upload confirmation view model.
        /// </summary>
        public UploadConfirmationViewModel UploadConfirmationViewModel
        {
            get { return GetViewModel<UploadConfirmationViewModel>(); }
        }

        /// <summary>
        /// Gets the schedule editor view model.
        /// </summary>
        public ScheduleEditorViewModel ScheduleEditorViewModel
        {
            get { return GetViewModel<ScheduleEditorViewModel>(); }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the view model instance for the specified type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The view model instance of the specified type.</returns>
        private static TViewModel GetViewModel<TViewModel>() where TViewModel : IViewModel
        {
            if (!viewModels.ContainsKey(typeof(TViewModel)))
            {
                throw new ArgumentException("The specified view model is not known by the locator: " + typeof(TViewModel).Name);
            }
            return (TViewModel)viewModels[typeof(TViewModel)];
        }

        #endregion
    }
}