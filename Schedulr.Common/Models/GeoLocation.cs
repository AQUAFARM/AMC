using System;
using System.Globalization;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// Represents a geographic location with a certain accuracy.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class GeoLocation : ObservableObject
    {
        #region Constants

        /// <summary>
        /// The minimum accepted accuracy.
        /// </summary>
        public const int MinAccuracy = 0;

        /// <summary>
        /// The maximum accepted accuracy.
        /// </summary>
        public const int MaxAccuracy = 16;

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        [DataMember]
        public double Latitude
        {
            get { return this.GetValue(LatitudeProperty); }
            set { this.SetValue(LatitudeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Latitude"/> observable property.
        /// </summary>
        public static ObservableProperty<double> LatitudeProperty = new ObservableProperty<double, GeoLocation>(o => o.Latitude);

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        [DataMember]
        public double Longitude
        {
            get { return this.GetValue(LongitudeProperty); }
            set { this.SetValue(LongitudeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Longitude"/> observable property.
        /// </summary>
        public static ObservableProperty<double> LongitudeProperty = new ObservableProperty<double, GeoLocation>(o => o.Longitude);

        /// <summary>
        /// Gets or sets the accuracy of the location between 1 (world level) and 16 (street level) or 0 if the accuracy is unknown.
        /// </summary>
        [DataMember]
        public int Accuracy
        {
            get { return this.GetValue(AccuracyProperty); }
            set { this.SetValue(AccuracyProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Accuracy"/> observable property.
        /// </summary>
        public static ObservableProperty<int> AccuracyProperty = new ObservableProperty<int, GeoLocation>(o => o.Accuracy, OnAccuracyChanged);

        private static void OnAccuracyChanged(ObservableObject sender, ObservablePropertyChangedEventArgs<int> e)
        {
            if (e.NewValue < MinAccuracy)
            {
                throw new ArgumentException("The accuracy cannot be smaller than " + MinAccuracy);
            }
            if (e.NewValue > MaxAccuracy)
            {
                throw new ArgumentException("The accuracy cannot be greater than " + MaxAccuracy);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoLocation"/> class.
        /// </summary>
        public GeoLocation()
            : this(0, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoLocation"/> class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        public GeoLocation(double latitude, double longitude)
            : this(latitude, longitude, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoLocation"/> class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="accuracy">The accuracy of the location between 1 (world level) and 16 (street level) or 0 if the accuracy is unknown.</param>
        public GeoLocation(double latitude, double longitude, int accuracy)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Accuracy = accuracy;
        }

        #endregion

        #region ToString

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Lat = {0}, Lng = {1}, Accuracy = {2}", this.Latitude, this.Longitude, this.Accuracy);
        }

        #endregion

        #region Equals & GetHashCode

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as GeoLocation;
            if (other != null)
            {
                return (other.Latitude == this.Latitude && other.Longitude == this.Longitude && other.Accuracy == this.Accuracy);
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return (this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode() ^ this.Accuracy);
        }

        #endregion
    }
}