using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schedulr.Models;
using Schedulr.Providers;
using Schedulr.ViewModels;

namespace Schedulr.Test
{
    [TestClass]
    public class AggregatePictureTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsOnNullPictures()
        {
            var sut = new AggregatePicture(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorThrowsOnZeroPictures()
        {
            var sut = new AggregatePicture(GetTestPictures(0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorThrowsOnOnePicture()
        {
            var sut = new AggregatePicture(GetTestPictures(1));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetShortFileNameThrows()
        {
            var sut = new AggregatePicture(GetTestPictures(2));
            var shortFileName = sut.ShortFileName;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetFileNameThrows()
        {
            var sut = new AggregatePicture(GetTestPictures(2));
            sut.FileName = "dummy";
        }

        [TestMethod]
        public void AggregatePictureShouldAggregateProperties()
        {
            var count = 3;
            var pictures = GetTestPictures(count);
            var sut = new AggregatePicture(pictures);

            Assert.AreEqual<int>(count, sut.AggregatedPictures.Count);
            for (var i = 0; i < count; i++)
            {
                Assert.AreEqual<Picture>(pictures[i], sut.AggregatedPictures[i]);
            }

            // Different properties aggregate to null.
            Assert.AreEqual<string>(null, sut.Tags);
            Assert.AreEqual<string>(null, sut.Title);
            Assert.AreEqual<bool?>(null, sut.VisibilityIsPublic);
#pragma warning disable 618 // Disable warning about obsolete members
            Assert.AreEqual<string>(null, sut.BatchId);
#pragma warning restore 618 // Restore warning about obsolete members
            Assert.AreEqual<ContentType?>(null, sut.ContentType);

            // Same properties aggregate to that value.
            Assert.AreEqual<string>(sut.AggregatedPictures[0].Description, sut.Description);
            Assert.AreEqual<bool?>(sut.AggregatedPictures[0].VisibilityIsFamily, sut.VisibilityIsFamily);
            Assert.AreEqual<bool?>(sut.AggregatedPictures[0].VisibilityIsFriend, sut.VisibilityIsFriend);
            Assert.AreEqual<DateTimeOffset?>(sut.AggregatedPictures[0].DateUploaded, sut.DateUploaded);
            Assert.AreEqual<Safety?>(sut.AggregatedPictures[0].Safety, sut.Safety);
            Assert.AreEqual<License?>(sut.AggregatedPictures[0].License, sut.License);

            // Collections aggregate to the union of all elements.
            Assert.AreEqual<int>(count + 1, sut.GroupIds.Count);
            Assert.IsTrue(sut.GroupIds.Contains("Group-Common"));
            for (int i = 1; i <= count; i++)
            {
                Assert.IsTrue(sut.GroupIds.Contains("Group-" + i));
            }

            Assert.AreEqual<int>(count + 1, sut.SetIds.Count);
            Assert.IsTrue(sut.SetIds.Contains("Set-Common"));
            for (int i = 1; i <= count; i++)
            {
                Assert.IsTrue(sut.SetIds.Contains("Set-" + i));
            }

            Assert.AreEqual<bool?>(true, sut.GroupIdsContains("Group-Common"));
            Assert.AreEqual<bool?>(true, sut.SetIdsContains("Set-Common"));
            Assert.AreEqual<bool?>(false, sut.GroupIdsContains("Dummy"));
            Assert.AreEqual<bool?>(false, sut.SetIdsContains("Dummy"));
            Assert.AreEqual<bool?>(null, sut.GroupIdsContains("Group-1"));
            Assert.AreEqual<bool?>(null, sut.SetIdsContains("Set-1"));

            // Now change some properties on the pictures and verify that the aggregate updates too.
            var dateUploaded = new DateTime(2010, 1, 1, 0, 0, 0);
            foreach (var picture in pictures)
            {
                picture.Title = "Common Title";
                picture.Description = Guid.NewGuid().ToString();
                picture.VisibilityIsPublic = true;
                picture.DateUploaded = dateUploaded;
#pragma warning disable 618 // Disable warning about obsolete members
                picture.BatchId = "Batch-9";
#pragma warning restore 618 // Restore warning about obsolete members
                picture.GroupIds.Add("Group-Common-New");
                picture.SetIds.Add("Set-Common-New");
            }

            Assert.AreEqual<string>("Common Title", sut.Title);
            Assert.AreEqual<string>(null, sut.Description);
            Assert.AreEqual<bool?>(true, sut.VisibilityIsPublic);
            Assert.AreEqual<DateTimeOffset?>(dateUploaded, sut.DateUploaded);
#pragma warning disable 618 // Disable warning about obsolete members
            Assert.AreEqual<string>("Batch-9", sut.BatchId);
#pragma warning restore 618 // Restore warning about obsolete members

            Assert.AreEqual<int>(count + 2, sut.GroupIds.Count);
            Assert.IsTrue(sut.GroupIds.Contains("Group-Common"));
            Assert.IsTrue(sut.GroupIds.Contains("Group-Common-New"));
            for (int i = 1; i <= count; i++)
            {
                Assert.IsTrue(sut.GroupIds.Contains("Group-" + i));
            }

            Assert.AreEqual<int>(count + 2, sut.SetIds.Count);
            Assert.IsTrue(sut.SetIds.Contains("Set-Common"));
            Assert.IsTrue(sut.SetIds.Contains("Set-Common-New"));
            for (int i = 1; i <= count; i++)
            {
                Assert.IsTrue(sut.SetIds.Contains("Set-" + i));
            }

            // Now change some properties on the aggregate and verify that the pictures update too.
            sut.Description = "Common Description";
            sut.Title = null;
            sut.VisibilityIsFamily = null;
            sut.VisibilityIsFriend = true;
            sut.VisibilityIsPublic = false;
            sut.DateUploaded = dateUploaded.AddDays(1);
#pragma warning disable 618 // Disable warning about obsolete members
            sut.BatchId = "Batch-5";
#pragma warning restore 618 // Restore warning about obsolete members
            sut.Safety = null;
            sut.ContentType = ContentType.Photo;
            sut.License = License.AttributionNoncommercialNoDerivativesCC;
            foreach (var picture in pictures)
            {
                Assert.AreEqual<string>("Common Description", picture.Description);
                Assert.AreEqual<string>(null, picture.Title);
                Assert.AreEqual<bool?>(null, picture.VisibilityIsFamily);
                Assert.AreEqual<bool?>(true, picture.VisibilityIsFriend);
                Assert.AreEqual<bool?>(false, picture.VisibilityIsPublic);
                Assert.AreEqual<DateTimeOffset?>(dateUploaded.AddDays(1), picture.DateUploaded);
#pragma warning disable 618 // Disable warning about obsolete members
                Assert.AreEqual<string>("Batch-5", picture.BatchId);
#pragma warning restore 618 // Restore warning about obsolete members
                Assert.AreEqual<Safety?>(null, picture.Safety);
                Assert.AreEqual<ContentType?>(ContentType.Photo, picture.ContentType);
                Assert.AreEqual<License?>(License.AttributionNoncommercialNoDerivativesCC, picture.License);
            }

            // Set tags on the aggregate which adds them to the aggregated pictures.
            sut.Tags = "  NewTag    ";
            sut.Tags = string.Empty;
            sut.Tags = null;

            // The Tags property remains null (always).
            Assert.AreEqual<string>(null, sut.Tags);

            // Add, replace and remove items from the group and set collections.
            sut.GroupIds[sut.GroupIds.IndexOf("Group-Common-New")] = "Group-Replaced";
            sut.GroupIds.Remove("Group-Common");
            sut.GroupIds.Add("Group-Common-Added");
            sut.GroupIds.Add("Group-Common-Added"); // Should only add once.
            sut.GroupIds.Remove("Dummy"); // Shouldn't throw.
            sut.GroupIds.Remove(null); // Shouldn't throw.
            sut.SetIds[sut.SetIds.IndexOf("Set-Common-New")] = "Set-Replaced";
            sut.SetIds.Remove("Set-Common");
            sut.SetIds.Add("Set-Common-Added");
            sut.SetIds.Add("Set-Common-Added"); // Should only add once.
            sut.SetIds.Remove("Dummy"); // Shouldn't throw.
            sut.SetIds.Remove(null); // Shouldn't throw.

            // Verify tags, groups and sets.
            for (var i = 0; i < pictures.Count; i++)
            {
                var picture = pictures[i];
                var id = (i + 1);

                // Each aggregated picture now contains the new tag at the end (trimmed and with a leading space).
                Assert.AreEqual<string>("Common Tag" + id + " NewTag", picture.Tags);

                // Each aggregated picture now has updated groups and sets.
                Assert.AreEqual<int>(3, picture.GroupIds.Count);
                Assert.IsTrue(picture.GroupIds.Contains("Group-" + id));
                Assert.IsTrue(picture.GroupIds.Contains("Group-Replaced"));
                Assert.IsTrue(picture.GroupIds.Contains("Group-Common-Added"));
                Assert.AreEqual<int>(3, picture.SetIds.Count);
                Assert.IsTrue(picture.SetIds.Contains("Set-" + id));
                Assert.IsTrue(picture.SetIds.Contains("Set-Replaced"));
                Assert.IsTrue(picture.SetIds.Contains("Set-Common-Added"));
            }
        }

        [TestMethod]
        public void AggregatePictureRaisesPropertyChanged()
        {
            var count = 2;
            var pictures = GetTestPictures(count);
            var sut = new AggregatePicture(pictures);
            var listener = new PropertyChangedListener(sut);

            sut.Description = pictures[0].Description;
            Assert.AreEqual<int>(0, listener.RaisedPropertyChangedEventArgs.Count);

            sut.Description = "New description";
            Assert.AreEqual<int>(1, listener.RaisedPropertyChangedEventArgs.Count);
        }

        [TestMethod]
        [DeploymentItem("Pictures", "Pictures")]
        public void AggregatePictureAggregatesFileInformation()
        {
            var pictures = new Picture[]
            {
                PictureProvider.GetPicture("Pictures\\PugetSails.jpg", null),
                PictureProvider.GetPicture("Pictures\\FlickrLogo.png", null)
            };
            var sut = new AggregatePicture(pictures);

            Assert.AreEqual<string>("\"Pictures\\PugetSails.jpg\" \"Pictures\\FlickrLogo.png\"", sut.FileName);
            Assert.AreEqual<long>(pictures.Sum(p => new FileInfo(p.FileName).Length), sut.FileSize);
        }

        #region Helper Methods

        private IList<Picture> GetTestPictures(int count)
        {
            var pictures = new List<Picture>();
            for (var i = 0; i < count; i++)
            {
                pictures.Add(GetTestPicture(i + 1));
            }
            return pictures;
        }

        private Picture GetTestPicture(int id)
        {
            var picture = new Picture();
#pragma warning disable 618 // Disable warning about obsolete members
            picture.BatchId = "Batch " + id;
#pragma warning restore 618 // Restore warning about obsolete members
            picture.DateUploaded = new DateTime(2009, 6, 8, 0, 0, 0);
            picture.Description = "Description";
            picture.FileName = "File " + id;
            picture.Tags = "Common Tag" + id;
            picture.Title = "Title " + id;
            picture.VisibilityIsFamily = true;
            picture.VisibilityIsFriend = false;
            picture.VisibilityIsPublic = (id % 2 == 0);
            picture.Safety = Safety.Safe;
            picture.ContentType = (id % 2 == 0 ? ContentType.Photo : ContentType.Screenshot);
            picture.License = License.AttributionCC;
            picture.SetIds.Add("Set-Common");
            picture.SetIds.Add("Set-" + id);
            picture.GroupIds.Add("Group-Common");
            picture.GroupIds.Add("Group-" + id);
            return picture;
        }

        #endregion
    }
}