using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schedulr.Models;
using Schedulr.Providers;

namespace Schedulr.Test
{
    [TestClass]
    public class PictureMetadataProviderTest
    {
        public const string TestPicturesPath = "Pictures";

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RetrieveMetadataFromFileThrowsOnNullFileName()
        {
            PictureMetadataProvider.RetrieveMetadataFromFile(null, new Picture(), new MockLogger());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RetrieveMetadataFromFileThrowsOnEmptyFileName()
        {
            PictureMetadataProvider.RetrieveMetadataFromFile(string.Empty, new Picture(), new MockLogger());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RetrieveMetadataFromFileThrowsOnNonExistentFileName()
        {
            PictureMetadataProvider.RetrieveMetadataFromFile(Util.GetDummyFileName(), new Picture(), new MockLogger());
        }

        [TestMethod]
        [DeploymentItem(TestPicturesPath, TestPicturesPath)]
        public void RetrieveMetadataFromFileRetrievesMetadata()
        {
            var fileName = "Pictures\\PugetSails.jpg";
            var picture = PictureProvider.GetPicture(fileName, null);
            PictureMetadataProvider.RetrieveMetadataFromFile(fileName, picture, new MockLogger());
            Assert.IsNotNull(picture);
            Assert.AreEqual<string>("I'm not sure if there was any specific reason to it, but on this particular sunny day in July, the Puget Sound (Seattle, US) was filled with small sailboats.", picture.Description);
            Assert.AreEqual<string>(fileName, picture.FileName);
            Assert.AreEqual<long>(new FileInfo(fileName).Length, picture.FileSize);
            Assert.AreEqual<string>("PugetSails.jpg", picture.ShortFileName);
            Assert.AreEqual<string>("Arty Light Sky Sun \"Sunny Day\"", picture.Tags);
            Assert.AreEqual<string>("Puget Sails", picture.Title);
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
        public void RetrieveMetadataFromFileWithDefaults()
        {
            var fileName = "Pictures\\PugetSails.jpg";
            var pictureDefaults = GetPictureDefaults();
            var picture = PictureProvider.GetPicture(fileName, pictureDefaults);
            PictureMetadataProvider.RetrieveMetadataFromFile(fileName, picture, new MockLogger());
            Assert.IsNotNull(picture);
            Assert.AreEqual<string>("I'm not sure if there was any specific reason to it, but on this particular sunny day in July, the Puget Sound (Seattle, US) was filled with small sailboats.", picture.Description);
            Assert.AreEqual<string>(fileName, picture.FileName);
            Assert.AreEqual<long>(new FileInfo(fileName).Length, picture.FileSize);
            Assert.AreEqual<string>("PugetSails.jpg", picture.ShortFileName);
            Assert.AreEqual<string>("Arty Light Sky Sun \"Sunny Day\"", picture.Tags);
            Assert.AreEqual<string>("Puget Sails", picture.Title);
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
        public void RetrieveMetadataFromFileWithNonExistantMetadata()
        {
            var fileName = "Pictures\\FlickrLogo.png";
            var picture = PictureProvider.GetPicture(fileName, null);
            PictureMetadataProvider.RetrieveMetadataFromFile(fileName, picture, new MockLogger());
            Assert.IsNotNull(picture);
            Assert.AreEqual<string>(null, picture.Description);
            Assert.AreEqual<string>(fileName, picture.FileName);
            Assert.AreEqual<long>(2750, picture.FileSize);
            Assert.AreEqual<string>("FlickrLogo.png", picture.ShortFileName);
            Assert.AreEqual<string>(null, picture.Tags);
            Assert.AreEqual<string>("FlickrLogo", picture.Title);
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
        public void RetrieveMetadataFromFileWithNonExistantMetadataWithDefaults()
        {
            var fileName = "Pictures\\FlickrLogo.png";
            var pictureDefaults = GetPictureDefaults();
            var picture = PictureProvider.GetPicture(fileName, pictureDefaults);
            PictureMetadataProvider.RetrieveMetadataFromFile(fileName, picture, new MockLogger());
            Assert.IsNotNull(picture);
            Assert.AreEqual<string>(pictureDefaults.Description, picture.Description);
            Assert.AreEqual<string>(fileName, picture.FileName);
            Assert.AreEqual<long>(2750, picture.FileSize);
            Assert.AreEqual<string>("FlickrLogo.png", picture.ShortFileName);
            Assert.AreEqual<string>(pictureDefaults.Tags, picture.Tags);
            Assert.AreEqual<string>("title-1", picture.Title);
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

        [TestMethod]
        [DeploymentItem(TestPicturesPath, TestPicturesPath)]
        public void RetrieveMetadataFromFileWithDefaultsDoesNotOverwriteEmptyProperties()
        {
            var fileName = "Pictures\\FlickrLogo.png";
            var pictureDefaults = new Picture();
            var picture = PictureProvider.GetPicture(fileName, pictureDefaults);
            PictureMetadataProvider.RetrieveMetadataFromFile(fileName, picture, new MockLogger());
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

        [TestMethod]
        [DeploymentItem(TestPicturesPath, TestPicturesPath)]
        public void RetrieveMetadataFromPictureFile()
        {
            VerifyMetadata("Pictures\\PugetSails.jpg");
        }

        [TestMethod]
        [DeploymentItem(TestPicturesPath, TestPicturesPath)]
        public void RetrieveMetadataFromSidecarFile()
        {
            VerifyMetadata("Pictures\\PugetSails.xmp");
        }

        [TestMethod]
        [DeploymentItem(TestPicturesPath, TestPicturesPath)]
        public void RetrieveMetadataWithExifGpsData()
        {
            var fileName = "Pictures\\ExifSample.jpg";
            var metadata = PictureMetadataProvider.RetrieveMetadataFromFile(fileName, new MockLogger());
            Assert.AreEqual<string>("                               ", metadata.Description);
            Assert.AreEqual<int>(0, metadata.Tags.Count);
            Assert.AreEqual<string>(null, metadata.Title);
            Assert.IsNotNull(metadata.GeoLocation);
            Assert.AreEqual<int>(GeoLocation.MaxAccuracy, metadata.GeoLocation.Accuracy);
            Assert.AreEqual<double>(37.157293, Math.Round(metadata.GeoLocation.Latitude, 6));
            Assert.AreEqual<double>(-93.227476, Math.Round(metadata.GeoLocation.Longitude, 6));
            Assert.AreEqual<bool>(false, metadata.FlashFired.Value);
            Assert.AreEqual<string>("No", metadata.FlashFiredInterpretation);
            Assert.AreEqual<int>(100, metadata.IsoSpeed.Value);
            Assert.AreEqual<string>(null, metadata.Lens);
            Assert.AreEqual<string>("Canon", metadata.Make);
            Assert.AreEqual<string>("Canon PowerShot SX230 HS", metadata.Model);
            Assert.AreEqual<double>(1.0 / 800.0, metadata.ExposureTime.Value);
            Assert.AreEqual<double>(5.6, metadata.FNumber.Value);
            Assert.AreEqual<double>(53.034, metadata.FocalLength.Value);
            Assert.AreEqual<double>(0.0, metadata.ExposureBias.Value);
            Assert.AreEqual<string>("0 eV", metadata.ExposureBiasInterpretation);
            Assert.AreEqual<DateTime>(new DateTime(2011, 5, 5, 21, 50, 22), metadata.CaptureTime.Value);
            Assert.AreEqual<int?>(null, metadata.ExposureProgram);
            Assert.AreEqual<string>(null, metadata.ExposureProgramInterpretation);
            Assert.AreEqual<string>("Pattern", metadata.MeteringModeInterpretation);
            Assert.AreEqual<int>(5, metadata.MeteringMode.Value);
            Assert.AreNotEqual<int>(0, metadata.AdditionalMetadata.Keys.Count);
        }

        private static void VerifyMetadata(string fileName)
        {
            var metadata = PictureMetadataProvider.RetrieveMetadataFromFile(fileName, new MockLogger());
            Assert.AreEqual<string>("I'm not sure if there was any specific reason to it, but on this particular sunny day in July, the Puget Sound (Seattle, US) was filled with small sailboats.", metadata.Description);
            Assert.AreEqual<string>("Arty|Light|Sky|Sun|Sunny Day", string.Join("|", metadata.Tags));
            Assert.AreEqual<string>("Puget Sails", metadata.Title);
            Assert.IsNotNull(metadata.GeoLocation);
            Assert.AreEqual<int>(GeoLocation.MaxAccuracy, metadata.GeoLocation.Accuracy);
            Assert.AreEqual<double>(47.606308, Math.Round(metadata.GeoLocation.Latitude, 6));
            Assert.AreEqual<double>(-122.341733, Math.Round(metadata.GeoLocation.Longitude, 6));
            Assert.AreEqual<bool>(false, metadata.FlashFired.Value);
            Assert.AreEqual<string>("No", metadata.FlashFiredInterpretation);
            Assert.AreEqual<int>(100, metadata.IsoSpeed.Value);
            Assert.AreEqual<string>("18.0-135.0 mm f/3.5-5.6", metadata.Lens);
            Assert.AreEqual<string>("NIKON CORPORATION", metadata.Make);
            Assert.AreEqual<string>("NIKON D80", metadata.Model);
            Assert.AreEqual<string>("1/320 sec", metadata.ExposureTimeInterpretation);
            Assert.AreEqual<double>(1.0 / 320.0, metadata.ExposureTime.Value);
            Assert.AreEqual<double>(10.0, metadata.FNumber.Value);
            Assert.AreEqual<double>(20.0, metadata.FocalLength.Value);
            Assert.AreEqual<double>(0.0, metadata.ExposureBias.Value);
            Assert.AreEqual<string>("0 eV", metadata.ExposureBiasInterpretation);
            Assert.AreEqual<DateTime>(new DateTime(2009, 6, 27, 12, 16, 15), metadata.CaptureTime.Value);
            Assert.AreEqual<string>("Not Defined", metadata.ExposureProgramInterpretation);
            Assert.AreEqual<int>(0, metadata.ExposureProgram.Value);
            Assert.AreEqual<string>("Pattern", metadata.MeteringModeInterpretation);
            Assert.AreEqual<int>(5, metadata.MeteringMode.Value);

            Assert.AreNotEqual<int>(0, metadata.AdditionalMetadata.Keys.Count);
            Assert.AreEqual<string>("6643856/1000000", metadata.AdditionalMetadata["exif:ApertureValue"]);
            Assert.AreEqual<string>("NIKON CORPORATION", metadata.AdditionalMetadata["tiff:Make"]);
            Assert.AreEqual<string>("18.0-135.0 mm f/3.5-5.6", metadata.AdditionalMetadata["aux:Lens"]);
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