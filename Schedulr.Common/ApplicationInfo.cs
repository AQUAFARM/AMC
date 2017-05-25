
namespace Schedulr
{
    /// <summary>
    /// Contains information about the application.
    /// </summary>
    public class ApplicationInfo
    {
        #region Properties

        /// <summary>
        /// Gets the URL where the application lives.
        /// </summary>
        public string Url { get { return Constants.ApplicationUrl; } }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public string Name { get { return Constants.ApplicationName; } }

        /// <summary>
        /// Gets the executable name of the application.
        /// </summary>
        public string ExecutableName { get; private set; }

        /// <summary>
        /// Gets the path to the executable of the application.
        /// </summary>
        public string ExecutablePath { get; private set; }

        /// <summary>
        /// Gets the full path of the configuration file.
        /// </summary>
        public string ConfigurationFileName { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInfo"/> class.
        /// </summary>
        /// <param name="executableName">The executable name of the application.</param>
        /// <param name="executablePath">The path to the executable of the application.</param>
        /// <param name="configurationFileName">The full path of the configuration file.</param>
        public ApplicationInfo(string executableName, string executablePath, string configurationFileName)
        {
            this.ExecutableName = executableName;
            this.ExecutablePath = executablePath;
            this.ConfigurationFileName = configurationFileName;
        }

        #endregion
    }
}