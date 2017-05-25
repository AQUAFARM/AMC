
namespace Schedulr.Models
{
    /// <summary>
    /// Determines the safety level for a picture.
    /// </summary>
    public enum Safety
    {
        /// <summary>
        /// No safety is specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Safe (suitable for a global family audience).
        /// </summary>
        Safe = 1,

        /// <summary>
        /// Moderate (the odd articstic nude is ok, but that's the limit).
        /// </summary>
        Moderate = 2,

        /// <summary>
        /// Restricted (suitable for over 18's only).
        /// </summary>
        Restricted = 3,
    }
}