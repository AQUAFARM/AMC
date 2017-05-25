using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using JelleDruyts.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JelleDruyts.Windows.Test
{
    [TestClass]
    public class ExifReaderTest
    {
        #region Constants

        public const string TestImagesPath = "Images";
        private const string ExifInputFileName = @"Images\ExifSample.jpg";

        #endregion

        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void GetTagsShouldLoadExistingTagsFromImage()
        {
            var fileName = ExifInputFileName;
            var sut = ExifReader.GetTags(fileName);

            Assert.AreEqual<String>("                               ", (String)sut[ExifTagType.ImageDescription].Value);
            Assert.AreEqual<string>("                               ", sut[ExifTagType.ImageDescription].ValueInterpretation);
            Assert.AreEqual<String>("Canon", (String)sut[ExifTagType.Make].Value);
            Assert.AreEqual<string>("Canon", sut[ExifTagType.Make].ValueInterpretation);
            Assert.AreEqual<String>("Canon PowerShot SX230 HS", (String)sut[ExifTagType.Model].Value);
            Assert.AreEqual<string>("Canon PowerShot SX230 HS", sut[ExifTagType.Model].ValueInterpretation);
            Assert.AreEqual<Double>(180, (Double)sut[ExifTagType.XResolution].Value);
            Assert.AreEqual<string>(180.ToString(), sut[ExifTagType.XResolution].ValueInterpretation);
            Assert.AreEqual<Double>(180, (Double)sut[ExifTagType.YResolution].Value);
            Assert.AreEqual<string>(180.ToString(), sut[ExifTagType.YResolution].ValueInterpretation);
            Assert.AreEqual<UInt16>(2, (UInt16)sut[ExifTagType.ResolutionUnit].Value);
            Assert.AreEqual<string>("Inches", sut[ExifTagType.ResolutionUnit].ValueInterpretation);
            Assert.AreEqual<String>("Paint.NET v3.5.8", (String)sut[ExifTagType.Software].Value);
            Assert.AreEqual<string>("Paint.NET v3.5.8", sut[ExifTagType.Software].ValueInterpretation);
            Assert.AreEqual<DateTime>(new DateTime(2011, 5, 5, 21, 50, 22), (DateTime)sut[ExifTagType.DateTime].Value);
            Assert.AreEqual<string>(new DateTime(2011, 5, 5, 21, 50, 22).ToString(), sut[ExifTagType.DateTime].ValueInterpretation);
            Assert.AreEqual<String>("Sample Author", (String)sut[ExifTagType.Artist].Value);
            Assert.AreEqual<string>("Sample Author", sut[ExifTagType.Artist].ValueInterpretation);
            Assert.AreEqual<UInt16>(2, (UInt16)sut[ExifTagType.YCbCrPositioning].Value);
            Assert.AreEqual<string>("Reserved", sut[ExifTagType.YCbCrPositioning].ValueInterpretation);
            Assert.AreEqual<Double>(0.00125, (Double)sut[ExifTagType.ExposureTime].Value);
            Assert.AreEqual<string>("1/800 sec", sut[ExifTagType.ExposureTime].ValueInterpretation);
            Assert.AreEqual<Double>(5.6, (Double)sut[ExifTagType.FNumber].Value);
            Assert.AreEqual<string>("f/" + 5.6.ToString(), sut[ExifTagType.FNumber].ValueInterpretation);
            Assert.AreEqual<UInt16>(100, (UInt16)sut[ExifTagType.ISOSpeedRatings].Value);
            Assert.AreEqual<string>(100.ToString(), sut[ExifTagType.ISOSpeedRatings].ValueInterpretation);
            Assert.AreEqual<Byte>(48, (Byte)sut[ExifTagType.ExifVersion].Value);
            Assert.AreEqual<string>("Exif Version 2.3", sut[ExifTagType.ExifVersion].ValueInterpretation);
            Assert.AreEqual<DateTime>(new DateTime(2011, 5, 5, 21, 50, 22), (DateTime)sut[ExifTagType.DateTimeOriginal].Value);
            Assert.AreEqual<string>(new DateTime(2011, 5, 5, 21, 50, 22).ToString(), sut[ExifTagType.DateTimeOriginal].ValueInterpretation);
            Assert.AreEqual<DateTime>(new DateTime(2011, 5, 5, 21, 50, 22), (DateTime)sut[ExifTagType.DateTimeDigitized].Value);
            Assert.AreEqual<string>(new DateTime(2011, 5, 5, 21, 50, 22).ToString(), sut[ExifTagType.DateTimeDigitized].ValueInterpretation);
            Assert.AreEqual<Byte>(1, (Byte)sut[ExifTagType.ComponentsConfiguration].Value);
            Assert.AreEqual<string>("YCbCr", sut[ExifTagType.ComponentsConfiguration].ValueInterpretation);
            Assert.AreEqual<Double>(3, (Double)sut[ExifTagType.CompressedBitsPerPixel].Value);
            Assert.AreEqual<string>(3.ToString(), sut[ExifTagType.CompressedBitsPerPixel].ValueInterpretation);
            Assert.AreEqual<Double>(9.65625, (Double)sut[ExifTagType.ShutterSpeedValue].Value);
            Assert.AreEqual<string>(9.65625.ToString(), sut[ExifTagType.ShutterSpeedValue].ValueInterpretation);
            Assert.AreEqual<Double>(4.96875, (Double)sut[ExifTagType.ApertureValue].Value);
            Assert.AreEqual<string>(4.96875.ToString(), sut[ExifTagType.ApertureValue].ValueInterpretation);
            Assert.AreEqual<Double>(0, (Double)sut[ExifTagType.ExposureBiasValue].Value);
            Assert.AreEqual<string>("0 eV", sut[ExifTagType.ExposureBiasValue].ValueInterpretation);
            Assert.AreEqual<Double>(4.96875, (Double)sut[ExifTagType.MaxApertureValue].Value);
            Assert.AreEqual<string>(4.96875.ToString(), sut[ExifTagType.MaxApertureValue].ValueInterpretation);
            Assert.AreEqual<UInt16>(5, (UInt16)sut[ExifTagType.MeteringMode].Value);
            Assert.AreEqual<string>("Pattern", sut[ExifTagType.MeteringMode].ValueInterpretation);
            Assert.AreEqual<UInt16>(24, (UInt16)sut[ExifTagType.Flash].Value);
            Assert.AreEqual<string>("Flash did not fire, auto mode", sut[ExifTagType.Flash].ValueInterpretation);
            Assert.AreEqual<Boolean>(false, (Boolean)sut[ExifTagType.FlashFired].Value);
            Assert.AreEqual<string>("No", sut[ExifTagType.FlashFired].ValueInterpretation);
            Assert.AreEqual<Double>(53.034, (Double)sut[ExifTagType.FocalLength].Value);
            Assert.AreEqual<string>(53.034.ToString() + " mm", sut[ExifTagType.FocalLength].ValueInterpretation);
            Assert.AreEqual<Byte>(48, (Byte)sut[ExifTagType.FlashpixVersion].Value);
            Assert.AreEqual<string>("Flashpix Format Version 1.0", sut[ExifTagType.FlashpixVersion].ValueInterpretation);
            Assert.AreEqual<UInt16>(1, (UInt16)sut[ExifTagType.ColorSpace].Value);
            Assert.AreEqual<string>("sRGB", sut[ExifTagType.ColorSpace].ValueInterpretation);
            Assert.AreEqual<UInt16>(4000, (UInt16)sut[ExifTagType.PixelXDimension].Value);
            Assert.AreEqual<string>(4000.ToString(), sut[ExifTagType.PixelXDimension].ValueInterpretation);
            Assert.AreEqual<UInt16>(3000, (UInt16)sut[ExifTagType.PixelYDimension].Value);
            Assert.AreEqual<string>(3000.ToString(), sut[ExifTagType.PixelYDimension].ValueInterpretation);
            Assert.AreEqual<Double>(16393.4426229508, Math.Round((Double)sut[ExifTagType.FocalPlaneXResolution].Value, 10));
            Assert.AreEqual<string>(16393.4426229508.ToString(), sut[ExifTagType.FocalPlaneXResolution].ValueInterpretation);
            Assert.AreEqual<Double>(16393.4426229508, Math.Round((Double)sut[ExifTagType.FocalPlaneYResolution].Value, 10));
            Assert.AreEqual<string>(16393.4426229508.ToString(), sut[ExifTagType.FocalPlaneYResolution].ValueInterpretation);
            Assert.AreEqual<UInt16>(2, (UInt16)sut[ExifTagType.FocalPlaneResolutionUnit].Value);
            Assert.AreEqual<string>("Inches", sut[ExifTagType.FocalPlaneResolutionUnit].ValueInterpretation);
            Assert.AreEqual<UInt16>(2, (UInt16)sut[ExifTagType.SensingMethod].Value);
            Assert.AreEqual<string>("One-chip color area sensor", sut[ExifTagType.SensingMethod].ValueInterpretation);
            Assert.AreEqual<Byte>(3, (Byte)sut[ExifTagType.FileSource].Value);
            Assert.AreEqual<string>("DSC", sut[ExifTagType.FileSource].ValueInterpretation);
            Assert.AreEqual<UInt16>(0, (UInt16)sut[ExifTagType.CustomRendered].Value);
            Assert.AreEqual<string>("Normal Process", sut[ExifTagType.CustomRendered].ValueInterpretation);
            Assert.AreEqual<UInt16>(0, (UInt16)sut[ExifTagType.ExposureMode].Value);
            Assert.AreEqual<string>("Auto Exposure", sut[ExifTagType.ExposureMode].ValueInterpretation);
            Assert.AreEqual<UInt16>(0, (UInt16)sut[ExifTagType.WhiteBalance].Value);
            Assert.AreEqual<string>("Auto White Balance", sut[ExifTagType.WhiteBalance].ValueInterpretation);
            Assert.AreEqual<Double>(1, (Double)sut[ExifTagType.DigitalZoomRatio].Value);
            Assert.AreEqual<string>(1.ToString(), sut[ExifTagType.DigitalZoomRatio].ValueInterpretation);
            Assert.AreEqual<UInt16>(0, (UInt16)sut[ExifTagType.SceneCaptureType].Value);
            Assert.AreEqual<string>("Standard", sut[ExifTagType.SceneCaptureType].ValueInterpretation);
            Assert.AreEqual<Byte>(2, (Byte)sut[ExifTagType.GPSVersionID].Value);
            Assert.AreEqual<string>("GPS Version 2.3.0.0", sut[ExifTagType.GPSVersionID].ValueInterpretation);
            Assert.AreEqual<String>("N", (String)sut[ExifTagType.GPSLatitudeRef].Value);
            Assert.AreEqual<string>("North Latitude", sut[ExifTagType.GPSLatitudeRef].ValueInterpretation);
            Assert.AreEqual<Double>(37.0 + (9.0 / 60) + (26.255 / (60 * 60)), ((Double)sut[ExifTagType.GPSLatitude].Value));
            Assert.AreEqual<string>((37.0 + (9.0 / 60) + (26.255 / (60 * 60))).ToString(), sut[ExifTagType.GPSLatitude].ValueInterpretation);
            Assert.AreEqual<String>("W", (String)sut[ExifTagType.GPSLongitudeRef].Value);
            Assert.AreEqual<string>("West Longitude", sut[ExifTagType.GPSLongitudeRef].ValueInterpretation);
            Assert.AreEqual<Double>(93.0 + (13.0 / 60) + (38.915 / (60 * 60)), ((Double)sut[ExifTagType.GPSLongitude].Value));
            Assert.AreEqual<string>((93.0 + (13.0 / 60) + (38.915 / (60 * 60))).ToString(), sut[ExifTagType.GPSLongitude].ValueInterpretation);
            Assert.AreEqual<Byte>(0, (Byte)sut[ExifTagType.GPSAltitudeRef].Value);
            Assert.AreEqual<string>("Sea Level", sut[ExifTagType.GPSAltitudeRef].ValueInterpretation);
            Assert.AreEqual<Double>(412.47, (Double)sut[ExifTagType.GPSAltitude].Value);
            Assert.AreEqual<string>(412.47.ToString() + " m", sut[ExifTagType.GPSAltitude].ValueInterpretation);
            Assert.AreEqual<TimeSpan>(new TimeSpan(14, 50, 1), ((TimeSpan)sut[ExifTagType.GPSTimeStamp].Value));
            Assert.AreEqual<string>(new TimeSpan(14, 50, 1).ToString(), sut[ExifTagType.GPSTimeStamp].ValueInterpretation);
            Assert.AreEqual<String>("A", (String)sut[ExifTagType.GPSStatus].Value);
            Assert.AreEqual<string>("Measurement In Progress", sut[ExifTagType.GPSStatus].ValueInterpretation);
            Assert.AreEqual<String>("WGS-84", (String)sut[ExifTagType.GPSMapDatum].Value);
            Assert.AreEqual<string>("WGS-84", sut[ExifTagType.GPSMapDatum].ValueInterpretation);
            Assert.AreEqual<DateTime>(new DateTime(2011, 5, 5), (DateTime)sut[ExifTagType.GPSDateStamp].Value);
            Assert.AreEqual<string>(new DateTime(2011, 5, 5).ToString(), sut[ExifTagType.GPSDateStamp].ValueInterpretation);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void GetTagsShouldReadUnicodeStrings()
        {
            var fileName = @"Images\ExifDoubleTagsAndUnicode.jpg";
            var sut = ExifReader.GetTags(fileName);
            var expected = @"2011, June 26
Killeen, Texas

We walked down to the newly finished basketball goal at playground #2 to test out our new basketball. Marline and Rebecca sat on a bench nearby.

For this shot, the camera was just inches from Rebecca's chin.

<b>Copyright © 2011 by Wil C. Fry. All Rights Reserved.</b>
Please do not post group icons, &quot;awards&quot; or unrelated image files.
<b><i>Please do not use/repost this image without my permission.</i></b>";
            Assert.AreEqual<string>(expected, sut[ExifTagType.ImageDescription].ValueInterpretation);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void GetTagsShouldNotThrowOnAnyFile()
        {
            foreach (var fileName in Directory.GetFiles(TestImagesPath))
            {
                var sut = ExifReader.GetTags(fileName);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTagsShouldThrowOnNullFileName()
        {
            ExifReader.GetTags((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTagsShouldThrowOnNullImage()
        {
            ExifReader.GetTags((Image)null);
        }

        [TestMethod]
        public void GetTagsShouldReturnNullForNonImageFile()
        {
            var nonImageFileName = Assembly.GetExecutingAssembly().Location;
            var sut = ExifReader.GetTags(nonImageFileName);
            Assert.IsNull(sut);
        }
    }
}