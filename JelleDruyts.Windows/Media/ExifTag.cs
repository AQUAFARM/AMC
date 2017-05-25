
namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Represents the value associated with an <see cref="ExifTagType"/>.
    /// </summary>
    public class ExifTag
    {
        #region Properties

        /// <summary>
        /// Gets the type of EXIF tag this value belongs to.
        /// </summary>
        public ExifTagType Type { get; private set; }

        /// <summary>
        /// Gets the value of the EXIF tag.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Gets a textual interpretation of the value of the EXIF tag.
        /// </summary>
        public string ValueInterpretation { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExifTag"/> class.
        /// </summary>
        /// <param name="type">The type of EXIF tag this value belongs to.</param>
        /// <param name="value">The value of the EXIF tag.</param>
        public ExifTag(ExifTagType type, object value)
            : this(type, value, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExifTag"/> class.
        /// </summary>
        /// <param name="type">The type of EXIF tag this value belongs to.</param>
        /// <param name="value">The value of the EXIF tag.</param>
        /// <param name="valueInterpretation">The textual description of the value of the EXIF tag.</param>
        public ExifTag(ExifTagType type, object value, string valueInterpretation)
        {
            this.Type = type;
            this.Value = value;
            this.ValueInterpretation = valueInterpretation;
        }

        #endregion
    }
}