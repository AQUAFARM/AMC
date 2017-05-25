using System.Configuration;
using System.IO;

namespace Schedulr.Providers
{
    /// <summary>
    /// Provides paths to certain files.
    /// </summary>
    internal static class PathProvider
    {
        /// <summary>
        /// Gets the path to the Schedulr configuration file.
        /// </summary>
        public static string ConfigurationFilePath
        {
            get
            {
                string configFileOverride = ConfigurationManager.AppSettings["ConfigurationFilePath"];
                if (!string.IsNullOrEmpty(configFileOverride))
                {
                    return configFileOverride;
                }
                else
                {
                    return Path.Combine(System.Windows.Forms.Application.LocalUserAppDataPath, "SchedulrConfiguration.xml");
                }
            }
        }

        /// <summary>
        /// Gets the path to the Schedulr log file.
        /// </summary>
        public static string LogFilePath
        {
            get
            {
                string logFileOverride = ConfigurationManager.AppSettings["LogFilePath"];
                if (!string.IsNullOrEmpty(logFileOverride))
                {
                    return logFileOverride;
                }
                else
                {
                    return Path.Combine(System.Windows.Forms.Application.LocalUserAppDataPath, "Schedulr.log");
                }
            }
        }

        /// <summary>
        /// Gets the path to the placeholder image for a Flickr group.
        /// </summary>
        public static string PlaceHolderImagePathFlickrGroup
        {
            get { return "/Resources/FlickrGroup.png"; }
        }

        /// <summary>
        /// Gets the path to the placeholder image for a Flickr set.
        /// </summary>
        public static string PlaceHolderImagePathFlickrSet
        {
            get { return "/Resources/FlickrSet.png"; }
        }
    }
}