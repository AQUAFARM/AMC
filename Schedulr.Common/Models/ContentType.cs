
namespace Schedulr.Models
{
    /// <summary>
    /// Determines the content type for a picture.
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// No content type is specified.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Represents a normal photograph.
        /// </summary>
        Photo = 1,
        
        /// <summary>
        /// Represents a screenshot.
        /// </summary>
        Screenshot = 2,

        /// <summary>
        /// Represents other content types, such as artwork.
        /// </summary>
        Other = 3,
    }
}