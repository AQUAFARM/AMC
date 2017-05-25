using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace JelleDruyts.Windows.Collections
{
    /// <summary>
    /// Synchronizes changes between two collections.
    /// </summary>
    /// <typeparam name="TCollection1">The type of the first collection.</typeparam>
    /// <typeparam name="TCollectionItemType1">The type of items in the first collection.</typeparam>
    /// <typeparam name="TCollection2">The type of the second collection.</typeparam>
    /// <typeparam name="TCollectionItemType2">The type of items in the second collection.</typeparam>
    public class CollectionSynchronizer<TCollection1, TCollectionItemType1, TCollection2, TCollectionItemType2> : IDisposable
        where TCollection1 : INotifyCollectionChanged, IList, IList<TCollectionItemType1>
        where TCollection2 : INotifyCollectionChanged, IList, IList<TCollectionItemType2>
    {
        #region Fields

        private TCollection1 collection1;
        private Converter<TCollectionItemType1, TCollectionItemType2> collection1ItemConverter;
        private TCollection2 collection2;
        private Converter<TCollectionItemType2, TCollectionItemType1> collection2ItemConverter;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionSynchronizer&lt;TCollection1, TCollectionItemType1, TCollection2, TCollectionItemType2&gt;"/> class.
        /// </summary>
        /// <param name="collection1">The first collection.</param>
        /// <param name="collection1ItemConverter">Converts items from the first collection's item type to the second collection's item type.</param>
        /// <param name="collection2">The second collection.</param>
        /// <param name="collection2ItemConverter">Converts items from the first collection's item type to the second collection's item type.</param>
        public CollectionSynchronizer(TCollection1 collection1, Converter<TCollectionItemType1, TCollectionItemType2> collection1ItemConverter, TCollection2 collection2, Converter<TCollectionItemType2, TCollectionItemType1> collection2ItemConverter)
        {
            if (collection1 == null)
            {
                throw new ArgumentNullException("collection1");
            }
            if (collection1ItemConverter == null)
            {
                throw new ArgumentNullException("collection1ItemConverter");
            }
            if (collection2 == null)
            {
                throw new ArgumentNullException("collection2");
            }
            if (collection2ItemConverter == null)
            {
                throw new ArgumentNullException("collection2ItemConverter");
            }

            this.collection1 = collection1;
            this.collection1ItemConverter = collection1ItemConverter;
            this.collection2 = collection2;
            this.collection2ItemConverter = collection2ItemConverter;
            this.collection1.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
            this.collection2.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        #endregion

        #region Helper Methods

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Stop listening for changes while updating the collections.
            this.collection1.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
            this.collection2.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);

            try
            {
                // Determine the source and target collections.
                IList source = this.collection1;
                IList target = this.collection2;
                Func<object, object> targetItemCreator = o => { return this.collection1ItemConverter((TCollectionItemType1)o); };
                if (object.Equals(sender, this.collection2))
                {
                    source = this.collection2;
                    target = this.collection1;
                    targetItemCreator = o => { return this.collection2ItemConverter((TCollectionItemType2)o); };
                }

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        PerformAdd(target, e.NewItems, e.NewStartingIndex, targetItemCreator);
                        break;

                    case NotifyCollectionChangedAction.Move:
                        var itemsToMove = new ArrayList();
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            itemsToMove.Add(target[e.OldStartingIndex]);
                            target.RemoveAt(e.OldStartingIndex);
                        }
                        for (var i = 0; i < itemsToMove.Count; i++)
                        {
                            target.Insert(e.NewStartingIndex + i, itemsToMove[i]);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        PerformRemove(target, e.OldItems, e.OldStartingIndex);
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        PerformRemove(target, e.OldItems, e.OldStartingIndex);
                        PerformAdd(target, e.NewItems, e.NewStartingIndex, targetItemCreator);
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        target.Clear();
                        for (int i = 0; i < source.Count; i++)
                        {
                            target.Add(targetItemCreator(source[i]));
                        }
                        break;
                }
            }
            finally
            {
                this.collection1.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
                this.collection2.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
            }
        }

        private static void PerformRemove(IList target, IList oldItems, int oldStartingIndex)
        {
            for (int i = 0; i < oldItems.Count; i++)
            {
                target.RemoveAt(oldStartingIndex);
            }
        }

        private static void PerformAdd(IList target, IList newItems, int newStartingIndex, Func<object, object> targetItemCreator)
        {
            for (int i = 0; i < newItems.Count; i++)
            {
                target.Insert(newStartingIndex + i, targetItemCreator(newItems[i]));
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Stop listening for changes.
                this.collection1.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
                this.collection2.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
            }
        }

        #endregion
    }
}