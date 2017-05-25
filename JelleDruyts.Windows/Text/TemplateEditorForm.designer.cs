namespace JelleDruyts.Windows.Text
{
    partial class TemplateEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.templateText = new System.Windows.Forms.TextBox();
            this.tokenList = new System.Windows.Forms.ComboBox();
            this.tokenLabel = new System.Windows.Forms.Label();
            this.formatLabel = new System.Windows.Forms.Label();
            this.insertButton = new System.Windows.Forms.Button();
            this.splitter = new System.Windows.Forms.SplitContainer();
            this.templateBox = new System.Windows.Forms.GroupBox();
            this.previewBox = new System.Windows.Forms.GroupBox();
            this.previewText = new System.Windows.Forms.TextBox();
            this.formatList = new System.Windows.Forms.ComboBox();
            this.tokensGroupBox = new System.Windows.Forms.GroupBox();
            this.tokenSampleValueTextBox = new System.Windows.Forms.TextBox();
            this.tokenSampleValueLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
            this.splitter.Panel1.SuspendLayout();
            this.splitter.Panel2.SuspendLayout();
            this.splitter.SuspendLayout();
            this.templateBox.SuspendLayout();
            this.previewBox.SuspendLayout();
            this.tokensGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(456, 407);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(537, 407);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // templateText
            // 
            this.templateText.AcceptsReturn = true;
            this.templateText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.templateText.Location = new System.Drawing.Point(3, 16);
            this.templateText.Multiline = true;
            this.templateText.Name = "templateText";
            this.templateText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.templateText.Size = new System.Drawing.Size(591, 133);
            this.templateText.TabIndex = 0;
            this.templateText.WordWrap = false;
            this.templateText.TextChanged += new System.EventHandler(this.templateText_TextChanged);
            // 
            // tokenList
            // 
            this.tokenList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tokenList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tokenList.FormattingEnabled = true;
            this.tokenList.Location = new System.Drawing.Point(61, 21);
            this.tokenList.Name = "tokenList";
            this.tokenList.Size = new System.Drawing.Size(230, 21);
            this.tokenList.TabIndex = 1;
            this.tokenList.SelectedIndexChanged += new System.EventHandler(this.tokensList_SelectedIndexChanged);
            // 
            // tokenLabel
            // 
            this.tokenLabel.AutoSize = true;
            this.tokenLabel.Location = new System.Drawing.Point(9, 24);
            this.tokenLabel.Name = "tokenLabel";
            this.tokenLabel.Size = new System.Drawing.Size(41, 13);
            this.tokenLabel.TabIndex = 0;
            this.tokenLabel.Text = "Token:";
            // 
            // formatLabel
            // 
            this.formatLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.formatLabel.AutoSize = true;
            this.formatLabel.Location = new System.Drawing.Point(297, 24);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(88, 13);
            this.formatLabel.TabIndex = 2;
            this.formatLabel.Text = "Format (optional):";
            // 
            // insertButton
            // 
            this.insertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.insertButton.Enabled = false;
            this.insertButton.Location = new System.Drawing.Point(516, 19);
            this.insertButton.Name = "insertButton";
            this.insertButton.Size = new System.Drawing.Size(69, 49);
            this.insertButton.TabIndex = 4;
            this.insertButton.Text = "&Insert";
            this.insertButton.UseVisualStyleBackColor = true;
            this.insertButton.Click += new System.EventHandler(this.insertButton_Click);
            // 
            // splitter
            // 
            this.splitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitter.Location = new System.Drawing.Point(15, 96);
            this.splitter.Name = "splitter";
            this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitter.Panel1
            // 
            this.splitter.Panel1.Controls.Add(this.templateBox);
            // 
            // splitter.Panel2
            // 
            this.splitter.Panel2.Controls.Add(this.previewBox);
            this.splitter.Size = new System.Drawing.Size(597, 305);
            this.splitter.SplitterDistance = 152;
            this.splitter.TabIndex = 5;
            // 
            // templateBox
            // 
            this.templateBox.Controls.Add(this.templateText);
            this.templateBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.templateBox.Location = new System.Drawing.Point(0, 0);
            this.templateBox.Name = "templateBox";
            this.templateBox.Size = new System.Drawing.Size(597, 152);
            this.templateBox.TabIndex = 0;
            this.templateBox.TabStop = false;
            this.templateBox.Text = "Template";
            // 
            // previewBox
            // 
            this.previewBox.Controls.Add(this.previewText);
            this.previewBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewBox.Location = new System.Drawing.Point(0, 0);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(597, 149);
            this.previewBox.TabIndex = 0;
            this.previewBox.TabStop = false;
            this.previewBox.Text = "Preview";
            // 
            // previewText
            // 
            this.previewText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewText.Location = new System.Drawing.Point(3, 16);
            this.previewText.Multiline = true;
            this.previewText.Name = "previewText";
            this.previewText.ReadOnly = true;
            this.previewText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.previewText.Size = new System.Drawing.Size(591, 130);
            this.previewText.TabIndex = 0;
            this.previewText.WordWrap = false;
            // 
            // formatList
            // 
            this.formatList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.formatList.DropDownWidth = 200;
            this.formatList.FormattingEnabled = true;
            this.formatList.Location = new System.Drawing.Point(391, 21);
            this.formatList.Name = "formatList";
            this.formatList.Size = new System.Drawing.Size(119, 21);
            this.formatList.TabIndex = 3;
            this.formatList.SelectedIndexChanged += new System.EventHandler(this.formatList_SelectedIndexChanged);
            this.formatList.TextChanged += new System.EventHandler(this.formatList_TextChanged);
            // 
            // tokensGroupBox
            // 
            this.tokensGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tokensGroupBox.Controls.Add(this.tokenSampleValueTextBox);
            this.tokensGroupBox.Controls.Add(this.tokenSampleValueLabel);
            this.tokensGroupBox.Controls.Add(this.tokenList);
            this.tokensGroupBox.Controls.Add(this.formatList);
            this.tokensGroupBox.Controls.Add(this.tokenLabel);
            this.tokensGroupBox.Controls.Add(this.formatLabel);
            this.tokensGroupBox.Controls.Add(this.insertButton);
            this.tokensGroupBox.Location = new System.Drawing.Point(15, 12);
            this.tokensGroupBox.Name = "tokensGroupBox";
            this.tokensGroupBox.Size = new System.Drawing.Size(597, 78);
            this.tokensGroupBox.TabIndex = 0;
            this.tokensGroupBox.TabStop = false;
            this.tokensGroupBox.Text = "Tokens";
            // 
            // tokenSampleValueTextBox
            // 
            this.tokenSampleValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tokenSampleValueTextBox.Location = new System.Drawing.Point(60, 48);
            this.tokenSampleValueTextBox.Name = "tokenSampleValueTextBox";
            this.tokenSampleValueTextBox.ReadOnly = true;
            this.tokenSampleValueTextBox.Size = new System.Drawing.Size(450, 20);
            this.tokenSampleValueTextBox.TabIndex = 6;
            // 
            // tokenSampleValueLabel
            // 
            this.tokenSampleValueLabel.AutoSize = true;
            this.tokenSampleValueLabel.Location = new System.Drawing.Point(9, 51);
            this.tokenSampleValueLabel.Name = "tokenSampleValueLabel";
            this.tokenSampleValueLabel.Size = new System.Drawing.Size(45, 13);
            this.tokenSampleValueLabel.TabIndex = 5;
            this.tokenSampleValueLabel.Text = "Sample:";
            // 
            // TemplateEditorForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.tokensGroupBox);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Name = "TemplateEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Template Editor";
            this.splitter.Panel1.ResumeLayout(false);
            this.splitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
            this.splitter.ResumeLayout(false);
            this.templateBox.ResumeLayout(false);
            this.templateBox.PerformLayout();
            this.previewBox.ResumeLayout(false);
            this.previewBox.PerformLayout();
            this.tokensGroupBox.ResumeLayout(false);
            this.tokensGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox templateText;
        private System.Windows.Forms.ComboBox tokenList;
        private System.Windows.Forms.Label tokenLabel;
        private System.Windows.Forms.Label formatLabel;
        private System.Windows.Forms.Button insertButton;
        private System.Windows.Forms.SplitContainer splitter;
        private System.Windows.Forms.GroupBox templateBox;
        private System.Windows.Forms.GroupBox previewBox;
        private System.Windows.Forms.TextBox previewText;
        private System.Windows.Forms.ComboBox formatList;
        private System.Windows.Forms.GroupBox tokensGroupBox;
        private System.Windows.Forms.TextBox tokenSampleValueTextBox;
        private System.Windows.Forms.Label tokenSampleValueLabel;
    }
}