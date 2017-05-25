
namespace Schedulr.Models
{
    /// <summary>
    /// Determines how picture collections are sorted.
    /// </summary>
    public enum PictureCollectionSortMode
    {
        /// <summary>
        /// Picture collections are sorted by name (ascending).
        /// </summary>
        NameAscending,

        /// <summary>
        /// Picture collections are sorted by name (descending).
        /// </summary>
        NameDescending,

        /// <summary>
        /// Picture collections are sorted by age (ascending).
        /// </summary>
        AgeAscending,

        /// <summary>
        /// Picture collections are sorted by age (descending).
        /// </summary>
        AgeDescending,

        /// <summary>
        /// Picture collections are sorted by user order (ascending).
        /// </summary>
        UserOrderAscending,

        /// <summary>
        /// Picture collections are sorted by user order (descending).
        /// </summary>
        UserOrderDescending,

        /// <summary>
        /// Picture collections are sorted by size (ascending).
        /// </summary>
        SizeAscending,

        /// <summary>
        /// Picture collections are sorted by size (descending).
        /// </summary>
        SizeDescending
    }
}
