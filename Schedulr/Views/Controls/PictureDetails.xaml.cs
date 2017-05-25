using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace Schedulr.Views.Controls
{
    /// <summary>
    /// Interaction logic for PictureDetails.xaml
    /// </summary>
    public partial class PictureDetails : UserControl
    {
        public PictureDetails()
        {
            InitializeComponent();
        }

        private void fileNameTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var fileName = ((TextBox)sender).Text;
            if (File.Exists(fileName))
            {
                try
                {
                    e.Handled = true;
                    Process.Start(fileName);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}