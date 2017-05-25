using System.Globalization;
using System.Windows;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Infrastructure;

namespace Schedulr.ViewModels
{
    public class LogoViewModel : ViewModel
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        public string ApplicationVersion
        {
            get { return this.GetValue(ApplicationVersionProperty); }
            set { this.SetValue(ApplicationVersionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ApplicationVersion"/> observable property.
        /// </summary>
        public static ObservableProperty<string> ApplicationVersionProperty = new ObservableProperty<string, LogoViewModel>(o => o.ApplicationVersion);

        /// <summary>
        /// Gets or sets the visibility of the new version message.
        /// </summary>
        public Visibility NewVersionVisibility
        {
            get { return this.GetValue(NewVersionVisibilityProperty); }
            set { this.SetValue(NewVersionVisibilityProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="NewVersionVisibility"/> observable property.
        /// </summary>
        public static ObservableProperty<Visibility> NewVersionVisibilityProperty = new ObservableProperty<Visibility, LogoViewModel>(o => o.NewVersionVisibility, Visibility.Hidden);

        /// <summary>
        /// Gets or sets the message to display if there is a new version.
        /// </summary>
        public string NewVersionMessage
        {
            get { return this.GetValue(NewVersionMessageProperty); }
            set { this.SetValue(NewVersionMessageProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="NewVersionMessage"/> observable property.
        /// </summary>
        public static ObservableProperty<string> NewVersionMessageProperty = new ObservableProperty<string, LogoViewModel>(o => o.NewVersionMessage);

        /// <summary>
        /// Gets or sets the URL where a new version can be downloaded.
        /// </summary>
        public string NewVersionUrl
        {
            get { return this.GetValue(NewVersionUrlProperty); }
            set { this.SetValue(NewVersionUrlProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="NewVersionUrl"/> observable property.
        /// </summary>
        public static ObservableProperty<string> NewVersionUrlProperty = new ObservableProperty<string, LogoViewModel>(o => o.NewVersionUrl);

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <remarks>At the time this method is called, the application is fully available and running.</remarks>
        public override void Initialize()
        {
            this.ApplicationVersion = App.DisplayVersion;
            Tasks.CheckForUpdates((version, downloadUrl) =>
            {
                this.NewVersionMessage = string.Format(CultureInfo.CurrentCulture, "A new version is available: v{0}!", version.ToString());
                this.NewVersionUrl = downloadUrl.ToString();
                this.NewVersionVisibility = Visibility.Visible;
            });
        }

        #endregion
    }
}