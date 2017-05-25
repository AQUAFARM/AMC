using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace JelleDruyts.Windows.Controls
{
    /// <summary>
    /// A <see cref="Hyperlink"/> that lets the operating system open the URI (typically an HTTP hyperlink, but anything the OS can handle).
    /// </summary>
    public class ExternalHyperlink : Hyperlink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalHyperlink"/> class.
        /// </summary>
        public ExternalHyperlink()
        {
            this.RequestNavigate += new RequestNavigateEventHandler(OnRequestNavigate);
        }

        /// <summary>
        /// Called when a request for navigation is made.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RequestNavigateEventArgs"/> instance containing the event data.</param>
        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (e.Uri != null)
            {
                Process.Start(e.Uri.ToString());
            }
        }
    }
}