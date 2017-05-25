using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Provides services to manipulate images.
    /// </summary>
    public static class ImageManipulator
    {
        #region Properties

        /// <summary>
        /// The default quality level when encoding JPEG images.
        /// </summary>
        [DefaultValue(90)]
        public static int OutputJpegQualityLevel { get; set; }

        /// <summary>
        /// The DPI to use when encoding images.
        /// </summary>
        [DefaultValue(96)]
        public static double OutputDpi { get; set; }

        #endregion

        #region Static Constructor

        /// <summary>
        /// Initializes the <see cref="ImageManipulator"/> class.
        /// </summary>
        static ImageManipulator()
        {
            OutputJpegQualityLevel = 90;
            OutputDpi = 96;
        }

        #endregion

        #region GetDimensions

        /// <summary>
        /// Gets the dimensions of an image.
        /// </summary>
        /// <param name="fileName">The file name of the image.</param>
        /// <returns>The dimensions of the image.</returns>
        public static Size GetDimensions(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                var frame = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.None);
                return new Size(frame.PixelWidth, frame.PixelHeight);
            }
        }

        #endregion

        #region GetMetadata

        /// <summary>
        /// Gets the metadata of an image.
        /// </summary>
        /// <param name="fileName">The file name of the image.</param>
        /// <returns>The metadata of the image.</returns>
        public static BitmapMetadata GetMetadata(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                var frame = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                return (BitmapMetadata)frame.Metadata;
            }
        }

        #endregion

        #region ResizeToMaximumSize

        /// <summary>
        /// Resizes an image to a specified maximum file size.
        /// </summary>
        /// <param name="image">The file name of the image.</param>
        /// <param name="maximumSizeBytes">The maximum size (in bytes) of the resized image.</param>
        /// <returns>A stream that contains the resized image, or the original image if it did not need to be resized.</returns>
        public static Stream ResizeToMaximumSize(Stream image, long maximumSizeBytes)
        {
            return ResizeToMaximumSize(image, maximumSizeBytes, null);
        }

        /// <summary>
        /// Resizes an image to a specified maximum file size.
        /// </summary>
        /// <param name="image">The file name of the image.</param>
        /// <param name="maximumSizeBytes">The maximum size (in bytes) of the resized image.</param>
        /// <param name="encoder">The encoder to use for the resized image.</param>
        /// <returns>A stream that contains the resized image, or the original image if it did not need to be resized.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static Stream ResizeToMaximumSize(Stream image, long maximumSizeBytes, BitmapEncoder encoder)
        {
            if (!image.CanSeek)
            {
                throw new ArgumentException("The image stream does not support seek operations which is required.");
            }
            var originalSize = image.Length;
            if (originalSize <= maximumSizeBytes)
            {
                return image;
            }
            else
            {
                // As a first guess of the percentage for resizing the side, assume the square root of the target file size
                // divided by the original file size (because doubling the sides would quadruple the file size (not taking
                // compression into account).
                // This algorithm has room for improvement, e.g. don't just stop as soon as it's smaller than the target
                // but keep getting closer to the maximum until a certain delta is reached.
                // Optionally also build in a safety net to only perform a specified number of attempts at resizing.
                var percentage = Math.Sqrt((double)maximumSizeBytes / originalSize);
                long? encodedSize = null;
                Stream result = null;
                while (encodedSize == null || encodedSize > maximumSizeBytes)
                {
                    if (result != null)
                    {
                        // Dispose any previous attempt.
                        result.Dispose();
                    }
                    image.Seek(0, SeekOrigin.Begin);
                    using (var manipulation = new ResizeManipulation(percentage))
                    {
                        result = ManipulateImage(image, manipulation, encoder, out encodedSize);
                    }
                    // Steadily decrease the percentage until the desired size is reached.
                    percentage -= 0.02;
                    if (percentage <= 0)
                    {
                        throw new ArgumentException("The image could not be resized to the specified maximum number of bytes.");
                    }
                }
                return result;
            }
        }

        #endregion

        #region ResizeToPercentage

        /// <summary>
        /// Resizes an image with a specified percentage.
        /// </summary>
        /// <param name="image">The image to process.</param>
        /// <param name="percentage">The percentage to resize the image.</param>
        /// <returns>A stream that contains the resized image, or the original image if it did not need to be resized.</returns>
        public static Stream ResizeToPercentage(Stream image, double percentage)
        {
            return ResizeToPercentage(image, percentage, null);
        }

        /// <summary>
        /// Resizes an image with a specified percentage.
        /// </summary>
        /// <param name="image">The file name of the image.</param>
        /// <param name="percentage">The percentage to resize the image.</param>
        /// <param name="encoder">The encoder to use for the resized image.</param>
        /// <returns>A stream that contains the resized image, or the original image if it did not need to be resized.</returns>
        public static Stream ResizeToPercentage(Stream image, double percentage, BitmapEncoder encoder)
        {
            using (var manipulation = new ResizeManipulation(percentage))
            {
                return ManipulateImage(image, manipulation, encoder);
            }
        }

        #endregion

        #region ResizeToLongestSide

        /// <summary>
        /// Resizes an image so that the longest side is the specified size.
        /// </summary>
        /// <param name="image">The image to process.</param>
        /// <param name="longestSide">The size of the longest side of the resized image.</param>
        /// <returns>A stream that contains the resized image, or the original image if it did not need to be resized.</returns>
        public static Stream ResizeToLongestSide(Stream image, int longestSide)
        {
            return ResizeToLongestSide(image, longestSide, null);
        }

        /// <summary>
        /// Resizes an image so that the longest side is the specified size.
        /// </summary>
        /// <param name="image">The image to process.</param>
        /// <param name="longestSide">The size of the longest side of the resized image.</param>
        /// <param name="encoder">The encoder to use for the resized image.</param>
        /// <returns>A stream that contains the resized image, or the original image if it did not need to be resized.</returns>
        public static Stream ResizeToLongestSide(Stream image, int longestSide, BitmapEncoder encoder)
        {
            using (var manipulation = new ResizeManipulation(longestSide))
            {
                return ManipulateImage(image, manipulation, encoder);
            }
        }

        #endregion

        #region ResizeToDimensions

        /// <summary>
        /// Resizes an image to a specified dimension.
        /// </summary>
        /// <param name="image">The image to process.</param>
        /// <param name="width">The new width of the resized image, or <see langword="null"/> to resize proportionally to the new <paramref name="height"/>.</param>
        /// <param name="height">The new height of the resized image, or <see langword="null"/> to resize proportionally to the new <paramref name="width"/>.</param>
        /// <returns>A stream that contains the resized image, or the original image if it did not need to be resized.</returns>
        public static Stream ResizeToDimensions(Stream image, int? width, int? height)
        {
            return ResizeToDimensions(image, width, height, null);
        }

        /// <summary>
        /// Resizes an image to a specified dimension.
        /// </summary>
        /// <param name="image">The image to process.</param>
        /// <param name="width">The new width of the resized image, or <see langword="null"/> to resize proportionally to the new <paramref name="height"/>.</param>
        /// <param name="height">The new height of the resized image, or <see langword="null"/> to resize proportionally to the new <paramref name="width"/>.</param>
        /// <returns>A stream that contains the resized image, or the original image if it did not need to be resized.</returns>
        /// <param name="encoder">The encoder to use for the resized image.</param>
        public static Stream ResizeToDimensions(Stream image, int? width, int? height, BitmapEncoder encoder)
        {
            using (var manipulation = new ResizeManipulation(width, height))
            {
                return ManipulateImage(image, manipulation, encoder);
            }
        }

        #endregion

        #region ManipulateImage

        /// <summary>
        /// Manipulates the specified image.
        /// </summary>
        /// <param name="image">The image to manipulate.</param>
        /// <param name="manipulation">The manipulation to apply.</param>
        /// <returns>A stream that contains the manipulated image, or the original image if it did not need to be manipulated.</returns>
        public static Stream ManipulateImage(Stream image, IImageManipulation manipulation)
        {
            return ManipulateImage(image, new IImageManipulation[] { manipulation }, null);
        }

        /// <summary>
        /// Manipulates the specified image.
        /// </summary>
        /// <param name="image">The image to manipulate.</param>
        /// <param name="manipulation">The manipulation to apply.</param>
        /// <param name="encoder">The encoder to use for the manipulated image.</param>
        /// <returns>A stream that contains the manipulated image, or the original image if it did not need to be manipulated.</returns>
        public static Stream ManipulateImage(Stream image, IImageManipulation manipulation, BitmapEncoder encoder)
        {
            return ManipulateImage(image, new IImageManipulation[] { manipulation }, encoder);
        }

        /// <summary>
        /// Manipulates the specified image.
        /// </summary>
        /// <param name="image">The image to manipulate.</param>
        /// <param name="manipulations">The manipulations to apply.</param>
        /// <returns>A stream that contains the manipulated image, or the original image if it did not need to be manipulated.</returns>
        public static Stream ManipulateImage(Stream image, IEnumerable<IImageManipulation> manipulations)
        {
            return ManipulateImage(image, manipulations, null);
        }

        /// <summary>
        /// Manipulates the specified image.
        /// </summary>
        /// <param name="image">The image to manipulate.</param>
        /// <param name="manipulations">The manipulations to apply.</param>
        /// <param name="encoder">The encoder to use for the manipulated image.</param>
        /// <returns>A stream that contains the manipulated image, or the original image if it did not need to be manipulated.</returns>
        public static Stream ManipulateImage(Stream image, IEnumerable<IImageManipulation> manipulations, BitmapEncoder encoder)
        {
            long? encodedSize;
            var stream = ManipulateImage(image, manipulations, encoder, out encodedSize);
            return stream;
        }

        #endregion

        #region Helper Methods

        private static Stream ManipulateImage(Stream image, IImageManipulation manipulation, BitmapEncoder encoder, out long? encodedSize)
        {
            return ManipulateImage(image, new IImageManipulation[] { manipulation }, encoder, out encodedSize);
        }

        private static Stream ManipulateImage(Stream image, IEnumerable<IImageManipulation> manipulations, BitmapEncoder encoder, out long? encodedSize)
        {
            encodedSize = null;
            if (manipulations == null || !manipulations.Any(m => m != null && m.ShouldApply()))
            {
                return image;
            }
            else
            {
                var original = BitmapFrame.Create(image);
                var processed = ProcessFrame(original, manipulations, OutputDpi);
                if (processed == null)
                {
                    // No resize was needed; try to return the original stream if it can be rewinded, otherwise process the original image.
                    if (image.CanSeek)
                    {
                        image.Seek(0, SeekOrigin.Begin);
                        return image;
                    }
                    else
                    {
                        processed = original;
                    }
                }
                return SaveEncodedImage(processed, encoder, out encodedSize);
            }
        }

        private static BitmapFrame ProcessFrame(BitmapFrame image, IEnumerable<IImageManipulation> manipulations, double dpi)
        {
            if (manipulations == null || !manipulations.Any(m => m != null && m.ShouldApply(image)))
            {
                return null;
            }
            var targetVisual = new DrawingVisual();
            RenderTargetBitmap target;
            using (var targetContext = targetVisual.RenderOpen())
            {
                var currentSize = new System.Windows.Rect(0, 0, image.PixelWidth, image.PixelHeight);

                // Draw the original image at the original size.
                var group = new DrawingGroup();
                group.Children.Add(new ImageDrawing(image, currentSize));
                targetContext.DrawDrawing(group);

                // Apply manipulations to the image and update its size.
                foreach (var manipulation in manipulations)
                {
                    if (manipulation != null && manipulation.ShouldApply(image))
                    {
                        currentSize = manipulation.Apply(targetContext, image, currentSize);
                    }
                }
                target = new RenderTargetBitmap((int)currentSize.Width, (int)currentSize.Height, dpi, dpi, PixelFormats.Default);
            }
            target.Render(targetVisual);
            foreach (var manipulation in manipulations)
            {
                if (manipulation != null)
                {
                    manipulation.Dispose();
                }
            }
            return BitmapFrame.Create(target, image.Thumbnail, image.Metadata as BitmapMetadata, image.ColorContexts);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static Stream SaveEncodedImage(BitmapFrame image, BitmapEncoder encoder, out long? encodedSize)
        {
            if (encoder == null)
            {
                encoder = new JpegBitmapEncoder { QualityLevel = OutputJpegQualityLevel };
            }
            var encodedImageStream = new MemoryStream();
            encoder.Frames.Add(image);
            encoder.Save(encodedImageStream);
            encodedSize = encodedImageStream.Length;
            encodedImageStream.Seek(0, SeekOrigin.Begin);
            return encodedImageStream;
        }

        #endregion
    }
}