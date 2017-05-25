using System.Net.NetworkInformation;

namespace JelleDruyts.Windows
{
    /// <summary>
    /// Provides convenience properties for network connectivity.
    /// </summary>
    public static class Network
    {
        #region Fields

        private static bool? isAvailable;

        #endregion

        #region Static Constructor

        static Network()
        {
            NetworkChange.NetworkAvailabilityChanged += (sender, e) => { isAvailable = null; };
        }

        #endregion

        #region IsAvailable

        /// <summary>
        /// Indicates whether any network connection is available.
        /// </summary>
        /// <returns><see langword="true"/> if a network connection is available; otherwise, <see langword="false"/>.</returns>
        /// <remarks>This is an optimized wrapper around the <see cref="NetworkInterface.GetIsNetworkAvailable"/> method that has much better performance when called multiple times.</remarks>
        public static bool IsAvailable()
        {
            if (!isAvailable.HasValue)
            {
                isAvailable = NetworkInterface.GetIsNetworkAvailable();
            }
            return isAvailable.Value;
        }

        #endregion
    }
}