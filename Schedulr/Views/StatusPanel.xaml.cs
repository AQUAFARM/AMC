using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using JelleDruyts.Windows.DragDrop;

namespace Schedulr.Views
{
    /// <summary>
    /// Interaction logic for StatusPanel.xaml
    /// </summary>
    public partial class StatusPanel : UserControl
    {
        public StatusPanel()
        {
            InitializeComponent();
        }

        private void statusHistoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        private void OnPopupDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var popupRoot = thumb.GetVisualAncestor<Border>();
            var yAdjust = popupRoot.ActualHeight + e.VerticalChange;
            var xAdjust = popupRoot.ActualWidth + e.HorizontalChange;
            if ((xAdjust >= 0) && (yAdjust >= 0))
            {
                popupRoot.Width = xAdjust;
                popupRoot.Height = yAdjust;
            }
        }
    }
}