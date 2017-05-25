using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using JelleDruyts.Windows.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JelleDruyts.Windows.Test
{
    [TestClass]
    public class CollectionSynchronizerTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsOnNullCollection1()
        {
            new CollectionSynchronizer<ObservableCollection<string>, string, ObservableCollection<CultureInfo>, CultureInfo>(null, n => new CultureInfo(n), GetCultures(), c => c.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsOnNullCollection1ItemConverter()
        {
            new CollectionSynchronizer<ObservableCollection<string>, string, ObservableCollection<CultureInfo>, CultureInfo>(GetCultureNames(), null, GetCultures(), c => c.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsOnNullCollection2()
        {
            new CollectionSynchronizer<ObservableCollection<string>, string, ObservableCollection<CultureInfo>, CultureInfo>(GetCultureNames(), n => new CultureInfo(n), null, c => c.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsOnNullCollection2ItemConverter()
        {
            new CollectionSynchronizer<ObservableCollection<string>, string, ObservableCollection<CultureInfo>, CultureInfo>(GetCultureNames(), n => new CultureInfo(n), GetCultures(), null);
        }

        [TestMethod]
        public void CollectionsAreSynchronized()
        {
            var cultureNames = GetCultureNames();
            var cultures = GetCultures();
            VerifyCollections(cultureNames, cultures);

            var sut = new ObservableCollectionSynchronizer<string, CultureInfo>(cultureNames, n => new CultureInfo(n), cultures, c => c.Name);

            // Add to first collection.
            cultureNames.Add("fr-FR");
            VerifyCollections(cultureNames, cultures);

            // Add to second collection.
            var germanCulture = new CultureInfo("de-DE");
            cultures.Add(germanCulture);
            VerifyCollections(cultureNames, cultures);

            // Remove from first collection.
            cultureNames.Remove("fr-FR");
            VerifyCollections(cultureNames, cultures);

            // Remove from second collection.
            cultures.Remove(germanCulture);
            VerifyCollections(cultureNames, cultures);

            // Replace in first collection.
            cultureNames.Replace(0, "nl-NL");
            VerifyCollections(cultureNames, cultures);

            // Replace in second collection.
            cultures.Replace(1, new CultureInfo("en-GB"));
            VerifyCollections(cultureNames, cultures);

            // Move in first collection.
            cultureNames.Move(1, 0);
            VerifyCollections(cultureNames, cultures);

            // Move in second collection.
            cultures.Move(1, 0);
            VerifyCollections(cultureNames, cultures);

            // Reset first collection.
            cultureNames.RequestReset();
            VerifyCollections(cultureNames, cultures);

            // Reset second collection.
            cultures.RequestReset();
            VerifyCollections(cultureNames, cultures);

            // Dispose the synchronizer so no changes are synchronized anymore.
            sut.Dispose();

            cultureNames.Add("it-IT");
            Assert.AreEqual<int>(cultureNames.Count - 1, cultures.Count);
            cultureNames.Remove("it-IT");

            cultures.Add(new CultureInfo("it-IT"));
            Assert.AreEqual<int>(cultureNames.Count + 1, cultures.Count);
        }

        private static void VerifyCollections(ObservableCollection<string> cultureNames, ObservableCollection<CultureInfo> cultures)
        {
            Assert.IsNotNull(cultureNames);
            Assert.IsNotNull(cultures);
            Assert.AreEqual<int>(cultureNames.Count, cultures.Count);

            for (var i = 0; i < cultureNames.Count; i++)
            {
                var cultureName = cultureNames[i];
                var culture = cultures[i];
                Assert.IsNotNull(cultureName);
                Assert.IsNotNull(culture);
                Assert.AreEqual<string>(cultureName, culture.Name);
            }
        }

        private static MyObservableCollection<string> GetCultureNames()
        {
            return new MyObservableCollection<string>
            {
                "en-US",
                "nl-BE"
            };
        }

        private static MyObservableCollection<CultureInfo> GetCultures()
        {
            return new MyObservableCollection<CultureInfo>(GetCultureNames().Select(n => new CultureInfo(n)));
        }

        private class MyObservableCollection<T> : ObservableCollection<T>
        {
            public MyObservableCollection()
                : base()
            {
            }

            public MyObservableCollection(IEnumerable<T> collection)
                : base(collection)
            {
            }

            public void Replace(int index, T item)
            {
                base.SetItem(index, item);
            }

            public void RequestReset()
            {
                base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
    }
}