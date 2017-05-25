using System.Windows;
using System.Windows.Controls;
using Schedulr.Extensibility;
using Schedulr.ViewModels;

namespace Schedulr.Views.Controls
{
    /// <summary>
    /// Interaction logic for PluginPicker.xaml
    /// </summary>
    public partial class PluginPicker : UserControl
    {
        public PluginCategory? Category
        {
            get { return (PluginCategory?)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }

        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof(PluginCategory?), typeof(PluginPicker), new UIPropertyMetadata(null, OnCategoryChanged));

        public PluginPicker()
        {
            InitializeComponent();
        }

        private static void OnCategoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = ((PluginPicker)sender);
            if (picker.Category.HasValue)
            {
                picker.DataContext = new PluginPickerViewModel(picker.Category.Value);
            }
            else
            {
                picker.DataContext = null;
            }
        }
    }
}