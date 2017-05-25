using System;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Defines the allowed resize directions.
    /// </summary>
    [Flags]
    public enum AllowedResizeDirections
    {
        /// <summary>
        /// No resize is allowed.
        /// </summary>
        None = 0,

        /// <summary>
        /// Resizing up is allowed.
        /// </summary>
        Up = 1,

        /// <summary>
        /// Resizing down is allowed.
        /// </summary>
        Down = 2,

        /// <summary>
        /// Resizing up and down are allowed.
        /// </summary>
        All = Up | Down
    }
}