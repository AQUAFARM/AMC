using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;

namespace Schedulr
{
   /// <summary>
   /// Bulk add and remove ObservableCollection
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class BulkObservableCollection<T> : ObservableCollection<T>
   {
      #region Private member variables

      private bool _isNotificationSuspended;

      #endregion

      #region Constructors

      /// <summary>
      /// 
      /// </summary>
      public BulkObservableCollection()
      {
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="collection"></param>
      public BulkObservableCollection(IEnumerable<T> collection)
      {
         AddRange(collection);
      }

      #endregion

      #region Overrides of ObservableCollection<T>

      /// <summary>
      /// Raises the <see cref="E:CollectionChanged"/> event.
      /// </summary>
      /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
      protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
      {
         if (!_isNotificationSuspended)
         {
            base.OnCollectionChanged(e);
         }
      }

      /// <summary>
      /// Raises the <see cref="E:PropertyChanged"/> event.
      /// </summary>
      /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
      protected override void OnPropertyChanged(PropertyChangedEventArgs e)
      {
         if (!_isNotificationSuspended)
         {
            base.OnPropertyChanged(e);
         }
      }

      #endregion

      #region Public methods on ObservableCollection<T>

      /// <summary>
      /// Adds the elements of the specified collection to the end of the <see cref="System.Collections.ObjectModel.ObservableCollection&lt;T&gt;"/>.
      /// </summary>
      /// <param name="collection">The collection.</param>
      /// <param name="clearExistingData">if set to <c>true</c> [clear existing data].</param>
      public void AddRange(IEnumerable<T> collection, bool clearExistingData)
      {
         if (clearExistingData)
         {
            _isNotificationSuspended = true;
            Clear();
         }
         AddRange(collection);
      }

      /// <summary>
      /// Adds the elements of the specified collection to the end of the <see cref="System.Collections.ObjectModel.ObservableCollection&lt;T&gt;"/>.
      /// </summary>
      /// <param name="collection">The collection.</param>
      public void AddRange(IEnumerable<T> collection)
      {
         if (collection == null)
         {
            throw new ArgumentNullException("collection");
         }

         _isNotificationSuspended = true;
         int startIndex = Count;

         try
         {
            IList<T> items = base.Items;

            if (items != null)
            {
               using (IEnumerator<T> enumerator = collection.GetEnumerator())
               {
                  while (enumerator.MoveNext())
                  {
                     items.Add(enumerator.Current);
                  }
               }
            }
         }
         finally
         {
            _isNotificationSuspended = false;

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)); 
         }
      }

      /// <summary>
      /// Removes a range of elements specified in the collection from the <see cref="System.Collections.ObjectModel.ObservableCollection&lt;T&gt;"/>.
      /// </summary>
      /// <param name="collection">The collection.</param>
      public void RemoveRange(IEnumerable<T> collection)
      {
         if (collection == null)
         {
            throw new ArgumentNullException("collection");
         }

         _isNotificationSuspended = true;

         try
         {
            RemoveItemsFromCollection(collection);
         }
         finally
         {
            _isNotificationSuspended = false;

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
         }
      }

       /// <summary>
       /// Inserts the elements of the specified collection to the specified index of the <see cref="System.Collections.ObjectModel.ObservableCollection&lt;T&gt;"/>.
       /// </summary>
       /// <param name="index">The index</param>
       /// <param name="collection">The collection.</param>
       public void InsertRange(int index, IEnumerable<T> collection)
      {
          if (collection == null)
          {
              throw new ArgumentNullException("collection");
          }
           if (index < 0 || index > this.Count)
           {
               throw new IndexOutOfRangeException();
           }

          _isNotificationSuspended = true;
          int startIndex = index;

          try
          {
              IList<T> items = base.Items;

              if (items != null)
              {
                  using (IEnumerator<T> enumerator = collection.GetEnumerator())
                  {
                      while (enumerator.MoveNext())
                      {
                          items.Insert(startIndex,enumerator.Current);
                          startIndex++;
                      }
                  }
              }
          }
          finally
          {
              _isNotificationSuspended = false;

              OnPropertyChanged(new PropertyChangedEventArgs("Count"));
              OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
              OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
          }
      }

      private void RemoveItemsFromCollection(IEnumerable<T> collection)
      {
         IList<T> items = base.Items;
         if (items != null)
         {
            using (IEnumerator<T> enumerator = new List<T>(collection).GetEnumerator())
            {
               while (enumerator.MoveNext())
               {
                  items.Remove(enumerator.Current);
               }
            }
         }
      }

      /// <summary>
      /// Creates a shallow copy of a range of elements in the source List(Of T).
      /// </summary>
      /// <param name="index">The index.</param>
      /// <param name="count">The count.</param>
      /// <returns></returns>
      public List<T> GetRange(int index, int count)
      {
         if (index < 0)
         {
            throw new ArgumentOutOfRangeException("index");
         }
         if (count < 0)
         {
            throw new ArgumentOutOfRangeException("index");
         }
         if ((Count - index) < count)
         {
            throw new ArgumentException("Invalid Offset Length");
         }

         return new List<T>(Items.Skip(index).Take(count));
      }

      /// <summary>
      /// Suspends the notifications.
      /// </summary>
      public void SuspendNotifications()
      {
         _isNotificationSuspended = true;
      }

      /// <summary>
      /// Resumes the notifications.
      /// </summary>
      public void ResumeNotifications()
      {
         _isNotificationSuspended = false;
      }

      #endregion
   }
}