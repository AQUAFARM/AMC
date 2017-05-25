using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace JelleDruyts.Windows.Text
{
    /// <summary>
    /// Replaces tokens in string templates with formatted values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Template"/> should contain tokens that are automatically discovered using
    /// the <see cref="TokenFormatExpression"/> property and replaced by their actual values
    /// in the <see cref="Process(IDictionary{string,object})"/> method.
    /// </para>
    /// <para>
    /// By default, tokens will be discovered using the $(Token) and $(Token:Format) syntax,
    /// where 'Token' is the token name and 'Format' (if provided) specifies the format string
    /// used to format the token's value.
    /// </para>
    /// <para>
    /// The format specifier is only used when the token data type being processed supports
    /// the <see cref="IFormattable"/> interface (i.e. <see cref="DateTime"/> and all numeric
    /// types) or if it is a string.
    /// </para>
    /// <para>
    /// For <see cref="IFormattable"/>, the format string is passed to its <see cref="IFormattable.ToString(string, IFormatProvider)"/>
    /// method so the syntax depends on the data type being formatted.
    /// </para>
    /// <para>
    /// For strings, the following format specifiers are allowed:
    /// <list type="bullet">
    ///   <item>
    ///     <term><c>Trim</c></term>
    ///     <description>Specifies that the string should be trimmed on both ends.</description>
    ///   </item>
    ///   <item>
    ///     <term><c>LowerCase</c></term>
    ///     <description>Specifies that the string should be converted to lower case.</description>
    ///   </item>
    ///   <item>
    ///     <term><c>UpperCase</c></term>
    ///     <description>Specifies that the string should be converted to upper case.</description>
    ///   </item>
    ///   <item>
    ///     <term><c>TitleCase</c></term>
    ///     <description>Specifies that the string should be converted to title case (i.e. upper case the first character of each word).</description>
    ///   </item>
    ///   <item>
    ///     <term><c>HtmlEncoded</c></term>
    ///     <description>Specifies that the string should be HTML encoded.</description>
    ///   </item>
    ///   <item>
    ///     <term><c>HtmlDecoded</c></term>
    ///     <description>Specifies that the string should be HTML decoded.</description>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// If a token value being passed to the <see cref="Process(IDictionary{string,object})"/> method is <see langword="null"/>,
    /// it will be replaced by an empty string by default, but this is customizable using the <see cref="TokenValueNullText"/>
    /// property.
    /// </para>
    /// <para>
    /// If a token value being passed to the <see cref="Process(IDictionary{string,object})"/> method is an <see cref="ICollection"/>,
    /// each item in the collection will separately be added to the processed template. The items will be
    /// separated by the <see cref="ArraySeparator"/> property.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows how to process a template with a format string for a date.
    /// <code>
    /// TemplateProcessor processor = new TemplateProcessor("Hello $(FirstName) $(LastName:), you were born on $(BirthDate:dd/MM/yyyy)");
    /// Dictionary&lt;string, object&gt; tokenValues = new Dictionary&lt;string, object&gt;();
    /// tokenValues.Add("FirstName", "Jeff");
    /// tokenValues.Add("LastName", "Buckley");
    /// tokenValues.Add("BirthDate", new DateTime(1966, 11, 17));
    /// string processed = processor.Process(tokenValues);
    /// // Returns "Hello Jeff Buckley, you were born on 17/11/1966"
    /// </code>
    /// The following example shows string formatting in action.
    /// <code>
    /// Dictionary&lt;string, object&gt; tokenValues = new Dictionary&lt;string, object&gt;();
    /// tokenValues.Add("HtmlText", "&lt;body&gt;Hi there&lt;/body&gt;");
    /// string processed = TemplateProcessor.Process("Here's some html: $(HtmlText:HtmlEncoded)", tokenValues);
    /// Returns: "Here's some html: &amp;lt;body&amp;gt;Hi there&amp;lt;/body&amp;gt;".
    /// </code>
    /// The following example shows the output for collections:
    /// <code>
    /// Dictionary&lt;string, object&gt; tokenValues = new Dictionary&lt;string, object&gt;();
    /// tokenValues.Add("Names", new string[] { "Jeff", "John", "Jane" });
    /// string processed = TemplateProcessor.Process("Hi there $(Names)", tokenValues);
    /// // Returns "Hi there Jeff, John, Jane".
    /// </code>
    /// The following example shows how to process a template with a custom xml-like token format.
    /// <code>
    /// TemplateProcessor processor = new TemplateProcessor("You were born on &lt;BirthDate format=\"yyyyMMdd\"/&gt;");
    /// processor.TokenFormatExpression = @"&lt;(?&lt;Name&gt;.*?)(\s+format=""(?&lt;Format&gt;.*?)"")?\s*/&gt;";
    /// Dictionary&lt;string, object&gt; tokenValues = new Dictionary&lt;string, object&gt;();
    /// tokenValues.Add("BirthDate", new DateTime(1966, 11, 17));
    /// string processed = processor.Process(tokenValues);
    /// // Returns "You were born on 17/11/1966".
    /// </code>
    /// </example>
    public class TemplateProcessor
    {
        #region Constants

        /// <summary>
        /// The default <see cref="TokenFormatExpression"/>.
        /// </summary>
        public const string DefaultTokenFormatExpression = @"\$\((?<Name>.*?)(:(?<Format>.*?))?\)";

        /// <summary>
        /// The default token format without a format specifier.
        /// </summary>
        public const string DefaultTokenFormatWithoutFormatSpecifier = "$({0})";

        /// <summary>
        /// The default token format with a format specifier.
        /// </summary>
        public const string DefaultTokenFormatWithFormatSpecifier = "$({0}:{1})";

        /// <summary>
        /// The default <see cref="TokenValueNullText"/>.
        /// </summary>
        public static readonly string DefaultTokenValueNullText;

        /// <summary>
        /// The default <see cref="ArraySeparator"/>.
        /// </summary>
        public const string DefaultArraySeparator = ", ";

        /// <summary>
        /// The string format specifier for Trim formatting.
        /// </summary>
        private const string StringFormatTrim = "Trim";

        /// <summary>
        /// The string format specifier for LowerCase formatting.
        /// </summary>
        private const string StringFormatLowerCase = "LowerCase";

        /// <summary>
        /// The string format specifier for UpperCase formatting.
        /// </summary>
        private const string StringFormatUpperCase = "UpperCase";

        /// <summary>
        /// The string format specifier for TitleCase formatting.
        /// </summary>
        private const string StringFormatTitleCase = "TitleCase";

        /// <summary>
        /// The string format specifier for HtmlEncoded formatting.
        /// </summary>
        private const string StringFormatHtmlEncoded = "HtmlEncoded";

        /// <summary>
        /// The string format specifier for HtmlDecoded formatting.
        /// </summary>
        private const string StringFormatHtmlDecoded = "HtmlDecoded";

        // Note: when adding string formats, be sure to update the GetSupportedStringFormats method.

        #endregion

        #region Fields

        /// <summary>
        /// The discovered tokens used in the template.
        /// </summary>
        private IEnumerable<DiscoveredToken> tokens;

        #endregion

        #region Template Property

        /// <summary>
        /// The template to process.
        /// </summary>
        private readonly string template;

        /// <summary>
        /// Gets the template to process.
        /// </summary>
        public string Template
        {
            get
            {
                return this.template;
            }
        }

        #endregion

        #region TokenFormatExpression Property

        /// <summary>
        /// The regular expression for the token format.
        /// </summary>
        private string tokenFormatExpression;

        /// <summary>
        /// Gets or sets the regular expression for the token format.
        /// </summary>
        /// <remarks>
        /// The regular expression must include a group named "Name" that finds the token name
        /// and optionally a group named "Format" that finds the token format specifier.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> argument cannot be <see langword="null"/>.</exception>
        /// <example>
        /// The following expression finds names and formats that match the <c>$(Name:Format)</c> syntax:
        /// <code>
        /// "\$\((?&lt;Name&gt;.*?)(:(?&lt;Format&gt;.*?))?\)"
        /// </code>
        /// The following expression finds names and formats that match the <c>&lt;Name format="Format"/&gt;</c> syntax:
        /// <code>
        /// "&lt;(?&lt;Name>.*?)(\s+format=""(?&lt;Format>.*?)"")?\s*/&gt;"
        /// </code>
        /// </example>
        public string TokenFormatExpression
        {
            get
            {
                return this.tokenFormatExpression;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.tokenFormatExpression = value;

                // Reset the discovered tokens.
                this.tokens = null;
            }
        }

        #endregion

        #region TokenValueNullText Property

        /// <summary>
        /// The text that is used when a <see langword="null"/> value is encountered for a token value.
        /// </summary>
        private string tokenValueNullText = DefaultTokenValueNullText;

        /// <summary>
        /// Gets or sets the text that is used when a <see langword="null"/> value is encountered for a token value.
        /// </summary>
        public string TokenValueNullText
        {
            get
            {
                return this.tokenValueNullText;
            }
            set
            {
                this.tokenValueNullText = value;
            }
        }

        #endregion

        #region ArraySeparator Property

        /// <summary>
        /// The array separator for token values that are an <see cref="ICollection"/> (e.g. arrays).
        /// </summary>
        private string arraySeparator = DefaultArraySeparator;

        /// <summary>
        /// Gets or sets the array separator for token values that are an <see cref="ICollection"/> (e.g. arrays).
        /// </summary>
        public string ArraySeparator
        {
            get
            {
                return this.arraySeparator;
            }
            set
            {
                this.arraySeparator = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TemplateProcessor"/> class.
        /// </summary>
        /// <param name="template">The template to process.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="template"/> argument cannot be <see langword="null"/>.</exception>
        public TemplateProcessor(string template)
            : this(template, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TemplateProcessor"/> class.
        /// </summary>
        /// <param name="template">The template to process.</param>
        /// <param name="tokenFormatExpression">The regular expression for the token format.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="template"/> argument cannot be <see langword="null"/>.</exception>
        public TemplateProcessor(string template, string tokenFormatExpression)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            this.template = template;
            if (string.IsNullOrEmpty(tokenFormatExpression))
            {
                this.tokenFormatExpression = DefaultTokenFormatExpression;
            }
            else
            {
                this.tokenFormatExpression = tokenFormatExpression;
            }
        }

        #endregion

        #region Process

        /// <summary>
        /// Processes the template by replacing all tokens with their values.
        /// </summary>
        /// <param name="tokenValues">The dictionary of token names with their values.</param>
        /// <returns>The template, with all tokens replaced by their token values.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenValues"/> argument cannot be <see langword="null"/>.</exception>
        public string Process(IDictionary<string, object> tokenValues)
        {
            if (tokenValues == null)
            {
                throw new ArgumentNullException("tokenValues");
            }

            if (this.tokens == null)
            {
                DiscoverTokens();
            }

            string processed = this.template;
            foreach (DiscoveredToken token in this.tokens)
            {
                // Only process the token if there's a corresponding value for it.
                if (tokenValues.ContainsKey(token.Name))
                {
                    // Get the value.
                    object tokenValue = tokenValues[token.Name];

                    // Get the replacement text.
                    string tokenValueText = GetTokenValueText(token, tokenValue);

                    // Replace the token.
                    processed = processed.Replace(token.TemplateText, tokenValueText);
                }
            }

            return processed;
        }

        /// <summary>
        /// Gets the text for the token value.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="tokenValue">The token value.</param>
        /// <returns>The text for the token value.</returns>
        private string GetTokenValueText(DiscoveredToken token, object tokenValue)
        {
            // Start off with the null text.
            string tokenValueText = this.tokenValueNullText;

            // Replace it if there's a value.
            if (tokenValue != null)
            {
                // Special case for IEnumerables.
                ICollection tokenCollection = tokenValue as ICollection;
                if (tokenCollection != null)
                {
                    StringBuilder tokenValueTexts = new StringBuilder();
                    foreach (object tokenItem in tokenCollection)
                    {
                        if (tokenValueTexts.Length > 0)
                        {
                            tokenValueTexts.Append(this.arraySeparator);
                        }
                        tokenValueTexts.Append(GetTokenValueText(token, tokenItem));
                    }
                    tokenValueText = tokenValueTexts.ToString();
                }
                else
                {
                    // Format the value if possible.
                    IFormattable formattableTokenValue = tokenValue as IFormattable;
                    if (!string.IsNullOrEmpty(token.Format) && formattableTokenValue != null)
                    {
                        tokenValueText = formattableTokenValue.ToString(token.Format, null);
                    }
                    else
                    {
                        tokenValueText = tokenValue.ToString();

                        // Special case for strings.
                        if (tokenValue is string)
                        {
                            var textInfo = CultureInfo.CurrentCulture.TextInfo;
                            if (string.Equals(token.Format, StringFormatTrim, StringComparison.OrdinalIgnoreCase))
                            {
                                tokenValueText = tokenValueText.Trim();
                            }
                            else if (string.Equals(token.Format, StringFormatLowerCase, StringComparison.OrdinalIgnoreCase))
                            {
                                tokenValueText = textInfo.ToLower(tokenValueText);
                            }
                            else if (string.Equals(token.Format, StringFormatUpperCase, StringComparison.OrdinalIgnoreCase))
                            {
                                tokenValueText = textInfo.ToUpper(tokenValueText);
                            }
                            else if (string.Equals(token.Format, StringFormatTitleCase, StringComparison.OrdinalIgnoreCase))
                            {
                                tokenValueText = textInfo.ToTitleCase(tokenValueText);
                            }
                            else if (string.Equals(token.Format, StringFormatHtmlEncoded, StringComparison.OrdinalIgnoreCase))
                            {
                                tokenValueText = WebUtility.HtmlEncode(tokenValueText);
                            }
                            else if (string.Equals(token.Format, StringFormatHtmlDecoded, StringComparison.OrdinalIgnoreCase))
                            {
                                tokenValueText = WebUtility.HtmlDecode(tokenValueText);
                            }
                        }
                    }
                }
            }
            return tokenValueText;
        }

        #endregion

        #region Static Process Method

        /// <summary>
        /// Processes the template by replacing all tokens with their values.
        /// </summary>
        /// <param name="template">The template to process.</param>
        /// <param name="tokenValues">The dictionary of token names with their values.</param>
        /// <returns>The template, with all tokens replaced by their token values.</returns>
        public static string Process(string template, IDictionary<string, object> tokenValues)
        {
            if (string.IsNullOrEmpty(template))
            {
                return template;
            }
            else
            {
                TemplateProcessor processor = new TemplateProcessor(template);
                return processor.Process(tokenValues);
            }
        }

        /// <summary>
        /// Processes the template by replacing all tokens with their values.
        /// </summary>
        /// <param name="template">The template to process.</param>
        /// <param name="tokenValues">The dictionary of token names with their values.</param>
        /// <param name="tokenFormatExpression">The regular expression for the token format.</param>
        /// <returns>The template, with all tokens replaced by their token values.</returns>
        public static string Process(string template, IDictionary<string, object> tokenValues, string tokenFormatExpression)
        {
            if (string.IsNullOrEmpty(template))
            {
                return template;
            }
            else
            {
                TemplateProcessor processor = new TemplateProcessor(template, tokenFormatExpression);
                return processor.Process(tokenValues);
            }
        }

        #endregion

        #region Static GetSupportedStringFormats

        /// <summary>
        /// Gets the supported string format specifiers.
        /// </summary>
        /// <returns>The supported string format specifiers.</returns>
        public static string[] GetSupportedStringFormats()
        {
            return new string[] { StringFormatTrim, StringFormatLowerCase, StringFormatUpperCase, StringFormatTitleCase, StringFormatHtmlEncoded, StringFormatHtmlDecoded };
        }

        #endregion

        #region Tokens Discovery

        /// <summary>
        /// Discovers the tokens in the template.
        /// </summary>
        private void DiscoverTokens()
        {
            List<DiscoveredToken> discoveredTokens = new List<DiscoveredToken>();

            Regex tokenExpression = new Regex(this.tokenFormatExpression);
            foreach (Match tokenMatch in tokenExpression.Matches(this.template))
            {
                if (tokenMatch.Success)
                {
                    if (tokenMatch.Groups["Name"].Success)
                    {
                        string name = tokenMatch.Groups["Name"].Value;
                        string templateText = tokenMatch.Value;
                        string format = null;
                        if (tokenMatch.Groups["Format"].Success)
                        {
                            format = tokenMatch.Groups["Format"].Value;
                        }
                        discoveredTokens.Add(new DiscoveredToken(templateText, name, format));
                    }
                }
            }

            this.tokens = discoveredTokens.ToArray();
        }

        /// <summary>
        /// A discovered token.
        /// </summary>
        private class DiscoveredToken
        {
            public string TemplateText;
            public string Name;
            public string Format;

            public DiscoveredToken(string templateText, string name, string format)
            {
                this.TemplateText = templateText;
                this.Name = name;
                this.Format = format;
            }
        }

        #endregion
    }
}