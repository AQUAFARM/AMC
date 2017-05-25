using System.Windows;
using System.Windows.Controls;

namespace Schedulr.Views
{
    /// <summary>
    /// Interaction logic for UploadedPicturesEditor.xaml
    /// </summary>
    public partial class UploadedPicturesEditor : UserControl
    {
        public UploadedPicturesEditor()
        {
            InitializeComponent();
        }

        #region Details Panel Visibility Handling

        // The code below is used to properly restore the details panel width when it has been resized by a grid splitter.
        // See http://social.msdn.microsoft.com/forums/en-US/wpf/thread/02c15062-e1de-4c54-a17a-7033ad58faf2 for details.

        private GridLength detailsColumnWidth = new GridLength(1, GridUnitType.Star);

        public bool DetailsPanelVisible
        {
            get { return (bool)GetValue(DetailsPanelVisibleProperty); }
            set { SetValue(DetailsPanelVisibleProperty, value); }
        }

        public static readonly DependencyProperty DetailsPanelVisibleProperty = DependencyProperty.Register("DetailsPanelVisible", typeof(bool), typeof(UploadedPicturesEditor), new UIPropertyMetadata(true, OnDetailsPanelVisibleChanged));

        private static void OnDetailsPanelVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var editor = (UploadedPicturesEditor)d;
            if (!editor.DetailsPanelVisible)
            {
                if (editor.detailsColumn.ActualWidth > 0)
                {
                    editor.detailsColumnWidth = editor.detailsColumn.Width;
                }
                editor.detailsColumn.Width = new GridLength(25);
            }
            else
            {
                editor.detailsColumn.Width = editor.detailsColumnWidth;
            }
        }

        #endregion
    }
}