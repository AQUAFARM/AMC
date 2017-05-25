
namespace Schedulr
{
    /// <summary>
    /// Defines the optional steps that are performed during a picture upload.
    /// </summary>
    public enum PictureUploadOptionalStep
    {
        /// <summary>
        /// The step that adds a picture to sets.
        /// </summary>
        AddToSets,

        /// <summary>
        /// The step that adds a picture to groups.
        /// </summary>
        AddToGroups,

        /// <summary>
        /// The step that sets the license of a picture.
        /// </summary>
        SetLicense,

        /// <summary>
        /// The step that sets the geographic location of a picture.
        /// </summary>
        SetGeoLocation,
    }
}