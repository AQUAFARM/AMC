using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schedulr.Models;
using Schedulr.Providers;

namespace Schedulr.Test
{
    [TestClass]
    public class PictureProviderTest
    {
        public const string TestPicturesPath = "Pictures";

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPictureThrowsOnNullFileName()
        {
            PictureProvider.GetPicture(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPictureThrowsOnEmptyFileName()
        {
            PictureProvider.GetPicture(string.Empty, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPictureThrowsOnNonExistentFileName()
        {
            PictureProvider.GetPicture(Util.GetDummyFileName(), null);
        }

        [TestMethod]
        [DeploymentItem(TestPicturesPath, TestPicturesPath)]
        public void GetPicture()
        {
            var fileName = "Pictures\\PugetSails.jpg";
            var picture = PictureProvider.GetPicture(fileName, null);
            Assert.IsNotNull(picture);
            Assert.AreEqual<string>(null, picture.Description);
            Assert.AreEqual<string>(fileName, picture.FileName);
            Assert.AreEqual<long>(new FileInfo(fileName).Length, picture.FileSize);
            Assert.AreEqual<string>("PugetSails.jpg", picture.ShortFileName);
            Assert.AreEqual<string>(null, picture.Tags);
            Assert.AreEqual<string>("PugetSails", picture.Title);
            Assert.AreEqual<bool?>(true, picture.VisibilityIsFamily);
            Assert.AreEqual<bool?>(true, picture.VisibilityIsFriend);
            Assert.AreEqual<bool?>(true, picture.VisibilityIsPublic);
            Assert.AreEqual<ContentType?>(ContentType.None, picture.ContentType);
            Assert.AreEqual<Safety?>(Safety.None, picture.Safety);
            Assert.AreEqual<License?>(License.None, picture.License);

            Assert.AreEqual<DateTimeOffset?>(null, picture.DateUploaded); // This should never be set to anything different.
#pragma warning disable 618 // Disable warning about obsolete members
            Assert.AreEqual<string>(null, picture.BatchId); // This should never be set to anything different.
#pragma warning restore 618 // Restore warning about obsolete members

            Assert.IsNotNull(picture.GroupIds);
            Assert.AreEqual<int>(0, picture.GroupIds.Count);
            Assert.IsNotNull(picture.SetIds);
            Assert.AreEqual<int>(0, picture.SetIds.Count);
        }

        [TestMethod]
        [DeploymentItem(TestPicturesPath, TestPicturesPath)]
        public void GetPictureWithDefaults()
        {
            var fileName = "Pictures\\PugetSails.jpg";
            var pictureDefaults = GetPictureDefaults();
            var picture = PictureProvider.GetPicture(fileName, pictureDefaults);
            Assert.IsNotNull(picture);
            Assert.AreEqual<string>(pictureDefaults.Description, picture.Description);
            Assert.AreEqual<string>(fileName, picture.FileName);
            Assert.AreEqual<long>(new FileInfo(fileName).Length, picture.FileSize);
            Assert.AreEqual<string>("PugetSails.jpg", picture.ShortFileName);
            Assert.AreEqual<string>(pictureDefaults.Tags, picture.Tags);
            Assert.AreEqual<string>(pictureDefaults.Title, picture.Title);
            Assert.AreEqual<bool?>(false, picture.VisibilityIsFamily);
            Assert.AreEqual<bool?>(true, picture.VisibilityIsFriend);
            Assert.AreEqual<bool?>(false, picture.VisibilityIsPublic);
            Assert.AreEqual<ContentType?>(pictureDefaults.ContentType, picture.ContentType);
            Assert.AreEqual<Safety?>(pictureDefaults.Safety, picture.Safety);
            Assert.AreEqual<License?>(pictureDefaults.License, picture.License);

            Assert.AreEqual<DateTimeOffset?>(null, picture.DateUploaded); // This should never be set to anything different.
#pragma warning disable 618 // Disable warning about obsolete members
            Assert.AreEqual<string>(null, picture.BatchId); // This should never be set to anything different.
#pragma warning restore 618 // Restore warning about obsolete members

            Verifier.VerifyList<string>(pictureDefaults.GroupIds, picture.GroupIds);
            Verifier.VerifyList<string>(pictureDefaults.SetIds, picture.SetIds);
        }

        [TestMethod]
        [DeploymentItem(TestPicturesPath, TestPicturesPath)]
        public void GetPictureWithDefaultsDoesNotOverwriteEmptyProperties()
        {
            var fileName = "Pictures\\FlickrLogo.png";
            var pictureDefaults = new Picture();

            var picture = PictureProvider.GetPicture(fileName, pictureDefaults);
            Assert.IsNotNull(picture);
            Assert.AreEqual<string>(pictureDefaults.Description, picture.Description);
            Assert.AreEqual<string>(fileName, picture.FileName);
            Assert.AreEqual<long>(2750, picture.FileSize);
            Assert.AreEqual<string>("FlickrLogo.png", picture.ShortFileName);
            Assert.AreEqual<string>(pictureDefaults.Tags, picture.Tags);
            Assert.AreEqual<string>("FlickrLogo", picture.Title);
            Assert.AreEqual<bool?>(pictureDefaults.VisibilityIsFamily, picture.VisibilityIsFamily);
            Assert.AreEqual<bool?>(pictureDefaults.VisibilityIsFriend, picture.VisibilityIsFriend);
            Assert.AreEqual<bool?>(pictureDefaults.VisibilityIsPublic, picture.VisibilityIsPublic);
            Assert.AreEqual<ContentType?>(pictureDefaults.ContentType, picture.ContentType);
            Assert.AreEqual<Safety?>(pictureDefaults.Safety, picture.Safety);
            Assert.AreEqual<License?>(pictureDefaults.License, picture.License);

            Assert.AreEqual<DateTimeOffset?>(null, picture.DateUploaded); // This should never be set to anything different.
#pragma warning disable 618 // Disable warning about obsolete members
            Assert.AreEqual<string>(null, picture.BatchId); // This should never be set to anything different.
#pragma warning restore 618 // Restore warning about obsolete members

            Verifier.VerifyList<string>(pictureDefaults.GroupIds, picture.GroupIds);
            Verifier.VerifyList<string>(pictureDefaults.SetIds, picture.SetIds);
        }

        private Picture GetPictureDefaults()
        {
            return new Picture
            {
                Title = "title-1",
                Description = "description-1. This description spans\r\nmultiple lines\r\n\r\nwhich should be supported!",
                Tags = "tags-1",
                VisibilityIsFamily = false,
                VisibilityIsFriend = true,
                VisibilityIsPublic = false,
                DateUploaded = DateTime.Now,
#pragma warning disable 618 // Disable warning about obsolete members
                BatchId = "Batch-99",
#pragma warning restore 618 // Restore warning about obsolete members
                ContentType = ContentType.Photo,
                Safety = Safety.Safe,
                License = License.AllRightsReserved,
                GroupIds = { "group-id-2", "group-id-3" },
                SetIds = { "set-id-1" }
            };
        }
    }
}