using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using JelleDruyts.Windows;
using Microsoft.Win32;
using Schedulr.Extensibility;
using Schedulr.Infrastructure;
using Schedulr.Messages;
using Schedulr.Models;
using Schedulr.Providers;

namespace Schedulr.Views.Controls
{
    /// <summary>
    /// Interaction logic for RenderingPreview.xaml
    /// </summary>
    public partial class RenderingPreview : UserControl
    {
        #region Fields

        private Account account;
        private int currentIndex;
        private DispatcherTimer positionTimer;
        private bool isPositioning;

        #endregion

        #region Constructors

        public RenderingPreview()
        {
            InitializeComponent();
            this.currentIndex = -1;
            this.IsVisibleChanged += delegate { if (this.currentIndex < 0) { RefreshPreview(); } };
            Messenger.Register<AccountActionMessage>(OnAccountAction);
            this.positionTimer = new DispatcherTimer();
            this.positionTimer.Interval = TimeSpan.FromMilliseconds(250);
            this.positionTimer.Tick += new EventHandler(OnPositionTimerTick);
            RefreshPreview();
        }

        #endregion

        #region Message Handlers

        private void OnAccountAction(AccountActionMessage message)
        {
            if (message.Action == ListAction.CurrentChanged)
            {
                this.account = message.Account;
                this.currentIndex = -1;
                RefreshPreview();
            }
        }

        #endregion

        #region Event Handlers

        private void currentIndexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int newIndex;
            if (int.TryParse(this.currentIndexTextBox.Text, out newIndex))
            {
                newIndex -= 1;
                if (this.currentIndex != newIndex)
                {
                    this.currentIndex = newIndex;
                    RefreshPreview();
                }
            }
        }

        private void previousButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.account != null && this.currentIndex > 0)
            {
                this.currentIndex -= 1;
                RefreshPreview();
            }
        }

        private void nextButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.account != null && this.currentIndex < this.account.QueuedBatches.SelectMany(b => b.Pictures).Count() - 1)
            {
                this.currentIndex += 1;
                RefreshPreview();
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPreview();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SavePreview();
        }

        #endregion

        #region Helper Methods

        private void RefreshPreview()
        {
            this.positionTimer.IsEnabled = false;
            List<Picture> queuedPictures = null;
            if (this.account != null)
            {
                queuedPictures = this.account.QueuedBatches.SelectMany(b => b.Pictures).ToList();
            }
            if (queuedPictures != null && queuedPictures.Count > 0)
            {
                if (this.IsVisible)
                {
                    if (this.currentIndex < 0)
                    {
                        this.currentIndex = 0;
                    }
                    if (this.currentIndex > queuedPictures.Count - 1)
                    {
                        this.currentIndex = queuedPictures.Count - 1;
                    }
                    this.previousButton.IsEnabled = (this.currentIndex > 0);
                    this.nextButton.IsEnabled = (this.currentIndex < queuedPictures.Count - 1);
                    this.currentIndexTextBox.IsEnabled = true;
                    this.currentIndexTextBox.Text = (this.currentIndex + 1).ToString();
                    this.totalCountTextBlock.Text = "of " + queuedPictures.Count.ToString();

                    var picture = queuedPictures[this.currentIndex];
                    TemplateTokenProvider.SetSampleValue<Picture>(picture);
                    ShowMessage("Rendering preview. Please wait...", false);
                    PerformRendering(picture, e => { ShowMessage("An error occurred while rendering the preview: " + e.Message, true); }, e =>
                    {
                        try
                        {
                            if (e != null)
                            {
                                if (App.IsVideoFile(picture.FileName))
                                {
                                    var videoFileName = FileSystem.GetTempFileName(Path.GetExtension(picture.FileName));
                                    e.CopyTo(videoFileName);
                                    ShowPreviewVideo(videoFileName);
                                }
                                else
                                {
                                    ShowPreviewImage(BitmapFrame.Create(e, BitmapCreateOptions.None, BitmapCacheOption.OnLoad));
                                }
                            }
                            else
                            {
                                ShowMessage("No preview available", true);
                            }
                        }
                        catch (Exception exc)
                        {
                            ShowMessage("An error occurred while rendering the preview: " + exc.Message, true);
                        }
                    });
                }
            }
            else
            {
                this.previousButton.IsEnabled = false;
                this.nextButton.IsEnabled = false;
                this.currentIndexTextBox.IsEnabled = false;
                this.currentIndexTextBox.Text = "0";
                this.totalCountTextBlock.Text = "of 0";
                ShowMessage("Please add some files to the queue to render a preview here.", true);
            }
        }

        private void PerformRendering(Picture picture, Action<Exception> errorAction, Action<Stream> completedAction)
        {
            Logger.Log("Rendering file: " + picture.FileName, TraceEventType.Verbose);
            var isVideo = App.IsVideoFile(picture.FileName);
            Stream fileToRender = null;
            if (File.Exists(picture.FileName))
            {
                fileToRender = File.OpenRead(picture.FileName);
            }

            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                var modifiedFileToRender = PluginManager.OnRenderingFile(new RenderingEventArgs(App.Info, picture, isVideo), fileToRender);
                e.Result = modifiedFileToRender;
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                try
                {
                    if (e.Error != null)
                    {
                        if (errorAction != null)
                        {
                            errorAction(e.Error);
                        }
                    }
                    else
                    {
                        if (completedAction != null)
                        {
                            completedAction((Stream)e.Result);
                        }
                    }
                }
                finally
                {
                    PluginManager.OnRenderingFileCompleted(new RenderingEventArgs(App.Info, picture, isVideo));
                    if (fileToRender != null)
                    {
                        try
                        {
                            fileToRender.Dispose();
                        }
                        catch (ObjectDisposedException)
                        {
                            // Ignore exceptions from already disposed streams.
                        }
                    }
                }
            };
            worker.RunWorkerAsync();
        }

        private void SavePreview()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Please select a file name for the rendered preview";
            saveFileDialog.Filter = "All files|*.*";
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                var queuedPictures = this.account.QueuedBatches.SelectMany(b => b.Pictures).ToList();
                var picture = queuedPictures[this.currentIndex];
                PerformRendering(picture, e => { MessageBox.Show("An error occurred while rendering the file: " + e.Message, "Cannot save rendered preview", MessageBoxButton.OK, MessageBoxImage.Error); }, e =>
                {
                    if (e != null)
                    {
                        e.CopyTo(saveFileDialog.FileName);
                    }
                    else
                    {
                        MessageBox.Show("No preview available.", "No preview available", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }

        private void ShowMessage(string message, bool canRefresh)
        {
            this.previewImageContainer.Visibility = Visibility.Hidden;
            this.previewImage.Source = null;
            this.previewVideoContainer.Visibility = Visibility.Collapsed;
            this.previewVideo.Source = null;
            this.messageTextBlock.Text = message;
            this.messageTextBlock.Visibility = Visibility.Visible;
            this.saveButton.IsEnabled = false;
            this.refreshButton.IsEnabled = canRefresh;
            this.zoomSlider.IsEnabled = false;
            this.mediaInfoContainer.Visibility = Visibility.Collapsed;
        }

        private void ShowPreviewImage(BitmapFrame image)
        {
            // Make sure to show the image at real pixel size by scaling to match the default DPI of 96 used by WPF.
            this.dpiScaleTransform.ScaleX = image.DpiX / 96;
            this.dpiScaleTransform.ScaleY = image.DpiY / 96;
            this.previewImageContainer.Visibility = Visibility.Visible;
            this.previewImage.Source = image;
            this.previewVideoContainer.Visibility = Visibility.Collapsed;
            this.previewVideo.Source = null;
            this.messageTextBlock.Visibility = Visibility.Hidden;
            this.saveButton.IsEnabled = true;
            this.refreshButton.IsEnabled = true;
            this.zoomSlider.IsEnabled = true;
            this.mediaInfoTextBlock.Text = string.Format(CultureInfo.CurrentCulture, "{0} x {1} pixels", image.PixelWidth, image.PixelHeight);
            this.mediaInfoContainer.Visibility = Visibility.Visible;
        }

        private void ShowPreviewVideo(string fileName)
        {
            this.videoStopButton.IsEnabled = true;
            this.videoPauseButton.IsEnabled = true;
            this.videoPlayButton.IsEnabled = false;
            this.messageTextBlock.Visibility = Visibility.Hidden;
            this.saveButton.IsEnabled = true;
            this.refreshButton.IsEnabled = true;
            this.zoomSlider.IsEnabled = true;
            this.mediaInfoTextBlock.Text = string.Empty;
            this.mediaInfoContainer.Visibility = Visibility.Visible;
            this.positionSlider.IsEnabled = false;
            this.positionTextBlock.Text = "00:00 / 00:00";
            this.previewImageContainer.Visibility = Visibility.Hidden;
            this.previewImage.Source = null;
            this.previewVideoContainer.Visibility = Visibility.Visible;
            this.previewVideo.Source = new Uri(fileName);
            this.previewVideo.Play();
        }

        #endregion

        #region Video Controlling

        private void previewVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Delete the temporary file as soon as it's no longer needed.
            File.Delete(this.previewVideo.Source.LocalPath);
            this.mediaInfoTextBlock.Text = string.Format(CultureInfo.CurrentCulture, "{0} x {1} pixels", this.previewVideo.NaturalVideoWidth, this.previewVideo.NaturalVideoHeight);
            if (this.previewVideo.NaturalDuration.HasTimeSpan)
            {
                var totalDuration = this.previewVideo.NaturalDuration.TimeSpan;
                SetPositionText(null);
                this.positionSlider.Maximum = totalDuration.TotalMilliseconds;
                this.positionSlider.IsEnabled = true;
                this.positionTimer.IsEnabled = true;
            }
        }

        private void SetPositionText(TimeSpan? currentPosition)
        {
            if (currentPosition == null)
            {
                currentPosition = this.previewVideo.Position;
            }
            var positionMessage = string.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}", currentPosition.Value.TotalMinutes, currentPosition.Value.Seconds);
            if (this.previewVideo.NaturalDuration.HasTimeSpan)
            {
                var totalDuration = this.previewVideo.NaturalDuration.TimeSpan;
                positionMessage += string.Format(CultureInfo.CurrentCulture, " / {0:00}:{1:00}", totalDuration.TotalMinutes, totalDuration.Seconds);
            }
            this.positionTextBlock.Text = positionMessage;
        }

        private void OnPositionTimerTick(object sender, EventArgs e)
        {
            if (!this.isPositioning)
            {
                this.positionSlider.Value = this.previewVideo.Position.TotalMilliseconds;
            }
        }

        private void previewVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Loop the video infinitely.
            this.previewVideo.Position = TimeSpan.Zero;
            this.previewVideo.Play();
        }

        private void previewVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Logger.Log("An error occurred while playing the video preview", e.ErrorException, TraceEventType.Warning);

            // Delete the temporary file as soon as it's no longer needed.
            if (this.previewVideo.Source != null && this.previewVideo.Source.LocalPath != null)
            {
                File.Delete(this.previewVideo.Source.LocalPath);
            }
            ShowMessage("An error occurred while playing the preview: " + e.ErrorException.Message, true);
        }

        private void videoPlayButton_Click(object sender, RoutedEventArgs e)
        {
            this.previewVideo.Play();
            this.videoStopButton.IsEnabled = true;
            this.videoPauseButton.IsEnabled = true;
            this.videoPlayButton.IsEnabled = false;
        }

        private void videoPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.previewVideo.CanPause)
            {
                this.previewVideo.Pause();
                this.videoStopButton.IsEnabled = true;
                this.videoPauseButton.IsEnabled = false;
                this.videoPlayButton.IsEnabled = true;
            }
        }

        private void videoStopButton_Click(object sender, RoutedEventArgs e)
        {
            this.previewVideo.Stop();
            this.videoStopButton.IsEnabled = false;
            this.videoPauseButton.IsEnabled = false;
            this.videoPlayButton.IsEnabled = true;
        }

        private void positionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetPositionText(TimeSpan.FromMilliseconds(e.NewValue));
        }

        private void positionSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.isPositioning = true;
        }

        private void positionSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.isPositioning = false;
            this.previewVideo.Position = TimeSpan.FromMilliseconds(this.positionSlider.Value);
        }

        #endregion
    }
}