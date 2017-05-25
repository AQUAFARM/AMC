using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Image = System.Windows.Controls.Image;

namespace JelleDruyts.Windows.Controls
{
    /// <summary>
    /// An <see cref="System.Windows.Controls.Image"/> control that initially displays the regular <see cref="System.Windows.Controls.Image.Source"/> until the <see cref="ActualSource"/> has been downloaded in the background.
    /// </summary>
    public class BackgroundLoadingImage : Image
    {
        #region Properties

        /// <summary>
        /// Gets the actual image source to be displayed (once it has been downloaded).
        /// </summary>
        public BitmapSource ActualImageSource { get; private set; }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the URI of the actual image to download in the background and then display.
        /// </summary>
        public Uri ActualSource
        {
            get { return (Uri)GetValue(ActualSourceProperty); }
            set { SetValue(ActualSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ActualSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualSourceProperty = DependencyProperty.Register("ActualSource", typeof(Uri), typeof(BackgroundLoadingImage), new UIPropertyMetadata(null, ActualSourceChanged));

        /// <summary>
        /// Gets or sets the width, in pixels, that the actual image is decoded to.
        /// </summary>
        public int? DecodePixelWidth
        {
            get { return (int?)GetValue(DecodePixelWidthProperty); }
            set { SetValue(DecodePixelWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DecodePixelWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DecodePixelWidthProperty = DependencyProperty.Register("DecodePixelWidth", typeof(int?), typeof(BackgroundLoadingImage), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the height, in pixels, that the actual image is decoded to.
        /// </summary>
        public int? DecodePixelHeight
        {
            get { return (int?)GetValue(DecodePixelHeightProperty); }
            set { SetValue(DecodePixelHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DecodePixelHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DecodePixelHeightProperty = DependencyProperty.Register("DecodePixelHeight", typeof(int?), typeof(BackgroundLoadingImage), new UIPropertyMetadata(null));

        #endregion

        #region Static Constructor

        /// <summary>
        /// Initializes the <see cref="BackgroundLoadingImage"/> class.
        /// </summary>
        static BackgroundLoadingImage()
        {
            // Apply new metadata for the Source property to get coercion callbacks.
            SourceProperty.AddOwner(typeof(BackgroundLoadingImage), new FrameworkPropertyMetadata(null, null, CoerceSource));
        }

        #endregion

        #region Helper Methods

        private static void ActualSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // If the actual image changed, reset the current ActualImageSource and start the download.
            var image = (BackgroundLoadingImage)sender;
            image.ActualImageSource = null;
            new ActualImageLoader(image);

            // Invalidate the Source property so that the displayed image is properly updated if needed.
            image.InvalidateProperty(SourceProperty);
        }

        private static object CoerceSource(DependencyObject sender, object baseValue)
        {
            // By coercing the Source property, we can return another value than the actual current property value.
            // We use this mechanism to return the actual image if it has been downloaded.
            BackgroundLoadingImage image = (BackgroundLoadingImage)sender;

            if (image.ActualImageSource != null && !image.ActualImageSource.IsDownloading)
            {
                // There is an actual image that has been downloaded, use that.
                return image.ActualImageSource;
            }
            else
            {
                // There is no actual image that has been downloaded, use the base value (i.e. the Source property).
                return baseValue;
            }
        }

        #endregion

        #region Private ActualImageLoader Class

        /// <summary>
        /// A helper class that performs the actual downloading.
        /// </summary>
        private class ActualImageLoader
        {
            /// <summary>
            /// The target image control to update.
            /// </summary>
            private BackgroundLoadingImage targetImage;

            /// <summary>
            /// The actual image being downloaded.
            /// </summary>
            private BitmapImage actualImage;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActualImageLoader"/> class.
            /// </summary>
            /// <param name="targetImage">The target image control to update.</param>
            public ActualImageLoader(BackgroundLoadingImage targetImage)
            {
                this.targetImage = targetImage;

                // Only start the download when all background operations have completed.
                this.targetImage.Dispatcher.BeginInvoke(new ThreadStart(StartDownload), DispatcherPriority.ContextIdle);
            }

            /// <summary>
            /// Starts the download.
            /// </summary>
            private void StartDownload()
            {
                try
                {
                    // Create the bitmap image that will be downloaded.
                    this.actualImage = new BitmapImage();
#if OFFLINE
                    if (!this.targetImage.ActualSource.IsFile)
                    {
                        this.ApplyActualImage(true);
                        return;
                    }
#endif
                    this.actualImage.BeginInit();
                    // Subscribe to both download events to make sure unsubscribing takes place even when the download fails.
                    this.actualImage.DownloadCompleted += SourceDownloadCompleted;
                    this.actualImage.DownloadFailed += SourceDownloadFailed;

                    // Set decode values if present.
                    if (this.targetImage.DecodePixelWidth.HasValue)
                    {
                        this.actualImage.DecodePixelWidth = this.targetImage.DecodePixelWidth.Value;
                    }
                    if (this.targetImage.DecodePixelHeight.HasValue)
                    {
                        this.actualImage.DecodePixelHeight = this.targetImage.DecodePixelHeight.Value;
                    }
                    this.actualImage.CacheOption = BitmapCacheOption.OnLoad;
                    this.actualImage.UriSource = this.targetImage.ActualSource;
                    this.actualImage.EndInit();

                    if (!this.actualImage.IsDownloading)
                    {
                        this.ApplyActualImage(false);
                    }
                }
                catch (Exception)
                {
                    this.ApplyActualImage(true);
                }
            }

            private void SourceDownloadCompleted(object sender, EventArgs e)
            {
                ApplyActualImage(false);
            }

            private void SourceDownloadFailed(object sender, ExceptionEventArgs e)
            {
                ApplyActualImage(true);
            }

            private void ApplyActualImage(bool failed)
            {
                // The download has completed, make sure to unsubscribe from download events and set the actual image.
                this.actualImage.DownloadCompleted -= SourceDownloadCompleted;
                this.actualImage.DownloadFailed -= SourceDownloadFailed;

                if (!failed)
                {
                    // We read the orientation exif info to adjust the shown image
                    var transformed = ProcessActualImage();
                    
                    // Make sure the BitmapSource is frozen so it can be shared across threads.
                    if (this.actualImage.CanFreeze)
                    {
                        this.actualImage.Freeze();
                    }

                    if (transformed != null)
                    {
                        if (transformed.CanFreeze)
                        {
                            transformed.Freeze();
                        }
                        this.targetImage.ActualImageSource = transformed;
                    }
                    else
                    {
                        // Set the actual image
                        this.targetImage.ActualImageSource = this.actualImage;
                    }
                    //force the Source property to be re-evaluated.
                    this.targetImage.InvalidateProperty(SourceProperty);
                }
            }
            /// <summary>
            /// Reorient the picture according to metadata if that exists
            /// </summary>
            private TransformedBitmap ProcessActualImage()
            {
                var frame = BitmapFrame.Create(
                    actualImage.UriSource,
                    BitmapCreateOptions.DelayCreation,
                    BitmapCacheOption.None);
                var meta = frame.Metadata as BitmapMetadata;

                if(meta!=null)
                {
                    if (meta.GetQuery("/app1/ifd/{ushort=274}") != null)
                    {
                        var orientation = (ExifOrientations)Enum.Parse(typeof(ExifOrientations), meta.GetQuery("/app1/ifd/{ushort=274}").ToString());
                        double angle = 0;
                        char flip = '-';

                        //The image is reoriented
                        switch (orientation)
                        {
                            //the image is orientated correctly
                            //or the orientation is unknown
                            case ExifOrientations.Normal:
                            case ExifOrientations.None:
                                break;

                            case ExifOrientations.HorizontalFlip:
                                flip = 'h';
                                break;

                            case ExifOrientations.Rotate180:
                                angle=180;
                                break;

                            case ExifOrientations.VerticalFlip:
                                flip = 'v';
                                break;

                            case ExifOrientations.Transpose:
                                angle = 90;
                                flip = 'h';
                                break;

                            case ExifOrientations.Rotate270:
                                angle = 90;
                                break;

                            case ExifOrientations.Transverse:
                                angle = -90;
                                flip = 'h';
                                break;

                            case ExifOrientations.Rotate90:
                                angle = -90;
                                break;
                        }

                        if (!angle.Equals(0))
                        {
                            var transformed = new TransformedBitmap();
                            transformed.BeginInit();
                            transformed.Source = this.actualImage;

                            var group = new TransformGroup();
                            
                            if (angle != 0)
                            {
                                group.Children.Add(new RotateTransform(angle, 0.5, 0.5));
                            }
                            if (flip!='-')
                            {
                                group.Children.Add(flip == 'h'
                                    ? new ScaleTransform() {ScaleX = -1, CenterX = 0.5, CenterY = 0.5}
                                    : new ScaleTransform() {ScaleY = -1, CenterX = 0.5, CenterY = 0.5});
                            }
                            transformed.Transform = group;
                            transformed.EndInit();

                            return transformed;
                        }
                    }
                }

                return null;
            }

            //for future use
            private Bitmap BitmapImage2Bitmap(BitmapImage bi)
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(bi));
                    enc.Save(outStream);
                    using (var newBitmap = new Bitmap(outStream))
                    {
                        // return bitmap; <-- leads to problems, stream is closed/closing ...
                        return new Bitmap(newBitmap);
                    }
                }
            }

            //converts the a bitmap to this.actualImage which is a BitmapImage
            private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Bmp);

                    stream.Position = 0;
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                    // Force the bitmap to load right now so we can dispose the stream.
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = stream;
                    bi.EndInit();

                    return bi;
                }
            }

            private enum ExifOrientations
            {
                None = 0,
                Normal = 1,
                HorizontalFlip = 2,
                Rotate180 = 3,
                VerticalFlip = 4,
                Transpose = 5,
                Rotate270 = 6,
                Transverse = 7,
                Rotate90 = 8
            }

        }

        #endregion
    }
}