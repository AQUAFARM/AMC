
namespace JelleDruyts.Windows.Text
{
    /// <summary>
    /// The supported formats for a template token.
    /// </summary>
    public enum TemplateTokenSupportedFormat
    {
        /// <summary>
        /// The token does not support formatting.
        /// </summary>
        None = 0,

        /// <summary>
        /// The token supports standard formatting.
        /// </summary>
        Standard = 1,

        /// <summary>
        /// The token supports string formatting.
        /// </summary>
        String = 2
    }
}