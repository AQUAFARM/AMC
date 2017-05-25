using System;
using System.IO;
using JelleDruyts.Windows;
using Schedulr.Models;

namespace Schedulr.Providers
{
    public static class PictureProvider
    {
        #region GetPicture

        /// <summary>
        /// Gets the picture information for a picture file.
        /// </summary>
        /// <param name="fileName">The full path to the picture file.</param>
        /// <param name="pictureDefaults">The defaults for the picture.</param>
        /// <returns>The picture information for the given path.</returns>
        public static Picture GetPicture(string fileName, Picture pictureDefaults)
        {
            if (!File.Exists(fileName))
            {
                throw new ArgumentException("The specified file does not exist: " + fileName);
            }

            Picture picture;
            if (pictureDefaults != null)
            {
                picture = SerializationProvider.Clone<Picture>(pictureDefaults);
            }
            else
            {
                picture = new Picture();
            }
            picture.FileName = fileName;

            // Only set the title to the file name if no default title was present.
            if (string.IsNullOrWhiteSpace(picture.Title))
            {
                picture.Title = Path.GetFileNameWithoutExtension(fileName);
            }

            // Always clear some properties.
            InitializePicture(picture);

            return picture;
        }

        #endregion

        #region InitializePicture

        /// <summary>
        /// Initializes a picture so that some properties are always guaranteed to be cleared.
        /// </summary>
        /// <param name="picture">The picture to initialize.</param>
        public static void InitializePicture(Picture picture)
        {
#pragma warning disable 618 // Disable warning about obsolete members
            picture.BatchId = null;
#pragma warning restore 618 // Restore warning about obsolete members
            picture.DateUploaded = null;
            picture.PictureId = null;
            picture.PreviewUrl = null;
            picture.WebUrl = null;
        }

        #endregion
    }
}