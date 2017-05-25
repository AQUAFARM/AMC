
namespace Schedulr.Messages
{
    /// <summary>
    /// Defines the possible reasons an upload was requested.
    /// </summary>
    public enum UploadPicturesRequestReason
    {
        /// <summary>
        /// An upload was requested from the command line to upload pictures in the background.
        /// </summary>
        CommandLineBackground,

        /// <summary>
        /// An upload was requested from the command line to upload pictures through the user interface.
        /// </summary>
        CommandLineUI,

        /// <summary>
        /// An upload was requested interactively by the user.
        /// </summary>
        Interactive
    }
}