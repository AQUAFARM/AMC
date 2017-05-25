
namespace Schedulr.Models
{
    /// <summary>
    /// Determines the visibility of a picture in the global search results.
    /// </summary>
    public enum SearchVisibility
    {
        /// <summary>
        /// No visibility is specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The picture is visible in the global search results.
        /// </summary>
        Visible = 1,

        /// <summary>
        /// The picture is hidden in the global search results.
        /// </summary>
        Hidden = 2,
    }
}