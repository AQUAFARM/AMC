using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JelleDruyts.Windows
{
    /// <summary>
    /// Provides extension methods for collection types.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Replaces all items in a collection with new items.
        /// </summary>
        /// <param name="collection">The collection to remove all items from and replace with the new items.</param>
        /// <param name="newItems">The new items to add to the collection.</param>
        public static void ReplaceItems(this IList collection, IEnumerable newItems)
        {
            if (collection != null)
            {
                collection.Clear();
                if (newItems != null)
                {
                    foreach (var item in newItems)
                    {
                        collection.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Sorts the items in a collection according to a specified comparison.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="collection">The collection to sort.</param>
        /// <param name="comparison">The method to compare the items in the collection.</param>
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var comparer = new Comparer<T>(comparison);
            List<T> sorted = collection.OrderBy(x => x, comparer).ToList();
            for (var i = 0; i < sorted.Count; i++)
            {
                collection.Move(collection.IndexOf(sorted[i]), i);
            }
        }

        private class Comparer<T> : IComparer<T>
        {
            private readonly Comparison<T> comparison;

            public Comparer(Comparison<T> comparison)
            {
                this.comparison = comparison;
            }

            public int Compare(T x, T y)
            {
                return comparison.Invoke(x, y);
            }
        }

    }
}