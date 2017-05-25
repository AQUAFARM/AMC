using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using JelleDruyts.Windows;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Models;

namespace Schedulr.ViewModels
{
    /// <summary>
    /// A list of <see cref="FlickrCollectionListItem"/> instances.
    /// </summary>
    public class FlickrCollectionList : ObservableObject, IDisposable
    {
        #region Fields

        private IList<FlickrCollectionListItem> userOrderedItems;
        private string descriptionPrefix;
        private AccountSettings settings;
        private ObservableProperty<PictureCollectionSortMode> sortModeProperty;

        #endregion

        #region Properties

        public ICommand SortByNameCommand { get; private set; }
        public ICommand SortByAgeCommand { get; private set; }
        public ICommand SortByUserOrderCommand { get; private set; }
        public ICommand SortBySizeCommand { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public ObservableCollection<FlickrCollectionListItem> Items
        {
            get { return this.GetValue(ItemsProperty); }
            set { this.SetValue(ItemsProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Items"/> observable property.
        /// </summary>
        public static ObservableProperty<ObservableCollection<FlickrCollectionListItem>> ItemsProperty = new ObservableProperty<ObservableCollection<FlickrCollectionListItem>, FlickrCollectionList>(o => o.Items);

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get { return this.GetValue(DescriptionProperty); }
            set { this.SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Description"/> observable property.
        /// </summary>
        public static ObservableProperty<string> DescriptionProperty = new ObservableProperty<string, FlickrCollectionList>(o => o.Description);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlickrCollectionList"/> class.
        /// </summary>
        /// <param name="items">The list items.</param>
        /// <param name="descriptionPrefix">The description prefix.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="sortModeProperty">The sort mode property.</param>
        public FlickrCollectionList(IList<FlickrCollectionListItem> items, string descriptionPrefix, AccountSettings settings, ObservableProperty<PictureCollectionSortMode> sortModeProperty)
        {
            this.userOrderedItems = items;
            this.SortByNameCommand = new RelayCommand(SortByName);
            this.SortByAgeCommand = new RelayCommand(SortByAge);
            this.SortByUserOrderCommand = new RelayCommand(SortByUserOrder);
            this.SortBySizeCommand = new RelayCommand(SortBySize);
            this.descriptionPrefix = descriptionPrefix;
            this.settings = settings;
            this.sortModeProperty = sortModeProperty;
            SetItems(GetCurrentSortMode());
            foreach (var item in this.Items)
            {
                item.PropertyChanged += OnItemPropertyChanged;
            }
            SetDescription();
        }

        #endregion

        #region Helper Methods

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FlickrCollectionListItem.IsSelectedProperty.Name)
            {
                // When the IsSelected property of a list item changed, update the description here.
                SetDescription();
            }
        }

        /// <summary>
        /// Sets the description.
        /// </summary>
        private void SetDescription()
        {
            var selectedItemCount = this.Items.Count(i => i.IsSelected == true);
            var partiallySelectedItemCount = this.Items.Count(i => i.IsSelected == null);
            var partiallySelectedMessage = string.Empty;
            if (partiallySelectedItemCount > 0)
            {
                partiallySelectedMessage = string.Format(CultureInfo.CurrentCulture, " by all, {0} selected by some", partiallySelectedItemCount);
            }
            this.Description = string.Format(CultureInfo.CurrentCulture, "{0} ({1} selected{2})", this.descriptionPrefix, selectedItemCount, partiallySelectedMessage);
        }

        private void SetItems(PictureCollectionSortMode sortMode)
        {
            // Remember the new value.
            this.settings.SetValue<PictureCollectionSortMode>(this.sortModeProperty, sortMode);

            // Apply the new value.
            if (this.userOrderedItems == null)
            {
                this.Items = new ObservableCollection<FlickrCollectionListItem>();
            }
            else
            {
                IEnumerable<FlickrCollectionListItem> sortedItems;
                switch (sortMode)
                {
                    case PictureCollectionSortMode.UserOrderAscending:
                        sortedItems = this.userOrderedItems;
                        break;
                    case PictureCollectionSortMode.UserOrderDescending:
                        sortedItems = this.userOrderedItems.Reverse();
                        break;
                    case PictureCollectionSortMode.NameAscending:
                        sortedItems = this.userOrderedItems.OrderBy(i => i.Collection.Name);
                        break;
                    case PictureCollectionSortMode.NameDescending:
                        sortedItems = this.userOrderedItems.OrderByDescending(i => i.Collection.Name);
                        break;
                    case PictureCollectionSortMode.AgeAscending:
                        sortedItems = this.userOrderedItems.OrderBy(i => i.Collection.Id);
                        break;
                    case PictureCollectionSortMode.AgeDescending:
                        sortedItems = this.userOrderedItems.OrderByDescending(i => i.Collection.Id);
                        break;
                    case PictureCollectionSortMode.SizeAscending:
                        sortedItems = this.userOrderedItems.OrderBy(i => i.Collection.Size);
                        break;
                    case PictureCollectionSortMode.SizeDescending:
                        sortedItems = this.userOrderedItems.OrderByDescending(i => i.Collection.Size);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("The enum value is not a valid sort mode: " + sortMode);
                }
                this.Items = new ObservableCollection<FlickrCollectionListItem>(sortedItems);
            }
        }

        private PictureCollectionSortMode GetCurrentSortMode()
        {
            return this.settings.GetValue<PictureCollectionSortMode>(this.sortModeProperty);
        }

        #endregion

        #region Commands

        private void SortByName(object parameter)
        {
            var sortMode = GetCurrentSortMode();
            if (sortMode == PictureCollectionSortMode.NameAscending)
            {
                sortMode = PictureCollectionSortMode.NameDescending;
            }
            else
            {
                sortMode = PictureCollectionSortMode.NameAscending;
            }
            SetItems(sortMode);
        }

        private void SortByAge(object parameter)
        {
            var sortMode = GetCurrentSortMode();
            if (sortMode == PictureCollectionSortMode.AgeDescending)
            {
                sortMode = PictureCollectionSortMode.AgeAscending;
            }
            else
            {
                sortMode = PictureCollectionSortMode.AgeDescending;
            }
            SetItems(sortMode);
        }

        private void SortByUserOrder(object parameter)
        {
            var sortMode = GetCurrentSortMode();
            if (sortMode == PictureCollectionSortMode.UserOrderAscending)
            {
                sortMode = PictureCollectionSortMode.UserOrderDescending;
            }
            else
            {
                sortMode = PictureCollectionSortMode.UserOrderAscending;
            }
            SetItems(sortMode);
        }

        private void SortBySize(object parameter)
        {
            var sortMode = GetCurrentSortMode();
            if (sortMode == PictureCollectionSortMode.SizeDescending)
            {
                sortMode = PictureCollectionSortMode.SizeAscending;
            }
            else
            {
                sortMode = PictureCollectionSortMode.SizeDescending;
            }
            SetItems(sortMode);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            foreach (var item in this.Items)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }
        }

        #endregion
    }
}