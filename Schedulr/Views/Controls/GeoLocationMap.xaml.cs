using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using Schedulr.Models;

namespace Schedulr.Views.Controls
{
    /// <summary>
    /// Interaction logic for GeoLocationMap.xaml
    /// </summary>
    public partial class GeoLocationMap : UserControl
    {
        #region Fields

        private static IEnumerable<MapTypeViewModel> availableMaps;

        #endregion

        #region Constructors

        static GeoLocationMap()
        {
            availableMaps = new MapTypeViewModel[]
            {
                new MapTypeViewModel(Models.GeoLocationMapProvider.BingRoad, GMapProviders.BingMap, "Bing Map (Road)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.BingSatellite, GMapProviders.BingSatelliteMap, "Bing Map (Satellite)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.BingHybrid, GMapProviders.BingHybridMap, "Bing Map (Hybrid)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.YahooRoad, GMapProviders.YahooMap, "Yahoo Map (Road)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.YahooSatellite, GMapProviders.YahooSatelliteMap, "Yahoo Map (Satellite)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.YahooHybrid, GMapProviders.YahooHybridMap, "Yahoo Map (Hybrid)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.GoogleRoad, GMapProviders.GoogleMap, "Google Map (Road)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.GoogleSatellite, GMapProviders.GoogleSatelliteMap, "Google Map (Satellite)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.GoogleTerrain, GMapProviders.GoogleTerrainMap, "Google Map (Terrain)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.GoogleHybrid, GMapProviders.GoogleHybridMap, "Google Map (Hybrid)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.OviRoad, GMapProviders.OviMap, "Ovi Map (Road)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.OviSatellite, GMapProviders.OviSatelliteMap, "Ovi Map (Satellite)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.OviTerrain, GMapProviders.OviTerrainMap, "Ovi Map (Terrain)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.OviHybrid, GMapProviders.OviHybridMap, "Ovi Map (Hybrid)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.YandexRoad, GMapProviders.YandexMap, "Yandex Map (Road)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.YandexSatellite, GMapProviders.YandexSatelliteMap, "Yandex Map (Satellite)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.YandexHybrid, GMapProviders.YandexHybridMap, "Yandex Map (Hybrid)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.OpenStreetMapRoad, GMapProviders.OpenStreetMap, "OpenStreetMap (Road)"),
                new MapTypeViewModel(Models.GeoLocationMapProvider.CloudMadeRoad, GMapProviders.CloudMadeMap, "CloudMade Map (Road)")
            };
        }

        public GeoLocationMap()
        {
            InitializeComponent();

            var defaultMapType = availableMaps.First();
            this.geoGMapControl.Manager.Mode = AccessMode.ServerOnly;
            this.geoGMapControl.MapProvider = defaultMapType.GMapMapType;
            this.geoGMapControl.DragButton = MouseButton.Left;
            this.geoGMapControl.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            this.geoGMapControl.MinZoom = 1;
            this.geoGMapControl.MaxZoom = GeoLocation.MaxAccuracy;
            this.geoGMapControl.Zoom = 1;
            this.geoGMapControl.OnMapZoomChanged += new MapZoomChanged(geoGMapControl_OnMapZoomChanged);
            this.geoGMapControl.OnPositionChanged += new PositionChanged(geoGMapControl_OnPositionChanged);
            this.geoGMapControl.Manager.UseGeocoderCache = false;

            this.mapTypesComboBox.ItemsSource = availableMaps;
            this.mapTypesComboBox.SelectedItem = defaultMapType;
            this.mapTypesComboBox.SelectionChanged += new SelectionChangedEventHandler(mapTypesComboBox_SelectionChanged);
        }

        #endregion

        #region Event Handlers

        private void geoGMapControl_OnPositionChanged(PointLatLng point)
        {
            this.Location.Latitude = point.Lat;
            this.Location.Longitude = point.Lng;
        }

        private void geoGMapControl_OnMapZoomChanged()
        {
            // Even though the map is configured with min and max zoom values, it can still return values outside that range.
            // Explicitly constrain the map's zoom level to the allowed values for a GeoLocation.
            var zoomLevel = (int)this.geoGMapControl.Zoom;
            zoomLevel = Math.Max(GeoLocation.MinAccuracy, Math.Min(GeoLocation.MaxAccuracy, zoomLevel));
            this.Location.Accuracy = zoomLevel;
        }

        private void mapTypesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.GeoLocationMapProvider = ((MapTypeViewModel)this.mapTypesComboBox.SelectedItem).MapProvider;
        }

        #endregion

        #region IsReadOnly

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(GeoLocationMap), new UIPropertyMetadata(false, OnIsReadOnlyChanged));

        private static void OnIsReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var instance = (GeoLocationMap)o;
            instance.geoGMapControl.IsEnabled = !instance.IsReadOnly;
            instance.zoomStackPanel.Visibility = (instance.IsReadOnly ? Visibility.Collapsed : Visibility.Visible);
            instance.mapTypesComboBox.Visibility = (instance.IsReadOnly ? Visibility.Collapsed : Visibility.Visible);
        }

        #endregion

        #region Location

        public GeoLocation Location
        {
            get { return (GeoLocation)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        public static readonly DependencyProperty LocationProperty = DependencyProperty.Register("Location", typeof(GeoLocation), typeof(GeoLocationMap), new UIPropertyMetadata(new GeoLocation(), OnLocationChanged));

        private static void OnLocationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var instance = (GeoLocationMap)sender;

            var oldLocation = e.OldValue as GeoLocation;
            if (oldLocation != null)
            {
                oldLocation.PropertyChanged -= instance.Location_PropertyChanged;
            }

            if (instance.Location == null)
            {
                instance.Location = new GeoLocation();
            }
            instance.Location.PropertyChanged += instance.Location_PropertyChanged;
            instance.UpdateMap();
        }

        private void Location_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateMap();
        }

        private void UpdateMap()
        {
            this.geoGMapControl.Position = new PointLatLng(this.Location.Latitude, this.Location.Longitude);
            this.geoGMapControl.Zoom = Math.Max(1, (double)this.Location.Accuracy);
        }

        #endregion

        #region SearchLocation

        /// <summary>
        /// Searches for a location and sets the current position if found.
        /// </summary>
        /// <param name="searchText">The location to search for.</param>
        /// <returns><see langword="true"/> if the location was found, <see langword="false"/> otherwise.</returns>
        public bool SearchLocation(string searchText)
        {
            var status = this.geoGMapControl.SetCurrentPositionByKeywords(searchText);
            return (status == GeoCoderStatusCode.G_GEO_SUCCESS);
        }

        #endregion

        #region GeoLocationMapProvider

        public GeoLocationMapProvider GeoLocationMapProvider
        {
            get { return (GeoLocationMapProvider)GetValue(GeoLocationMapProviderProperty); }
            set { SetValue(GeoLocationMapProviderProperty, value); }
        }

        public static readonly DependencyProperty GeoLocationMapProviderProperty = DependencyProperty.Register("GeoLocationMapProvider", typeof(GeoLocationMapProvider), typeof(GeoLocationMap), new UIPropertyMetadata(OnGeoLocationMapProviderChanged));

        private static void OnGeoLocationMapProviderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var instance = (GeoLocationMap)o;
            var mapTypeViewModel = availableMaps.FirstOrDefault(m => m.MapProvider == (GeoLocationMapProvider)e.NewValue) ?? availableMaps.First();
            instance.mapTypesComboBox.SelectedItem = mapTypeViewModel;
            instance.geoGMapControl.MapProvider = mapTypeViewModel.GMapMapType;
        }

        #endregion

        #region Private MapTypeViewModel Class

        private class MapTypeViewModel
        {
            public GeoLocationMapProvider MapProvider { get; private set; }
            public GMapProvider GMapMapType { get; private set; }
            public string Name { get; private set; }

            public MapTypeViewModel(GeoLocationMapProvider mapProvider, GMapProvider gMapMapType, string name)
            {
                this.MapProvider = mapProvider;
                this.GMapMapType = gMapMapType;
                this.Name = name;
            }
        }

        #endregion
    }
}