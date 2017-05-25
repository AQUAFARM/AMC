using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Schedulr.Models;

namespace Schedulr.Providers
{
    /// <summary>
    /// Provides metadata services for pictures.
    /// </summary>
    public static class PictureMetadataProvider
    {
        #region RetrieveMetadata

        /// <summary>
        /// Retrieves the meta data.
        /// </summary>
        /// <param name="fileName">The file in which to find the metadata.</param>
        /// <param name="picture">The picture to update with the retrieved metadata.</param>
        /// <param name="logger">The logger to use.</param>
        /// <returns>The picture metadata.</returns>
        public static PictureMetadata RetrieveMetadataFromFile(string fileName, Picture picture, ILogger logger)
        {
            var metadata = RetrieveMetadataFromFile(fileName, logger);

            if (metadata != null)
            {
                if (!string.IsNullOrEmpty(metadata.Title))
                {
                    picture.Title = metadata.Title;
                }
                if (!string.IsNullOrEmpty(metadata.Description))
                {
                    picture.Description = metadata.Description;
                }
                if (metadata.GeoLocation != null)
                {
                    picture.Location = metadata.GeoLocation;
                }
                if (metadata.Tags.Count > 0)
                {
                    var tags = new StringBuilder();
                    foreach (var tag in metadata.Tags)
                    {
                        var tagToAdd = tag;
                        if (tags.Length > 0)
                        {
                            // Separate tags with spaces.
                            tags.Append(" ");
                        }
                        if (tagToAdd.Contains(" "))
                        {
                            // If the tag contains spaces, put double quotes around them.
                            tagToAdd = string.Format(CultureInfo.CurrentCulture, "\"{0}\"", tagToAdd);
                        }
                        tags.Append(tagToAdd);
                    }
                    picture.Tags = tags.ToString();
                }
            }

            return metadata;
        }

        /// <summary>
        /// Retrieves the meta data.
        /// </summary>
        /// <param name="fileName">The file in which to find the metadata.</param>
        /// <param name="logger">The logger to use.</param>
        /// <returns>The picture metadata.</returns>
        public static PictureMetadata RetrieveMetadataFromFile(string fileName, ILogger logger)
        {
            if (!File.Exists(fileName))
            {
                throw new ArgumentException("The specified file does not exist: " + fileName);
            }
            PictureMetadata metadata = null;
            try
            {
                if (logger != null)
                {
                    logger.Log(string.Format(CultureInfo.CurrentCulture, "Retrieving metadata from file \"{0}\"", fileName), TraceEventType.Information);
                }
                metadata = PictureMetadata.ReadFromFile(fileName);
                if (metadata == null && logger != null)
                {
                    logger.Log("Could not retrieve metadata from file " + fileName, TraceEventType.Information);
                }
            }
            catch (Exception exc)
            {
                if (logger != null)
                {
                    logger.Log("Could not retrieve metadata from file " + fileName, exc);
                }
            }
            return metadata;
        }

        #endregion
    }
}