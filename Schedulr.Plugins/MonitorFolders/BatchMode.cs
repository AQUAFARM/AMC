
namespace Schedulr.Plugins.MonitorFolders
{
    /// <summary>
    /// Determines how batches are created when adding files.
    /// </summary>
    public enum BatchMode
    {
        /// <summary>
        /// Create a batch per file that is added.
        /// </summary>
        BatchPerFile,

        /// <summary>
        /// Create a batch per folder in which files are being added.
        /// </summary>
        BatchPerFolder,

        /// <summary>
        /// Create a single batch for all files that are added.
        /// </summary>
        SingleBatch
    }
}