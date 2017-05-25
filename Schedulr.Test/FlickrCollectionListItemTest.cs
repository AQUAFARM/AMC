using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schedulr.Models;
using Schedulr.ViewModels;

namespace Schedulr.Test
{
    [TestClass]
    public class FlickrCollectionListItemTest
    {
        [TestMethod]
        public void IsSelectedShouldUpdateSinglePicture()
        {
            var flickrSet = new PictureCollection { Id = "Set-1", Name = "Set 1" };
            var picture = new Picture { SetIds = { "Set-1", "Set-2" } };
            var sut = new FlickrCollectionListItem(flickrSet, picture.SetIds, null, "MyPlaceholderImage");

            // Check the basic properties.
            Assert.AreEqual<PictureCollection>(flickrSet, sut.Collection);
            Assert.AreEqual<bool?>(null, sut.IsSelected);
            Assert.AreEqual<string>("MyPlaceholderImage", sut.PlaceholderImage);

            // Check that the picture hasn't changed yet.
            Assert.AreEqual<int>(2, picture.SetIds.Count);
            Assert.AreEqual<string>("Set-1", picture.SetIds[0]);
            Assert.AreEqual<string>("Set-2", picture.SetIds[1]);

            // Remove the Set from the collection by deselecting the item.
            sut.IsSelected = false;
            Assert.AreEqual<int>(1, picture.SetIds.Count);
            Assert.AreEqual<string>("Set-2", picture.SetIds[0]);

            sut.IsSelected = true;
            Assert.AreEqual<int>(2, picture.SetIds.Count);
            Assert.AreEqual<string>("Set-2", picture.SetIds[0]);
            Assert.AreEqual<string>("Set-1", picture.SetIds[1]);
        }
    }
}