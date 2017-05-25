using System.Runtime.Serialization;
using System.Windows;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// Contains all User Interface settings for the application.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class UISettings : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the main window state.
        /// </summary>
        [DataMember]
        public WindowState MainWindowState
        {
            get { return this.GetValue(MainWindowStateProperty); }
            set { this.SetValue(MainWindowStateProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="MainWindowState"/> observable property.
        /// </summary>
        public static ObservableProperty<WindowState> MainWindowStateProperty = new ObservableProperty<WindowState, UISettings>(o => o.MainWindowState);

        /// <summary>
        /// Gets or sets the main window height.
        /// </summary>
        [DataMember]
        public double MainWindowHeight
        {
            get { return this.GetValue(MainWindowHeightProperty); }
            set { this.SetValue(MainWindowHeightProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="MainWindowHeight"/> observable property.
        /// </summary>
        public static ObservableProperty<double> MainWindowHeightProperty = new ObservableProperty<double, UISettings>(o => o.MainWindowHeight, 750);

        /// <summary>
        /// Gets or sets the main window width.
        /// </summary>
        [DataMember]
        public double MainWindowWidth
        {
            get { return this.GetValue(MainWindowWidthProperty); }
            set { this.SetValue(MainWindowWidthProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="MainWindowWidth"/> observable property.
        /// </summary>
        public static ObservableProperty<double> MainWindowWidthProperty = new ObservableProperty<double, UISettings>(o => o.MainWindowWidth, 1000);

        #endregion
    }
}