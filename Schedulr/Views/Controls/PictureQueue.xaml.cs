using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using JelleDruyts.Windows;
using Schedulr.Messages;
using Schedulr.Models;
using Schedulr.ViewModels;

namespace Schedulr.Views.Controls
{
    /// <summary>
    /// Interaction logic for PictureQueue.xaml
    /// </summary>
    public partial class PictureQueue : UserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureQueue"/> class.
        /// </summary>
        public PictureQueue()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Deselect Items When Clicking Outside An Item

        /// <summary>
        /// Handles the MouseDown event of the picturesListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void picturesListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Deselect all items when the user clicks outside an item.
            var listbox = (ListBox)sender;
            // First set the focus to the control so that any data bindings are committed
            // on the currently selected item(s).

            listbox.Focus();
            listbox.SelectedItem = null;
        }

        #endregion

        #region Stretch Expander Header

        private void StretchedHeaderTemplate_Loaded(object sender, RoutedEventArgs e)
        {
            // This is a simple solution to stretch the content in an expander header.
            // See http://joshsmithonwpf.wordpress.com/2007/02/24/stretching-content-in-an-expander-header/.
            FrameworkElement rootElem = sender as FrameworkElement;
            ContentPresenter contentPres = rootElem.TemplatedParent as ContentPresenter;
            contentPres.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        #endregion

        #region Picture Queue Selection Changed

        private void picturesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Cheat the MVVM pattern a bit. The problem is that we can't data bind
            // the ViewModel to the ListBox.SelectedItems property since it is a read-only
            // property. So we can't automatically keep the ViewModel in sync with the selected
            // pictures. A possible solution is available for this problem at
            // http://blog.functionalfun.net/2009/02/how-to-databind-to-selecteditems.html
            // but in this case it's a lot easier to just send a message from the view with
            // the selected pictures.
            ListBox listbox = (ListBox)sender;
            // First set the focus to the control so that any data bindings are committed
            // on the currently selected item(s).
            listbox.Focus();
            IList<Picture> selectedPictures = listbox.SelectedItems.Cast<PictureViewModel>().Select(p => p.Picture).ToList();
            PictureQueueViewModel pictureQueue = (PictureQueueViewModel)this.DataContext;
            Messenger.Send<PictureQueueSelectionChangedMessage>(new PictureQueueSelectionChangedMessage(selectedPictures, pictureQueue));
        }

        #endregion

        #region Picture Queue Group Style Selection

        private GroupStyle SelectGroupStyle(CollectionViewGroup group, int level)
        {
            var displayMode = AccountSettings.PictureQueueDisplayModeProperty.DefaultValue;
            if (this.DataContext != null)
            {
                PictureQueueViewModel pictureQueue = (PictureQueueViewModel)this.DataContext;
                pictureQueue.PropertyChanged -= new PropertyChangedEventHandler(pictureQueueViewModelPropertyChanged);
                pictureQueue.PropertyChanged += new PropertyChangedEventHandler(pictureQueueViewModelPropertyChanged);
                pictureQueue.Account.Settings.PropertyChanged -= new PropertyChangedEventHandler(accountSettingsPictureQueueDisplayModePropertyPropertyChanged);
                pictureQueue.Account.Settings.PropertyChanged += new PropertyChangedEventHandler(accountSettingsPictureQueueDisplayModePropertyPropertyChanged);
                displayMode = pictureQueue.Account.Settings.PictureQueueDisplayMode;
            }
            return (GroupStyle)FindResource("PictureQueueGroupStyle" + displayMode.ToString());
        }

        private void pictureQueueViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PictureQueueViewModel.AccountProperty.Name)
            {
                UpdateGroupStyle();
            }
        }

        private void accountSettingsPictureQueueDisplayModePropertyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == AccountSettings.PictureQueueDisplayModeProperty.Name)
            {
                UpdateGroupStyle();
            }
        }

        private void UpdateGroupStyle()
        {
            // If the display mode changes, force the GroupStyleSelector to be re-evaluated by re-assigning it.
            this.picturesListBox.GroupStyleSelector = SelectGroupStyle;
        }

        #endregion
    }
}