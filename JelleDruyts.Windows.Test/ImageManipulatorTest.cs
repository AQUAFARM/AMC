using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using JelleDruyts.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JelleDruyts.Windows.Test
{
    [TestClass]
    public class ImageManipulatorTest
    {
        #region Constants

        public const string TestImagesPath = "Images";
        private const string LandscapeInputFileName = @"Images\Landscape.jpg";
        private const string PortraitInputFileName = @"Images\Portrait.jpg";
        private const string WatermarkInputFileName = @"Images\Watermark.png";

        #endregion

        #region Resize To Width & Height

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToDimensionsShouldNotResizeWithNullDimensions()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToDimensions(originalImage, null, null);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(ImageManipulator.GetDimensions(fileName), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToDimensionsShouldNotResizeWithSameDimensions()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToDimensions(originalImage, originalSize.Width, originalSize.Height);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToDimensionsShouldResizeToSpecifiedDimensionsWidthHeight()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToDimensions(originalImage, 100, 100);
                image.CopyTo(outputFileName);
            }
            Assert.AreNotEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(new Size(100, 100), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToDimensionsShouldResizeToSpecifiedDimensionsWidth()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToDimensions(originalImage, 100, null);
                image.CopyTo(outputFileName);
            }
            Assert.AreNotEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            var originalSize = ImageManipulator.GetDimensions(fileName);
            Assert.AreEqual<Size>(new Size(100, (int)(originalSize.Height * 100 / (double)originalSize.Width)), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToDimensionsShouldResizeToSpecifiedDimensionsHeight()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToDimensions(originalImage, null, 100);
                image.CopyTo(outputFileName);
            }
            Assert.AreNotEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            var originalSize = ImageManipulator.GetDimensions(fileName);
            Assert.AreEqual<Size>(new Size((int)(originalSize.Width * 100 / (double)originalSize.Height), 100), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        #endregion

        #region Resize To Percentage

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToPercentageShouldNotResizeWithSamePercentage()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToPercentage(originalImage, 1.0);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToPercentageShouldResizeDownToSpecifiedPercentage()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var percentage = 0.5;
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToPercentage(originalImage, percentage);
                image.CopyTo(outputFileName);
            }
            Assert.AreNotEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(new Size((int)(percentage * originalSize.Width), (int)(percentage * originalSize.Height)), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToPercentageShouldResizeUpToSpecifiedPercentage()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var percentage = 1.5;
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToPercentage(originalImage, percentage);
                image.CopyTo(outputFileName);
            }
            Assert.AreNotEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(new Size((int)(percentage * originalSize.Width), (int)(percentage * originalSize.Height)), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        #endregion

        #region Resize To Longest Side

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToLongestSideShouldNotResizeWithSameLongestSide()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToLongestSide(originalImage, Math.Max(originalSize.Width, originalSize.Height));
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToLongestSideShouldResizeDownToSpecifiedLongestSideLandscape()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToLongestSide(originalImage, 100);
                image.CopyTo(outputFileName);
            }
            Assert.AreNotEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(new Size(100, (int)(originalSize.Height * 100 / (double)originalSize.Width)), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToLongestSideShouldResizeUpToSpecifiedLongestSideLandscape()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToLongestSide(originalImage, 2000);
                image.CopyTo(outputFileName);
            }
            Assert.AreNotEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(new Size(2000, (int)(originalSize.Height * 2000 / (double)originalSize.Width)), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToLongestSideShouldResizeDownToSpecifiedLongestSidePortrait()
        {
            var fileName = PortraitInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToLongestSide(originalImage, 100);
                image.CopyTo(outputFileName);
            }
            Assert.AreNotEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(new Size((int)(originalSize.Width * 100 / (double)originalSize.Height), 100), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToLongestSideShouldResizeUpToSpecifiedLongestSidePortrait()
        {
            var fileName = PortraitInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToLongestSide(originalImage, 2000);
                image.CopyTo(outputFileName);
            }
            Assert.AreNotEqual<long>(new FileInfo(fileName).Length, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(new Size((int)(originalSize.Width * 2000 / (double)originalSize.Height), 2000), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        #endregion

        #region Resize To Maximum Size

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToMaximumSizeShouldNotResizeIfSmallerThanMaximumSize()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalFileSize = new FileInfo(fileName).Length;
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToMaximumSize(originalImage, originalFileSize + 1);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<long>(originalFileSize, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(ImageManipulator.GetDimensions(fileName), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToMaximumSizeShouldNotResizeIfEqualToMaximumSize()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalFileSize = new FileInfo(fileName).Length;
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToMaximumSize(originalImage, originalFileSize);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<long>(originalFileSize, new FileInfo(outputFileName).Length);
            Assert.AreEqual<Size>(ImageManipulator.GetDimensions(fileName), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToMaximumSizeShouldResizeDownToMaximumSize()
        {
            var fileName = PortraitInputFileName;
            var outputFileName = GetOutputFileName();
            var percentage = 0.5;
            var originalFileSize = new FileInfo(fileName).Length;
            var maximumFileSize = (long)(originalFileSize * percentage);
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToMaximumSize(originalImage, maximumFileSize);
                image.CopyTo(outputFileName);
            }
            Assert.IsTrue(new FileInfo(outputFileName).Length <= maximumFileSize);
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToMaximumSizeShouldResizeUpToMaximumSize()
        {
            var fileName = PortraitInputFileName;
            var outputFileName = GetOutputFileName();
            var percentage = 1.5;
            var originalFileSize = new FileInfo(fileName).Length;
            var maximumFileSize = (long)(originalFileSize * percentage);
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToMaximumSize(originalImage, maximumFileSize);
                image.CopyTo(outputFileName);
            }
            Assert.IsTrue(new FileInfo(outputFileName).Length <= maximumFileSize);
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ResizeToMaximumSizeShouldResizeEvenIfNearlySameSize()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalFileSize = new FileInfo(fileName).Length;
            var maximumFileSize = originalFileSize - 1;
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToMaximumSize(originalImage, maximumFileSize);
                image.CopyTo(outputFileName);
            }
            Assert.IsTrue(new FileInfo(outputFileName).Length <= maximumFileSize);
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        [ExpectedException(typeof(ArgumentException))]
        public void ResizeToMaximumSizeShouldNotResizeToImpossibleSize()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalFileSize = new FileInfo(fileName).Length;
            var maximumFileSize = 1;
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ResizeToMaximumSize(originalImage, maximumFileSize);
            }
        }

        #endregion

        #region Manipulate Image

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldAddTextWatermark()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            using (var originalImage = File.OpenRead(fileName))
            {
                var brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                brush.Opacity = 0.5;
                var manipulation = new TextWatermarkManipulation("Test Watermark") { Position = ContentAlignment.BottomRight, Brush = brush, FontSize = 20, Margin = 20, Typeface = new System.Windows.Media.Typeface("Courier New") };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(ImageManipulator.GetDimensions(fileName), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldAddImageWatermark()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ImageWatermarkManipulation(WatermarkInputFileName) { Position = ContentAlignment.TopRight };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(ImageManipulator.GetDimensions(fileName), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldAddWatermarks()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            using (var originalImage = File.OpenRead(fileName))
            {
                var brush1 = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                brush1.Opacity = 0.5;
                var manipulation1 = new TextWatermarkManipulation("First Watermark") { Position = ContentAlignment.MiddleCenter, Brush = brush1, FontSize = 20, Margin = int.MaxValue, Typeface = new System.Windows.Media.Typeface("Courier New") };
                var brush2 = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
                brush2.Opacity = 0.8;
                var manipulation2 = new TextWatermarkManipulation("Second Watermark") { Position = ContentAlignment.BottomRight, Brush = brush2, FontSize = 40, Margin = 20, Typeface = new System.Windows.Media.Typeface("Times New Roman") };
                var image = ImageManipulator.ManipulateImage(originalImage, new ImageManipulation[] { manipulation1, manipulation2 });
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(ImageManipulator.GetDimensions(fileName), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldResizeAndAddWatermark()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation1 = new ResizeManipulation(100);
                var manipulation2 = new TextWatermarkManipulation("Watermark") { Position = ContentAlignment.MiddleCenter, Brush = System.Windows.Media.Brushes.Red, FontSize = 20, Margin = int.MaxValue, Typeface = new System.Windows.Media.Typeface("Courier New") };
                var image = ImageManipulator.ManipulateImage(originalImage, new ImageManipulation[] { manipulation1, manipulation2 });
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(new Size(100, (int)(originalSize.Height * 100 / (double)originalSize.Width)), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeUpToSpecifiedPercentage()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(1.5) { AllowedDirections = AllowedResizeDirections.Down };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeDownToSpecifiedPercentage()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(0.5) { AllowedDirections = AllowedResizeDirections.Up };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeUpToSpecifiedDimensionsWidth()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(2000, null) { AllowedDirections = AllowedResizeDirections.Down };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeDownToSpecifiedDimensionsWidth()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(10, null) { AllowedDirections = AllowedResizeDirections.Up };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeUpToSpecifiedDimensionsHeight()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(null, 2000) { AllowedDirections = AllowedResizeDirections.Down };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeDownToSpecifiedDimensionsHeight()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(null, 10) { AllowedDirections = AllowedResizeDirections.Up };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeUpToSpecifiedDimensionsWidthHeight()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(2000, 2000) { AllowedDirections = AllowedResizeDirections.Down };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeDownToSpecifiedDimensionsWidthHeight()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(10, 10) { AllowedDirections = AllowedResizeDirections.Up };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeDownToSpecifiedDimensionsWidthLargeHeight()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(10, 2000) { AllowedDirections = AllowedResizeDirections.Up };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeDownToSpecifiedDimensionsLargeWidthHeight()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(2000, 10) { AllowedDirections = AllowedResizeDirections.Up };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeUpToSpecifiedLongestSide()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(2000) { AllowedDirections = AllowedResizeDirections.Down };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotResizeDownToSpecifiedLongestSide()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new ResizeManipulation(100) { AllowedDirections = AllowedResizeDirections.Up };
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldNotProcessNullManipulation()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            using (var originalImage = File.OpenRead(fileName))
            {
                var image = ImageManipulator.ManipulateImage(originalImage, (IImageManipulation)null);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(ImageManipulator.GetDimensions(fileName), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldAddBorder()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new BorderManipulation(new System.Windows.Thickness(10, 20, 30, 40));
                manipulation.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(new Size(originalSize.Width + 10 + 30, originalSize.Height + 20 + 40), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldCropImage()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new CropManipulation(new System.Windows.Thickness(10, 20, 30, 40));
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(new Size(originalSize.Width - 10 - 30, originalSize.Height - 20 - 40), ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        [TestMethod]
        [DeploymentItem(TestImagesPath, TestImagesPath)]
        public void ManipulateImageShouldMakeBlackAndWhite()
        {
            var fileName = LandscapeInputFileName;
            var outputFileName = GetOutputFileName();
            var originalSize = ImageManipulator.GetDimensions(fileName);
            using (var originalImage = File.OpenRead(fileName))
            {
                var manipulation = new BlackAndWhiteManipulation();
                var image = ImageManipulator.ManipulateImage(originalImage, manipulation);
                image.CopyTo(outputFileName);
            }
            Assert.AreEqual<Size>(originalSize, ImageManipulator.GetDimensions(outputFileName));
            VerifyMetadata(fileName, outputFileName);
        }

        #endregion

        #region Helper Methods

        private static string GetOutputFileName()
        {
            var testName = new StackTrace().GetFrame(1).GetMethod().Name;
            return string.Format(CultureInfo.CurrentCulture, "Output-{0}.png", testName);
        }

        private static void VerifyMetadata(string expectedImageFilename, string actualImageFilename)
        {
            var expectedMetadata = ImageManipulator.GetMetadata(expectedImageFilename);
            var actualMetadata = ImageManipulator.GetMetadata(actualImageFilename);
            Assert.IsNotNull(actualMetadata);
            Assert.AreEqual(expectedMetadata.ApplicationName, actualMetadata.ApplicationName);
            VerifyCollection(expectedMetadata.Author, actualMetadata.Author);
            Assert.AreEqual(expectedMetadata.CameraManufacturer, actualMetadata.CameraManufacturer);
            Assert.AreEqual(expectedMetadata.CameraModel, actualMetadata.CameraModel);
            Assert.AreEqual(expectedMetadata.Comment, actualMetadata.Comment);
            Assert.AreEqual(expectedMetadata.Copyright, actualMetadata.Copyright);
            Assert.AreEqual(expectedMetadata.DateTaken, actualMetadata.DateTaken);
            Assert.AreEqual(expectedMetadata.Format, actualMetadata.Format);
            VerifyCollection(expectedMetadata.Keywords, actualMetadata.Keywords);
            Assert.AreEqual(expectedMetadata.Location, actualMetadata.Location);
            Assert.AreEqual(expectedMetadata.Rating, actualMetadata.Rating);
            Assert.AreEqual(expectedMetadata.Subject, actualMetadata.Subject);
            Assert.AreEqual(expectedMetadata.Title, actualMetadata.Title);
        }

        private static void VerifyCollection(IList expected, IList actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.AreEqual(expected.Count, actual.Count);
                for (var i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(actual[i], expected[i]);
                }
            }
        }

        #endregion
    }
}