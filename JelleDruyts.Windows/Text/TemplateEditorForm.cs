using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace JelleDruyts.Windows.Text
{
    /// <summary>
    /// A form that allows the user to edit a text template with tokens.
    /// </summary>
    /// <seealso cref="TemplateProcessor"/>
    public partial class TemplateEditorForm : Form
    {
        #region Fields

        /// <summary>
        /// The token values to be used for the preview window.
        /// </summary>
        private IDictionary<string, object> tokenValues = new Dictionary<string, object>();

        /// <summary>
        /// The known format specifiers for standard formatting (DateTime and Numeric).
        /// </summary>
        private static readonly string[] KnownStandardFormatSpecifiers;

        /// <summary>
        /// The known format specifiers for string formatting.
        /// </summary>
        private static readonly string[] KnownStringFormatSpecifiers;

        #endregion

        #region Static Constructor

        /// <summary>
        /// Initializes the <see cref="T:TemplateEditorForm"/> class.
        /// </summary>
        static TemplateEditorForm()
        {
            List<string> specifiers = new List<string>();
            specifiers.Add("<type or select>");
            specifiers.Add("c");
            specifiers.Add("d");
            specifiers.Add("D");
            specifiers.Add("e");
            specifiers.Add("f");
            specifiers.Add("F");
            specifiers.Add("g");
            specifiers.Add("G");
            specifiers.Add("m");
            specifiers.Add("n");
            specifiers.Add("r");
            specifiers.Add("s");
            specifiers.Add("t");
            specifiers.Add("T");
            specifiers.Add("u");
            specifiers.Add("U");
            specifiers.Add("x");
            specifiers.Add("y");
            specifiers.AddRange(DateTimeFormatInfo.InvariantInfo.GetAllDateTimePatterns());
            KnownStandardFormatSpecifiers = specifiers.ToArray();

            specifiers.Clear();
            specifiers.Add(string.Empty);
            specifiers.AddRange(TemplateProcessor.GetSupportedStringFormats());
            KnownStringFormatSpecifiers = specifiers.ToArray();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TemplateEditorUI"/> class.
        /// </summary>
        public TemplateEditorForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Template Property

        /// <summary>
        /// Gets or sets the template being edited.
        /// </summary>
        public string Template
        {
            get
            {
                return templateText.Text;
            }
            set
            {
                templateText.Text = value;
            }
        }

        #endregion

        #region AvailableTokens Property

        /// <summary>
        /// The available tokens.
        /// </summary>
        private IList<TemplateTokenInfo> availableTokens;

        /// <summary>
        /// Gets or sets the available tokens.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> argument cannot be <see langword="null"/>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<TemplateTokenInfo> AvailableTokens
        {
            get
            {
                return this.availableTokens;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.availableTokens = value;

                // Update the token values.
                this.tokenValues.Clear();
                foreach (TemplateTokenInfo tokenInfo in this.availableTokens)
                {
                    this.tokenValues.Add(tokenInfo.Name, tokenInfo.SampleValue);
                }
                UpdateTokenList();
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
        /// <seealso cref="TemplateProcessor.TokenFormatExpression"/>
        public string TokenFormatExpression
        {
            get
            {
                return this.tokenFormatExpression;
            }
            set
            {
                this.tokenFormatExpression = value;
                UpdateTokenList();
            }
        }

        #endregion

        #region TokenFormatWithoutFormatSpecifier Property

        /// <summary>
        /// The token format string if there is no format specifier for the token.
        /// </summary>
        private string tokenFormatWithoutFormatSpecifier = TemplateProcessor.DefaultTokenFormatWithoutFormatSpecifier;

        /// <summary>
        /// Gets the token format string if there is no format specifier for the token.
        /// </summary>
        public string TokenFormatWithoutFormatSpecifier
        {
            get
            {
                return this.tokenFormatWithoutFormatSpecifier;
            }
            set
            {
                this.tokenFormatWithoutFormatSpecifier = value;
                UpdateTokenList();
            }
        }

        #endregion

        #region TokenFormatWithFormatSpecifier Property

        /// <summary>
        /// The token format string if there is also a format specifier for the token.
        /// </summary>
        private string tokenFormatWithFormatSpecifier = TemplateProcessor.DefaultTokenFormatWithFormatSpecifier;

        /// <summary>
        /// Gets the token format string if there is also a format specifier for the token.
        /// </summary>
        public string TokenFormatWithFormatSpecifier
        {
            get
            {
                return this.tokenFormatWithFormatSpecifier;
            }
            set
            {
                this.tokenFormatWithFormatSpecifier = value;
                UpdateTokenList();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the Click event of the okButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the cancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Handles the Click event of the insertButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void insertButton_Click(object sender, EventArgs e)
        {
            InsertToken();
        }

        /// <summary>
        /// Handles the TextChanged event of the templateText control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void templateText_TextChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the tokensList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void tokensList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the formatList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void formatList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTokenSampleValue();
        }

        /// <summary>
        /// Handles the TextChanged event of the formatList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void formatList_TextChanged(object sender, EventArgs e)
        {
            UpdateTokenSampleValue();
        }

        #endregion

        #region UI Handling

        /// <summary>
        /// Updates the list of tokens.
        /// </summary>
        private void UpdateTokenList()
        {
            this.tokenList.DisplayMember = "Name";
            this.tokenList.DataSource = this.availableTokens;
            if (this.tokenList.Items.Count > 0)
            {
                this.tokenList.SelectedIndex = 0;
            }
            UpdatePreview();
        }

        /// <summary>
        /// Inserts the selected token into the template.
        /// </summary>
        private void InsertToken()
        {
            if (this.tokenList.SelectedItem != null)
            {
                TemplateTokenInfo tokenInfo = (TemplateTokenInfo)this.tokenList.SelectedItem;
                string formatSpecifier = this.formatList.Text.Trim();
                string templateToken = GetTemplateToken(tokenInfo, formatSpecifier);
                this.templateText.SelectedText = templateToken + " ";
            }
        }

        private string GetTemplateToken(TemplateTokenInfo tokenInfo, string formatSpecifier)
        {
            string token = tokenInfo.Name;
            string templateToken;
            if (tokenInfo.SupportedFormat == TemplateTokenSupportedFormat.None
                || string.Equals(KnownStandardFormatSpecifiers[0], formatSpecifier, StringComparison.Ordinal)
                || string.IsNullOrEmpty(formatSpecifier))
            {
                // A format text has not been specified.
                templateToken = string.Format(CultureInfo.CurrentCulture, this.tokenFormatWithoutFormatSpecifier, token);
            }
            else
            {
                // A format text has been specified, include that.
                templateToken = string.Format(CultureInfo.CurrentCulture, this.tokenFormatWithFormatSpecifier, token, formatSpecifier);
            }
            return templateToken;
        }

        /// <summary>
        /// Updates the preview box.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void UpdatePreview()
        {
            if (string.IsNullOrEmpty(this.Template) || this.availableTokens == null)
            {
                this.previewText.Text = null;
            }
            else
            {
                try
                {
                    this.previewText.Text = TemplateProcessor.Process(this.Template, this.tokenValues, this.TokenFormatExpression);
                }
                catch (Exception exc)
                {
                    this.previewText.Text = GetExceptionMessage(exc);
                }
            }
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        private void UpdateUI()
        {
            if (this.tokenList.SelectedItem != null)
            {
                TemplateTokenInfo tokenInfo = (TemplateTokenInfo)this.tokenList.SelectedItem;

                // Show options for well-known formats (e.g. strings).
                if (tokenInfo.SupportedFormat == TemplateTokenSupportedFormat.Standard)
                {
                    // Allow any format but suggest some values.
                    this.formatList.DataSource = KnownStandardFormatSpecifiers;
                    this.formatList.SelectedIndex = 0;
                    this.formatList.DropDownStyle = ComboBoxStyle.DropDown;
                }
                else if (tokenInfo.SupportedFormat == TemplateTokenSupportedFormat.String)
                {
                    // Restrict the input to the possible values.
                    this.formatList.DataSource = KnownStringFormatSpecifiers;
                    this.formatList.SelectedIndex = 0;
                    this.formatList.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                else
                {
                    // Don't show any format.
                    this.formatList.DataSource = null;
                    this.formatList.Text = string.Empty;
                }
                this.formatList.Enabled = (tokenInfo.SupportedFormat != TemplateTokenSupportedFormat.None);
            }

            this.insertButton.Enabled = (this.tokenList.SelectedItem != null);
            UpdateTokenSampleValue();
        }

        private void UpdateTokenSampleValue()
        {
            string tokenSampleValue = null;
            if (this.tokenList.SelectedItem != null)
            {
                TemplateTokenInfo tokenInfo = (TemplateTokenInfo)this.tokenList.SelectedItem;
                string formatSpecifier = this.formatList.Text.Trim();
                var tokenValueTemplate = GetTemplateToken(tokenInfo, formatSpecifier);
                try
                {
                    tokenSampleValue = TemplateProcessor.Process(tokenValueTemplate, this.tokenValues, this.TokenFormatExpression);
                }
                catch (Exception exc)
                {
                    tokenSampleValue = GetExceptionMessage(exc);
                }

            }
            this.tokenSampleValueTextBox.Text = tokenSampleValue;
        }

        private static string GetExceptionMessage(Exception exc)
        {
            var message = string.Format(CultureInfo.CurrentCulture, "[{0}: {1}]", exc.GetType().Name, exc.Message);
            return message;
        }

        #endregion
    }
}