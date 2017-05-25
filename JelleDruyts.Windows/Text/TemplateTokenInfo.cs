using System;
using System.Collections;

namespace JelleDruyts.Windows.Text
{
    /// <summary>
    /// Holds information about a template token.
    /// </summary>
    public class TemplateTokenInfo
    {
        #region Name Property

        /// <summary>
        /// The name of the token.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Gets the name of the token.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        #endregion

        #region SampleValue Property

        /// <summary>
        /// A sample value for the token, to be used in a preview window.
        /// </summary>
        private object sampleValue;

        /// <summary>
        /// Gets sample value for the token, to be used in a preview window.
        /// </summary>
        public object SampleValue
        {
            get
            {
                return this.sampleValue;
            }
        }

        #endregion

        #region SupportedFormat Property

        /// <summary>
        /// The supported formatting for the token.
        /// </summary>
        private TemplateTokenSupportedFormat supportedFormat;

        /// <summary>
        /// Gets the supported formatting for the token.
        /// </summary>
        public TemplateTokenSupportedFormat SupportedFormat
        {
            get
            {
                return this.supportedFormat;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TemplateTokenInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the token.</param>
        /// <param name="sampleValue">A sample value for the token, to be used in a preview window.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> argument cannot be <see langword="null"/>.</exception>
        public TemplateTokenInfo(string name, object sampleValue)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.name = name;
            this.sampleValue = sampleValue;

            TemplateTokenSupportedFormat supportedFormat = TemplateTokenSupportedFormat.None;
            if (sampleValue != null)
            {
                object sampleValueItem = sampleValue;
                ICollection collection = sampleValue as ICollection;
                if (collection != null)
                {
                    // Find the first non-null item in the collection.
                    foreach (object item in collection)
                    {
                        if (item != null)
                        {
                            sampleValueItem = item;
                            break;
                        }
                    }
                }
                if (sampleValueItem is IFormattable)
                {
                    supportedFormat = TemplateTokenSupportedFormat.Standard;
                }
                else if (sampleValueItem is string)
                {
                    supportedFormat = TemplateTokenSupportedFormat.String;
                }
            }
            this.supportedFormat = supportedFormat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TemplateTokenInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the token.</param>
        /// <param name="sampleValue">A sample value for the token, to be used in a preview window.</param>
        /// <param name="supportedFormat">The supported formatting for the token.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> argument cannot be <see langword="null"/>.</exception>
        public TemplateTokenInfo(string name, object sampleValue, TemplateTokenSupportedFormat supportedFormat)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.name = name;
            this.sampleValue = sampleValue;
            this.supportedFormat = supportedFormat;
        }

        #endregion
    }
}