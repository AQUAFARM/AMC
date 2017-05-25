using System;
using System.Diagnostics;
using System.Windows;

namespace Schedulr.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AuthenticationDialog.xaml
    /// </summary>
    public partial class AuthenticationDialog : Window
    {
        /// <summary>
        /// The Flickr authentication URL.
        /// </summary>
        private string authenticationUrl;

        public string VerificationCode;
        
        public AuthenticationDialog(string authenticationUrl)
        {
            InitializeComponent();
            this.Owner = App.Instance.MainWindow;
            this.authenticationUrl = authenticationUrl;
            this.authenticationWebBrowser.Navigate(this.authenticationUrl);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(VerifierTextBox.Text))
            {
                MessageBox.Show("You must paste the verifier code into the textbox.");
                return;
            }
            this.DialogResult = true;
            this.VerificationCode = VerifierTextBox.Text.Trim();
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void authenticationHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(this.authenticationUrl);
        }
    }
}