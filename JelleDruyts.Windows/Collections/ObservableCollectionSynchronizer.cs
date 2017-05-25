using System;
using System.Collections.ObjectModel;

namespace JelleDruyts.Windows.Collections
{
    /// <summary>
    /// Synchronizes changes between two observable collections.
    /// </summary>
    /// <typeparam name="TCollectionItemType1">The type of items in the first collection.</typeparam>
    /// <typeparam name="TCollectionItemType2">The type of items in the second collection.</typeparam>
    public class ObservableCollectionSynchronizer<TCollectionItemType1, TCollectionItemType2> : CollectionSynchronizer<ObservableCollection<TCollectionItemType1>, TCollectionItemType1, ObservableCollection<TCollectionItemType2>, TCollectionItemType2>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionSynchronizer&lt;TCollectionItemType1, TCollectionItemType2&gt;"/> class.
        /// </summary>
        /// <param name="collection1">The first collection.</param>
        /// <param name="collection1ItemConverter">Converts items from the first collection's item type to the second collection's item type.</param>
        /// <param name="collection2">The second collection.</param>
        /// <param name="collection2ItemConverter">Converts items from the first collection's item type to the second collection's item type.</param>
        public ObservableCollectionSynchronizer(ObservableCollection<TCollectionItemType1> collection1, Converter<TCollectionItemType1, TCollectionItemType2> collection1ItemConverter, ObservableCollection<TCollectionItemType2> collection2, Converter<TCollectionItemType2, TCollectionItemType1> collection2ItemConverter)
            : base(collection1, collection1ItemConverter, collection2, collection2ItemConverter)
        {
        }

        #endregion
    }
}