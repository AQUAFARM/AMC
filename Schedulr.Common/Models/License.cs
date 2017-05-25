
namespace Schedulr.Models
{
    /// <summary>
    /// Determines the content type for a picture.
    /// </summary>
    public enum License
    {
        /// <summary>
        /// No license is specified.
        /// </summary>
        None = -1,

        /// <summary>
        /// None (All rights reserved).
        /// </summary>
        AllRightsReserved = 0,

        /// <summary>
        /// Attribution-NonCommercial-ShareAlike Creative Commons.
        /// </summary>
        AttributionNoncommercialShareAlikeCC = 1,

        /// <summary>
        /// Attribution-NonCommercial Creative Commons
        /// </summary>
        AttributionNoncommercialCC = 2,

        /// <summary>
        /// Attribution-NonCommercial-NoDerivs Creative Commons
        /// </summary>
        AttributionNoncommercialNoDerivativesCC = 3,

        /// <summary>
        /// Attribution Creative Commons
        /// </summary>
        AttributionCC = 4,

        /// <summary>
        /// Attribution-ShareAlike Creative Commons
        /// </summary>
        AttributionShareAlikeCC = 5,

        /// <summary>
        /// Attribution-NoDerivs Creative Commons
        /// </summary>
        AttributionNoDerivativesCC = 6,
    }
}