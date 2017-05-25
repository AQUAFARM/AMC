
namespace Schedulr.Plugins.MonitorFolders
{
    /// <summary>
    /// Determines how searches are performed.
    /// </summary>
    public enum SearchMode
    {
        /// <summary>
        /// Search by including all files matching the search pattern.
        /// </summary>
        Include,

        /// <summary>
        /// Search by including all files except those matching the exclude pattern.
        /// </summary>
        Exclude
    }
}