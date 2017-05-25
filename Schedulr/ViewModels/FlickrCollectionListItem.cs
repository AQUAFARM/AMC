using System.Collections.ObjectModel;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Models;

namespace Schedulr.ViewModels
{
    /// <summary>
    /// A Flickr collection that updates the collection ID's of one or more pictures when it is selected or deselected.
    /// </summary>
    public class FlickrCollectionListItem : ObservableObject
    {
        #region Fields

        /// <summary>
        /// The collection from the picture containing all selected collection ID's.
        /// </summary>
        private ObservableCollection<string> pictureCollection;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the picture collection.
        /// </summary>
        public PictureCollection Collection { get; private set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets a value that determines if this item is selected.
        /// </summary>
        public bool? IsSelected
        {
            get { return this.GetValue(IsSelectedProperty); }
            set { this.SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="IsSelected"/> observable property.
        /// </summary>
        public static ObservableProperty<bool?> IsSelectedProperty = new ObservableProperty<bool?, FlickrCollectionListItem>(o => o.IsSelected, OnIsSelectedChanged);

        /// <summary>
        /// Gets or sets the placeholder image to show while loading the actual image.
        /// </summary>
        public string PlaceholderImage
        {
            get { return this.GetValue(PlaceholderImageProperty); }
            set { this.SetValue(PlaceholderImageProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PlaceholderImage"/> observable property.
        /// </summary>
        public static ObservableProperty<string> PlaceholderImageProperty = new ObservableProperty<string, FlickrCollectionListItem>(o => o.PlaceholderImage);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlickrCollectionListItem"/> class.
        /// </summary>
        /// <param name="collection">The picture collection.</param>
        /// <param name="pictureCollection">The collection from the picture containing all collection ID's.</param>
        /// <param name="isSelected">A value that determines if this item is selected.</param>
        /// <param name="placeholderImage">The placeholder image to show while loading the actual image.</param>
        public FlickrCollectionListItem(PictureCollection collection, ObservableCollection<string> pictureCollection, bool? isSelected, string placeholderImage)
        {
            this.pictureCollection = pictureCollection;
            this.Collection = collection;
            this.IsSelected = isSelected;
            this.PlaceholderImage = placeholderImage;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Called when the <see cref="IsSelected"/> property has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ObservablePropertyChangedEventArgs{TProperty}"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(ObservableObject sender, ObservablePropertyChangedEventArgs<bool?> e)
        {
            var item = (FlickrCollectionListItem)sender;
            if (e.NewValue.HasValue)
            {
                if (e.NewValue.Value)
                {
                    // The collection item has been selected, add the collection's ID to the picture.
                    if (!item.pictureCollection.Contains(item.Collection.Id))
                    {
                        item.pictureCollection.Add(item.Collection.Id);
                    }
                }
                else
                {
                    // The collection item has been deselected, remove the collection's ID from the picture.
                    if (item.pictureCollection.Contains(item.Collection.Id))
                    {
                        item.pictureCollection.Remove(item.Collection.Id);
                    }
                }
            }
        }

        #endregion
    }
}