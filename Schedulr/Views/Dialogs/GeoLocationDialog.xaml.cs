using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using JelleDruyts.Windows;
using Schedulr.Models;

namespace Schedulr.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for GeoLocationDialog.xaml
    /// </summary>
    public partial class GeoLocationDialog : Window
    {
        public GeoLocationDialog()
        {
            InitializeComponent();
        }

        public GeoLocation Location
        {
            get
            {
                return this.geoLocationMap.Location;
            }
            set
            {
                this.geoLocationMap.Location = SerializationProvider.Clone<GeoLocation>(value);
            }
        }

        public GeoLocationMapProvider GeoLocationMapProvider
        {
            get
            {
                return this.geoLocationMap.GeoLocationMapProvider;
            }
            set
            {
                this.geoLocationMap.GeoLocationMapProvider = value;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void searchLocationButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void searchLocationTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            this.searchLocationTextBox.Background = null;
            if (e.Key == Key.Enter)
            {
                PerformSearch();
                e.Handled = true;
            }
        }

        private void PerformSearch()
        {
            var success = this.geoLocationMap.SearchLocation(this.searchLocationTextBox.Text);
            this.searchLocationTextBox.Background = (success ? Brushes.LightGreen : Brushes.OrangeRed);
        }
    }
}