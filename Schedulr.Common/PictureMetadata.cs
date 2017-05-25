using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using JelleDruyts.Windows;
using JelleDruyts.Windows.Media;
using Schedulr.Models;

namespace Schedulr
{
    /// <summary>
    /// Represents metadata about a picture.
    /// </summary>
    public class PictureMetadata
    {
        #region Properties

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the ISO speed.
        /// </summary>
        public int? IsoSpeed { get; private set; }

        /// <summary>
        /// Gets the geographic location.
        /// </summary>
        public GeoLocation GeoLocation { get; private set; }

        /// <summary>
        /// Gets the lens.
        /// </summary>
        public string Lens { get; private set; }

        /// <summary>
        /// Gets the make.
        /// </summary>
        public string Make { get; private set; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// Gets the exposure time in seconds.
        /// </summary>
        public double? ExposureTime { get; private set; }

        /// <summary>
        /// Gets the display-friendly exposure time.
        /// </summary>
        public string ExposureTimeInterpretation { get; private set; }

        /// <summary>
        /// Gets the F number.
        /// </summary>
        public double? FNumber { get; private set; }

        /// <summary>
        /// Gets the focal length in millimeters.
        /// </summary>
        public double? FocalLength { get; private set; }

        /// <summary>
        /// Gets the exposure bias.
        /// </summary>
        public double? ExposureBias { get; private set; }

        /// <summary>
        /// Gets the display-friendly exposure bias.
        /// </summary>
        public string ExposureBiasInterpretation { get; private set; }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        public IList<string> Tags { get; private set; }

        /// <summary>
        /// Gets a value that determines if the flash fired.
        /// </summary>
        public bool? FlashFired { get; private set; }

        /// <summary>
        /// Gets a display-friendly (Yes/No) value that determines if the flash fired.
        /// </summary>
        public string FlashFiredInterpretation { get; private set; }

        /// <summary>
        /// Gets the capture time.
        /// </summary>
        public DateTime? CaptureTime { get; private set; }

        /// <summary>
        /// Gets the exposure program.
        /// </summary>
        public int? ExposureProgram { get; private set; }

        /// <summary>
        /// Gets the display-friendly exposure program.
        /// </summary>
        public string ExposureProgramInterpretation { get; private set; }

        /// <summary>
        /// Gets the metering mode.
        /// </summary>
        public int? MeteringMode { get; private set; }

        /// <summary>
        /// Gets the display-friendly metering mode.
        /// </summary>
        public string MeteringModeInterpretation { get; private set; }

        /// <summary>
        /// Gets the additional metadata.
        /// </summary>
        [Browsable(false)]
        public IDictionary<string, string> AdditionalMetadata { get; private set; }

        #endregion

        #region ReadFromFile

        /// <summary>
        /// Retrieves metadata from a picture file and returns the information as a <see cref="PictureMetadata"/> instance.
        /// </summary>
        /// <param name="fileName">The path to the picture file to read.</param>
        public static PictureMetadata ReadFromFile(string fileName)
        {
            var metadata = new PictureMetadata();
            metadata.Tags = new List<string>();
            metadata.AdditionalMetadata = new Dictionary<string, string>();

            // First load EXIF data from the file.
            var exifTags = ExifReader.GetTags(fileName);
            if (exifTags != null)
            {
                metadata.Description = exifTags.ContainsKey(ExifTagType.ImageDescription) ? (string)exifTags[ExifTagType.ImageDescription].Value : null;
                metadata.IsoSpeed = exifTags.ContainsKey(ExifTagType.ISOSpeedRatings) ? (int?)(ushort)exifTags[ExifTagType.ISOSpeedRatings].Value : null;
                metadata.CaptureTime = exifTags.ContainsKey(ExifTagType.DateTimeOriginal) ? (DateTime?)(DateTime)exifTags[ExifTagType.DateTimeOriginal].Value : null;
                metadata.Make = exifTags.ContainsKey(ExifTagType.Make) ? (string)exifTags[ExifTagType.Make].Value : null;
                metadata.Model = exifTags.ContainsKey(ExifTagType.Model) ? (string)exifTags[ExifTagType.Model].Value : null;
                metadata.ExposureTime = exifTags.ContainsKey(ExifTagType.ExposureTime) ? (double?)(double)exifTags[ExifTagType.ExposureTime].Value : null;
                metadata.ExposureTimeInterpretation = exifTags.ContainsKey(ExifTagType.ExposureTime) ? exifTags[ExifTagType.ExposureTime].ValueInterpretation : null;
                metadata.FNumber = exifTags.ContainsKey(ExifTagType.FNumber) ? (double?)(double)exifTags[ExifTagType.FNumber].Value : null;
                metadata.FocalLength = exifTags.ContainsKey(ExifTagType.FocalLength) ? (double?)(double)exifTags[ExifTagType.FocalLength].Value : null;
                metadata.ExposureBias = exifTags.ContainsKey(ExifTagType.ExposureBiasValue) ? (double?)(double)exifTags[ExifTagType.ExposureBiasValue].Value : null;
                metadata.ExposureBiasInterpretation = exifTags.ContainsKey(ExifTagType.ExposureBiasValue) ? exifTags[ExifTagType.ExposureBiasValue].ValueInterpretation : null;
                metadata.FlashFired = exifTags.ContainsKey(ExifTagType.FlashFired) ? (bool?)(bool)exifTags[ExifTagType.FlashFired].Value : null;
                metadata.FlashFiredInterpretation = exifTags.ContainsKey(ExifTagType.FlashFired) ? exifTags[ExifTagType.FlashFired].ValueInterpretation : null;
                metadata.ExposureProgram = exifTags.ContainsKey(ExifTagType.ExposureProgram) ? (int?)(ushort)exifTags[ExifTagType.ExposureProgram].Value : null;
                metadata.ExposureProgramInterpretation = exifTags.ContainsKey(ExifTagType.ExposureProgram) ? exifTags[ExifTagType.ExposureProgram].ValueInterpretation : null;
                metadata.MeteringMode = exifTags.ContainsKey(ExifTagType.MeteringMode) ? (int?)(ushort)exifTags[ExifTagType.MeteringMode].Value : null;
                metadata.MeteringModeInterpretation = exifTags.ContainsKey(ExifTagType.MeteringMode) ? exifTags[ExifTagType.MeteringMode].ValueInterpretation : null;
                if (exifTags.ContainsKey(ExifTagType.GPSLatitude) && exifTags.ContainsKey(ExifTagType.GPSLongitude) && exifTags.ContainsKey(ExifTagType.GPSLatitudeRef) && exifTags.ContainsKey(ExifTagType.GPSLongitudeRef))
                {
                    var latitude = (double)exifTags[ExifTagType.GPSLatitude].Value;
                    var longitude = (double)exifTags[ExifTagType.GPSLongitude].Value;
                    if (string.Equals("S", (string)exifTags[ExifTagType.GPSLatitudeRef].Value, StringComparison.OrdinalIgnoreCase))
                    {
                        latitude = -latitude;
                    }
                    if (string.Equals("W", (string)exifTags[ExifTagType.GPSLongitudeRef].Value, StringComparison.OrdinalIgnoreCase))
                    {
                        longitude = -longitude;
                    }
                    metadata.GeoLocation = new GeoLocation(latitude, longitude, GeoLocation.MaxAccuracy);
                }
            }

            // Then attempt to find a raw metadata XML block inside the file.
            string fileContents = null;
            using (var reader = new StreamReader(fileName))
            {
                fileContents = reader.ReadToEnd();
            }
            const string beginCapture = "<rdf:RDF";
            const string endCapture = "</rdf:RDF>";
            var beginPos = fileContents.IndexOf(beginCapture, StringComparison.OrdinalIgnoreCase);
            var endPos = fileContents.IndexOf(endCapture, StringComparison.OrdinalIgnoreCase);
            if (beginPos >= 0 && endPos > beginPos)
            {
                var xmlPart = fileContents.Substring(beginPos, (endPos - beginPos) + endCapture.Length);
                PictureMetadata.ParseMetadataXml(metadata, xmlPart);
            }
            return metadata;
        }

        #endregion

        #region Helper Methods

        private static void ParseMetadataXml(PictureMetadata metadata, string rawMetadata)
        {
            // Load the metadata as XML and set up namespaces.
            var rawMetadataDocument = new XmlDocument();
            rawMetadataDocument.LoadXml(rawMetadata);
            var namespaceManager = new XmlNamespaceManager(rawMetadataDocument.NameTable);
            namespaceManager.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
            namespaceManager.AddNamespace("exif", "http://ns.adobe.com/exif/1.0/");
            namespaceManager.AddNamespace("x", "adobe:ns:meta/");
            namespaceManager.AddNamespace("xap", "http://ns.adobe.com/xap/1.0/");
            namespaceManager.AddNamespace("tiff", "http://ns.adobe.com/tiff/1.0/");
            namespaceManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

            // Get basic properties.
            foreach (var tag in GetInnerTextList("/rdf:RDF/rdf:Description/dc:subject/rdf:Bag/rdf:li", rawMetadataDocument, namespaceManager))
            {
                if (!metadata.Tags.Contains(tag, StringComparer.Ordinal))
                {
                    metadata.Tags.Add(tag);
                }
            }
            metadata.Title = metadata.Title ?? GetInnerText("/rdf:RDF/rdf:Description/dc:title/rdf:Alt/rdf:li", rawMetadataDocument, namespaceManager);
            metadata.Description = metadata.Description ?? GetInnerText("/rdf:RDF/rdf:Description/dc:description/rdf:Alt/rdf:li", rawMetadataDocument, namespaceManager);
            int isoSpeed;
            if (int.TryParse(GetInnerText("/rdf:RDF/rdf:Description/exif:ISOSpeedRatings/rdf:Seq", rawMetadataDocument, namespaceManager), out isoSpeed))
            {
                metadata.IsoSpeed = metadata.IsoSpeed ?? isoSpeed;
            }

            // See if the flash fired.
            var flashFiredText = GetInnerText("/rdf:RDF/rdf:Description/exif:Flash/exif:Fired", rawMetadataDocument, namespaceManager);
            if (string.IsNullOrEmpty(flashFiredText))
            {
                // If it's not stored as an XML element, it may also be stored as an attribute directly.
                flashFiredText = GetInnerText("/rdf:RDF/rdf:Description/exif:Flash/@exif:Fired", rawMetadataDocument, namespaceManager);
            }
            bool flashFired;
            if (!string.IsNullOrEmpty(flashFiredText) && bool.TryParse(flashFiredText, out flashFired))
            {
                metadata.FlashFired = metadata.FlashFired ?? flashFired;
            }
            if (metadata.FlashFired.HasValue)
            {
                metadata.FlashFiredInterpretation = metadata.FlashFiredInterpretation ?? ExifReader.GetTagValueInterpretation(ExifTagType.FlashFired, null, metadata.FlashFired.Value);
            }

            // Get all attributes and simple elements on the description node(s).
            foreach (XmlNode descriptionNode in rawMetadataDocument.SelectNodes("/rdf:RDF/rdf:Description", namespaceManager))
            {
                foreach (XmlAttribute descriptionAttribute in descriptionNode.Attributes)
                {
                    if (!string.IsNullOrEmpty(descriptionAttribute.Value))
                    {
                        metadata.AdditionalMetadata[descriptionAttribute.Name] = descriptionAttribute.Value;
                    }
                }
                foreach (XmlNode descriptionChildNode in descriptionNode.ChildNodes)
                {
                    if (descriptionChildNode.HasChildNodes && descriptionChildNode.FirstChild.NodeType == XmlNodeType.Text)
                    {
                        if (!string.IsNullOrEmpty(descriptionChildNode.FirstChild.InnerText))
                        {
                            metadata.AdditionalMetadata[descriptionChildNode.Name] = descriptionChildNode.FirstChild.InnerText;
                        }
                    }
                }
            }

            // Get the GPS location.
            if (metadata.AdditionalMetadata.ContainsKey("exif:GPSLatitude") && metadata.AdditionalMetadata.ContainsKey("exif:GPSLongitude"))
            {
                double latitude;
                double longitude;
                if (Sexagesimal.TryParse(metadata.AdditionalMetadata["exif:GPSLatitude"], out latitude) && Sexagesimal.TryParse(metadata.AdditionalMetadata["exif:GPSLongitude"], out longitude))
                {
                    metadata.GeoLocation = metadata.GeoLocation ?? new GeoLocation(latitude, longitude, GeoLocation.MaxAccuracy);
                }
            }

            // Get other properties.
            metadata.Lens = metadata.Lens ?? metadata.GetAdditionalMetadataAsString("aux:Lens");
            metadata.Make = metadata.Make ?? metadata.GetAdditionalMetadataAsString("tiff:Make");
            metadata.Model = metadata.Model ?? metadata.GetAdditionalMetadataAsString("tiff:Model");
            metadata.ExposureTime = metadata.ExposureTime ?? metadata.GetAdditionalMetadataAsDouble("exif:ExposureTime");
            if (metadata.ExposureTime.HasValue)
            {
                metadata.ExposureTimeInterpretation = metadata.ExposureTimeInterpretation ?? ExifReader.GetTagValueInterpretation(ExifTagType.ExposureTime, null, metadata.ExposureTime.Value);
            }
            metadata.FNumber = metadata.FNumber ?? metadata.GetAdditionalMetadataAsDouble("exif:FNumber");
            metadata.FocalLength = metadata.FocalLength ?? metadata.GetAdditionalMetadataAsInt("exif:FocalLength");
            metadata.ExposureBias = metadata.ExposureBias ?? metadata.GetAdditionalMetadataAsDouble("exif:ExposureBiasValue");
            if (metadata.ExposureBias.HasValue)
            {
                metadata.ExposureBiasInterpretation = metadata.ExposureBiasInterpretation ?? ExifReader.GetTagValueInterpretation(ExifTagType.ExposureBiasValue, null, metadata.ExposureBias.Value);
            }
            metadata.CaptureTime = metadata.CaptureTime ?? metadata.GetAdditionalMetadataAsDateTime("exif:DateTimeOriginal");
            metadata.ExposureProgram = metadata.ExposureProgram ?? metadata.GetAdditionalMetadataAsInt("exif:ExposureProgram");
            if (metadata.ExposureProgram.HasValue)
            {
                metadata.ExposureProgramInterpretation = metadata.ExposureProgramInterpretation ?? ExifReader.GetTagValueInterpretation(ExifTagType.ExposureProgram, null, (ushort)metadata.ExposureProgram.Value);
            }
            metadata.MeteringMode = metadata.MeteringMode ?? metadata.GetAdditionalMetadataAsInt("exif:MeteringMode");
            if (metadata.MeteringMode.HasValue)
            {
                metadata.MeteringModeInterpretation = metadata.MeteringModeInterpretation ?? ExifReader.GetTagValueInterpretation(ExifTagType.MeteringMode, null, (ushort)metadata.MeteringMode.Value);
            }
        }

        private static string GetInnerText(string xpath, XmlDocument metadata, XmlNamespaceManager namespaceManager)
        {
            var node = metadata.SelectSingleNode(xpath, namespaceManager);
            return (node == null ? null : node.InnerText);
        }

        private static IList<string> GetInnerTextList(string xpath, XmlDocument metadata, XmlNamespaceManager namespaceManager)
        {
            return metadata.SelectNodes(xpath, namespaceManager).Cast<XmlNode>().Select(n => n.InnerText).ToList();
        }

        private string GetAdditionalMetadataAsString(string key)
        {
            return (this.AdditionalMetadata.ContainsKey(key) ? this.AdditionalMetadata[key] : null);
        }

        private DateTime? GetAdditionalMetadataAsDateTime(string key)
        {
            var value = GetAdditionalMetadataAsString(key);
            return (value == null ? null : (DateTime?)DateTime.Parse(value));
        }

        private int? GetAdditionalMetadataAsInt(string key)
        {
            var value = GetAdditionalMetadataAsDouble(key);
            return (value == null ? null : (int?)value.Value);
        }

        private double? GetAdditionalMetadataAsDouble(string key)
        {
            var value = GetAdditionalMetadataAsString(key);
            if (value != null)
            {
                if (value.Contains("/"))
                {
                    try
                    {
                        var valueParts = value.Split('/');
                        return double.Parse(valueParts[0]) / double.Parse(valueParts[1]);
                    }
                    catch
                    {
                        // Ignore parsing errors.
                    }
                }
                else
                {
                    return double.Parse(value);
                }
            }
            return null;
        }

        #endregion
    }
}