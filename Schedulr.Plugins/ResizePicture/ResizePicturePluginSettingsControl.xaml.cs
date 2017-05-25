using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Schedulr.Plugins.ResizePicture
{
    /// <summary>
    /// Interaction logic for ResizePicturePluginSettingsControl.xaml
    /// </summary>
    public partial class ResizePicturePluginSettingsControl : UserControl
    {
        private ResizePicturePluginSettings settings;

        public ResizePicturePluginSettingsControl(ResizePicturePluginSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
            this.DataContext = this.settings;
            var customSizeItem = new SizeComboBoxItem(0, "Custom size", true);
            var items = new List<SizeComboBoxItem>
            {
                new SizeComboBoxItem(800, "800 pixels (suitable for on-screen viewing)"),
                new SizeComboBoxItem(1280, "1280 pixels (suitable for small prints)"),
                new SizeComboBoxItem(1600, "1600 pixels (suitable for larger prints)"),
                new SizeComboBoxItem(2048, "2048 pixels (best for archiving)"),
                customSizeItem
            };
            this.longestSideComboBox.ItemsSource = items;
            this.longestSideComboBox.SelectedItem = items.Where(i => i.Size == this.settings.LongestSide).FirstOrDefault() ?? customSizeItem;
            this.longestSideComboBox.SelectionChanged += longestSideComboBox_SelectionChanged;
        }

        private void longestSideComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (SizeComboBoxItem)this.longestSideComboBox.SelectedItem;
            if (!selectedItem.IsCustom)
            {
                this.settings.LongestSide = selectedItem.Size;
            }
        }

        private class SizeComboBoxItem
        {
            public int Size { get; private set; }
            public string Description { get; private set; }
            public bool IsCustom { get; private set; }

            public SizeComboBoxItem(int size, string description)
                : this(size, description, false)
            {
            }

            public SizeComboBoxItem(int size, string description, bool isCustom)
            {
                this.Size = size;
                this.Description = description;
                this.IsCustom = isCustom;
            }

            public override string ToString()
            {
                return this.Description;
            }
        }
    }
}