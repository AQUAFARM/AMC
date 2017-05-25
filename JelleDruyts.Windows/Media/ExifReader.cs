using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace JelleDruyts.Windows.Media
{
    // Adapted from http://www.codeproject.com/KB/graphics/exiftagcol.aspx

    /// <summary>
    /// Defines methods for reading EXIF tags from images.
    /// </summary>
    public static class ExifReader
    {
        #region Static Fields

        private static readonly string[] ComponentsConfigurationValues = new string[] { "", "Y", "Cb", "Cr", "R", "G", "B" };
        private const string ReservedValue = "Reserved";

        #endregion

        #region GetTags

        /// <summary>
        /// Reads EXIF tags from a file.
        /// </summary>
        /// <param name="fileName">The file name of the image to load.</param>
        /// <returns>The EXIF tags read from the image file, or <see langword="null"/> if the file wasn't an image.</returns>
        public static IDictionary<ExifTagType, ExifTag> GetTags(string fileName)
        {
            return GetTags(fileName, true, false);
        }

        /// <summary>
        /// Reads EXIF tags from a file.
        /// </summary>
        /// <param name="fileName">The file name of the image to load.</param>
        /// <param name="useEmbeddedColorManagement"><see langword="true"/> to use color management information embedded in the data stream; otherwise <see langword="false"/>.</param>
        /// <param name="validateImageData"><see langword="true"/> to validate the image data; otherwise <see langword="false"/>.</param>
        /// <returns>The EXIF tags read from the image file, or <see langword="null"/> if the file wasn't an image.</returns>
        public static IDictionary<ExifTagType, ExifTag> GetTags(string fileName, bool useEmbeddedColorManagement, bool validateImageData)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                Image image = null;
                try
                {
                    try
                    {
                        image = Image.FromStream(stream, useEmbeddedColorManagement, validateImageData);
                    }
                    catch (ArgumentException)
                    {
                        // Ignore exceptions that are thrown when the file is not an image.
                        return null;
                    }
                    return GetTags(image);
                }
                finally
                {
                    if (image != null)
                    {
                        image.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Reads EXIF tags from a file.
        /// </summary>
        /// <param name="image">The image to load.</param>
        /// <returns>The EXIF tags read from the image file, or <see langword="null"/> if the file wasn't an image.</returns>
        public static IDictionary<ExifTagType, ExifTag> GetTags(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            return ReadTags(image.PropertyItems);
        }

        #endregion

        #region ReadTags

        private static IDictionary<ExifTagType, ExifTag> ReadTags(IEnumerable<PropertyItem> properties)
        {
            var tags = new Dictionary<ExifTagType, ExifTag>();

            foreach (var property in properties)
            {
                if (!Enum.IsDefined(typeof(ExifTagType), property.Id))
                {
                    continue;
                }

                var tagType = (ExifTagType)property.Id;
                if (tags.ContainsKey(tagType))
                {
                    continue;
                }

                var value = GetTagValue(property, tagType);
                var valueInterpretation = GetTagValueInterpretation(tagType, property.Value, value);
                tags.Add(tagType, new ExifTag(tagType, value, valueInterpretation));

                if (tagType == ExifTagType.Flash)
                {
                    // Add a custom tag that is a simple boolean value that determines if the flash fired or not.
                    // This depends on the last bit, i.e. if the value is even then the flash did not fire.
                    var flashFired = ((UInt16)value % 2 == 1);
                    tags.Add(ExifTagType.FlashFired, new ExifTag(ExifTagType.FlashFired, flashFired, GetTagValueInterpretation(ExifTagType.FlashFired, null, flashFired)));
                }
            }

            return tags;
        }

        #endregion

        #region GetTagValueInterpretation

        /// <summary>
        /// Gets the interpreted value of an EXIF tag.
        /// </summary>
        /// <param name="tagType">The type of the EXIF tag to interpret.</param>
        /// <param name="rawValue">The raw byte value of the EXIF tag.</param>
        /// <param name="value">The parsed value of the EXIF tag.</param>
        /// <returns>The interpreted value of the EXIF tag.</returns>
        public static string GetTagValueInterpretation(ExifTagType tagType, byte[] rawValue, object value)
        {
            try
            {
                switch (tagType)
                {
                    #region Version Numbers

                    case ExifTagType.ExifVersion:
                        return GetVersionInterpretation("Exif Version ", GetString(rawValue));
                    case ExifTagType.FlashpixVersion:
                        return GetVersionInterpretation("Flashpix Format Version ", GetString(rawValue));
                    case ExifTagType.GPSVersionID:
                        return GetVersionInterpretation("GPS Version ", rawValue);

                    #endregion

                    #region Lookup Tables

                    case ExifTagType.GPSLatitudeRef:
                    case ExifTagType.GPSDestLatitudeRef:
                        switch ((string)value)
                        {
                            case "N": return "North Latitude";
                            case "S": return "South Latitude";
                            default: return ReservedValue;
                        }
                    case ExifTagType.GPSLongitudeRef:
                    case ExifTagType.GPSDestLongitudeRef:
                        switch ((string)value)
                        {
                            case "E": return "East Longitude";
                            case "W": return "West Longitude";
                            default: return ReservedValue;
                        }
                    case ExifTagType.GPSAltitudeRef:
                        switch ((byte)value)
                        {
                            case 0: return "Sea Level";
                            case 1: return "Sea Level Reference (negative value)";
                            default: return ReservedValue;
                        }
                    case ExifTagType.GPSStatus:
                        switch ((string)value)
                        {
                            case "A": return "Measurement In Progress";
                            case "V": return "Measurement Interoperability";
                            default: return ReservedValue;
                        }
                    case ExifTagType.GPSMeasureMode:
                        switch ((string)value)
                        {
                            case "2": return "2-Dimensional Measurement";
                            case "3": return "3-Dimensional Measurement";
                            default: return ReservedValue;
                        }
                    case ExifTagType.GPSSpeedRef:
                    case ExifTagType.GPSDestDistanceRef:
                        switch ((string)value)
                        {
                            case "K": return "Kilometers Per Hour";
                            case "M": return "Miles Per Hour";
                            case "N": return "Knots";
                            default: return ReservedValue;
                        }
                    case ExifTagType.GPSTrackRef:
                        switch ((string)value)
                        {
                            case "T": return "True Direction";
                            case "M": return "Magnetic Direction";
                            default: return ReservedValue;
                        }
                    case ExifTagType.GPSDifferential:
                        switch ((UInt16)value)
                        {
                            case 0: return "Measurement Without Differential Correction";
                            case 1: return "Differential Correction Applied";
                            default: return ReservedValue;
                        }
                    case ExifTagType.Compression:
                        switch ((UInt16)value)
                        {
                            case 1: return "Uncompressed";
                            case 6: return "JPEG Compression (thumbnails only)";
                            default: return ReservedValue;
                        }
                    case ExifTagType.PhotometricInterpretation:
                        switch ((UInt16)value)
                        {
                            case 2: return "RGB";
                            case 6: return "YCbCr";
                            default: return ReservedValue;
                        }
                    case ExifTagType.Orientation:
                        switch ((UInt16)value)
                        {
                            case 1: return "The 0th row is at the visual top of the image, and the 0th column is the visual left-hand side.";
                            case 2: return "The 0th row is at the visual top of the image, and the 0th column is the visual right-hand side.";
                            case 3: return "The 0th row is at the visual bottom of the image, and the 0th column is the visual right-hand side.";
                            case 4: return "The 0th row is at the visual bottom of the image, and the 0th column is the visual left-hand side.";
                            case 5: return "The 0th row is the visual left-hand side of the image, and the 0th column is the visual top.";
                            case 6: return "The 0th row is the visual right-hand side of the image, and the 0th column is the visual top.";
                            case 7: return "The 0th row is the visual right-hand side of the image, and the 0th column is the visual bottom.";
                            case 8: return "The 0th row is the visual left-hand side of the image, and the 0th column is the visual bottom.";
                            default: return ReservedValue;
                        }
                    case ExifTagType.ResolutionUnit:
                    case ExifTagType.FocalPlaneResolutionUnit:
                        switch ((UInt16)value)
                        {
                            case 2: return "Inches";
                            case 3: return "Centimeters";
                            default: return ReservedValue;
                        }
                    case ExifTagType.YCbCrPositioning:
                        switch ((UInt16)value)
                        {
                            case 1: return "Centered";
                            case 6: return "Co-sited";
                            default: return ReservedValue;
                        }
                    case ExifTagType.ExposureProgram:
                        switch ((UInt16)value)
                        {
                            case 0: return "Not Defined";
                            case 1: return "Manual";
                            case 2: return "Normal Program";
                            case 3: return "Aperture Priority";
                            case 4: return "Shutter Priority";
                            case 5: return "Creative Program (biased toward depth of field)";
                            case 6: return "Action Program (biased toward fast shutter speed)";
                            case 7: return "Portrait Mode (for closeup photos with the background out of focus)";
                            case 8: return "Landscape Mode (for landscape photos with the background in focus)";
                            default: return ReservedValue;
                        }
                    case ExifTagType.MeteringMode:
                        switch ((UInt16)value)
                        {
                            case 0: return "Unknown";
                            case 1: return "Average";
                            case 2: return "Center Weighted Average";
                            case 3: return "Spot";
                            case 4: return "MultiSpot";
                            case 5: return "Pattern";
                            case 6: return "Partial";
                            case 255: return "Other";
                            default: return ReservedValue;
                        }
                    case ExifTagType.LightSource:
                        switch ((UInt16)value)
                        {
                            case 0: return "Unknown";
                            case 1: return "Daylight";
                            case 2: return "Fluorescent";
                            case 3: return "Tungsten (Incandescent Light)";
                            case 4: return "Flash";
                            case 9: return "Fine Weather";
                            case 10: return "Cloudy Weather";
                            case 11: return "Shade";
                            case 12: return "Daylight Fluorescent (D 5700 – 7100K)";
                            case 13: return "Day White Fluorescent (N 4600 – 5400K)";
                            case 14: return "Cool White Fluorescent (W 3900 – 4500K)";
                            case 15: return "White Fluorescent (WW 3200 – 3700K)";
                            case 16: return "Warm White Fluorescent (L 2600 – 3250K)";
                            case 17: return "Standard Light A";
                            case 18: return "Standard Light B";
                            case 19: return "Standard Light C";
                            case 20: return "D55";
                            case 21: return "D65";
                            case 22: return "D75";
                            case 23: return "D50";
                            case 24: return "ISO Studio Tungsten";
                            case 255: return "Other Light Source";
                            default: return ReservedValue;
                        }
                    case ExifTagType.Flash:
                        switch ((UInt16)value)
                        {
                            case 0x0: return "Flash did not fire";
                            case 0x1: return "Flash fired";
                            case 0x5: return "Strobe return light not detected";
                            case 0x7: return "Strobe return light detected";
                            case 0x9: return "Flash fired, compulsory flash mode";
                            case 0xD: return "Flash fired, compulsory flash mode, return light not detected";
                            case 0xF: return "Flash fired, compulsory flash mode, return light detected";
                            case 0x10: return "Flash did not fire, compulsory flash mode";
                            case 0x18: return "Flash did not fire, auto mode";
                            case 0x19: return "Flash fired, auto mode";
                            case 0x1D: return "Flash fired, auto mode, return light not detected";
                            case 0x1F: return "Flash fired, auto mode, return light detected";
                            case 0x20: return "No flash function";
                            case 0x41: return "Flash fired, red-eye reduction mode";
                            case 0x45: return "Flash fired, red-eye reduction mode, return light not detected";
                            case 0x47: return "Flash fired, red-eye reduction mode, return light detected";
                            case 0x49: return "Flash fired, compulsory flash mode, red-eye reduction mode";
                            case 0x4D: return "Flash fired, compulsory flash mode, red-eye reduction mode, return light not detected";
                            case 0x4F: return "Flash fired, compulsory flash mode, red-eye reduction mode, return light detected";
                            case 0x59: return "Flash fired, auto mode, red-eye reduction mode";
                            case 0x5D: return "Flash fired, auto mode, return light not detected, red-eye reduction mode";
                            case 0x5F: return "Flash fired, auto mode, return light detected, red-eye reduction mode";
                            default: return ReservedValue;
                        }
                    case ExifTagType.ColorSpace:
                        switch ((UInt16)value)
                        {
                            case 1: return "sRGB";
                            case 0xFFFF: return "Uncalibrated";
                            default: return ReservedValue;
                        }
                    case ExifTagType.SensingMethod:
                        switch ((UInt16)value)
                        {
                            case 1: return "Not defined";
                            case 2: return "One-chip color area sensor";
                            case 3: return "Two-chip color area sensor";
                            case 4: return "Three-chip color area sensor";
                            case 5: return "Color sequential area sensor";
                            case 7: return "Trilinear sensor";
                            case 8: return "Color sequential linear sensor";
                            default: return ReservedValue;
                        }
                    case ExifTagType.FileSource:
                        switch ((byte)value)
                        {
                            case 3: return "DSC";
                            default: return ReservedValue;
                        }
                    case ExifTagType.SceneType:
                        switch ((byte)value)
                        {
                            case 1: return "A directly photographed image";
                            default: return ReservedValue;
                        }
                    case ExifTagType.CustomRendered:
                        switch ((UInt16)value)
                        {
                            case 0: return "Normal Process";
                            case 1: return "Custom Process";
                            default: return ReservedValue;
                        }
                    case ExifTagType.ExposureMode:
                        switch ((UInt16)value)
                        {
                            case 0: return "Auto Exposure";
                            case 1: return "Manual Exposure";
                            case 2: return "Auto Bracket";
                            default: return ReservedValue;
                        }
                    case ExifTagType.WhiteBalance:
                        switch ((UInt16)value)
                        {
                            case 0: return "Auto White Balance";
                            case 1: return "Manual White Balance";
                            default: return ReservedValue;
                        }
                    case ExifTagType.SceneCaptureType:
                        switch ((UInt16)value)
                        {
                            case 0: return "Standard";
                            case 1: return "Landscape";
                            case 2: return "Portrait";
                            case 3: return "Night Scene";
                            default: return ReservedValue;
                        }
                    case ExifTagType.Contrast:
                        switch ((UInt16)value)
                        {
                            case 0: return "Normal";
                            case 1: return "Soft";
                            case 2: return "Hard";
                            default: return ReservedValue;
                        }
                    case ExifTagType.Saturation:
                        switch ((UInt16)value)
                        {
                            case 0: return "Normal";
                            case 1: return "Low Saturation";
                            case 2: return "High Saturation";
                            default: return ReservedValue;
                        }
                    case ExifTagType.Sharpness:
                        switch ((UInt16)value)
                        {
                            case 0: return "Normal";
                            case 1: return "Soft";
                            case 2: return "Hard";
                            default: return ReservedValue;
                        }
                    case ExifTagType.SubjectDistanceRange:
                        switch ((UInt16)value)
                        {
                            case 0: return "Unknown";
                            case 1: return "Macro";
                            case 2: return "Close View";
                            case 3: return "Distant View";
                            default: return ReservedValue;
                        }
                    case ExifTagType.GainControl:
                        switch ((UInt16)value)
                        {
                            case 0: return "None";
                            case 1: return "Low Gain Up";
                            case 2: return "High Gain Up";
                            case 3: return "Low Gain Down";
                            case 4: return "High Gain Down";
                            default: return ReservedValue;
                        }

                    #endregion

                    #region Special Strings

                    case ExifTagType.GPSProcessingMethod:
                        return GetString(rawValue);
                    case ExifTagType.GPSAreaInformation:
                        return GetString(rawValue);
                    case ExifTagType.MakerNote:
                        return GetString(rawValue);
                    case ExifTagType.UserComment:
                        return GetString(rawValue);

                    #endregion

                    #region Units

                    case ExifTagType.FocalLength:
                        return ((double)value) + " mm";
                    case ExifTagType.FocalLengthIn35mmFilm:
                        return ((UInt16)value) + " mm";
                    case ExifTagType.GPSAltitude:
                    case ExifTagType.SubjectDistance:
                        return (double)value + " m";

                    #endregion

                    #region Other

                    case ExifTagType.ComponentsConfiguration:
                        return GetComponentsConfiguration(rawValue);
                    case ExifTagType.ExposureTime:
                        var time = (double)value;
                        if (time < 1)
                        {
                            return string.Format(CultureInfo.CurrentCulture, "1/{0} sec", 1.0 / time);
                        }
                        else
                        {
                            return string.Format(CultureInfo.CurrentCulture, "{0} sec", time);
                        }
                    case ExifTagType.FNumber:
                        return string.Format(CultureInfo.CurrentCulture, "f/{0}", Math.Round((double)value, 2));
                    case ExifTagType.ExposureBiasValue:
                        return GetExposureBiasInterpretation((double)value);
                    case ExifTagType.FlashFired:
                        return (bool)value ? "Yes" : "No";

                    #endregion
                }
            }
            catch (Exception)
            {
                // Ignore exceptions that arise from interpretation.
            }
            return (value == null ? null : value.ToString());
        }

        #endregion

        #region Helper Methods

        private static object GetTagValue(PropertyItem property, ExifTagType tagType)
        {
            if (property.Type == 0x1) // BYTE (8-bit unsigned int)
            {
                return property.Value[0];
            }
            else if (property.Type == 0x2) // ASCII (8 bit ASCII code)
            {
                var value = GetString(property.Value);

                // Convert known tags to DateTime.
                if (value != null && (tagType == ExifTagType.DateTime || tagType == ExifTagType.DateTimeOriginal || tagType == ExifTagType.DateTimeDigitized))
                {
                    DateTime dateTimeValue;
                    if (DateTime.TryParseExact(value, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTimeValue))
                    {
                        return dateTimeValue;
                    }
                }
                if (value != null && (tagType == ExifTagType.GPSDateStamp))
                {
                    DateTime dateTimeValue;
                    if (DateTime.TryParseExact(value, "yyyy:MM:dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTimeValue))
                    {
                        return dateTimeValue;
                    }
                }

                return value;
            }
            else if (property.Type == 0x3) // SHORT (16-bit unsigned int)
            {
                return BitConverter.ToUInt16(property.Value, 0);
            }
            else if (property.Type == 0x4) // LONG (32-bit unsigned int)
            {
                return BitConverter.ToUInt32(property.Value, 0);
            }
            else if (property.Type == 0x5) // RATIONAL (Two LONGs, unsigned)
            {
                if (property.Len == 24)
                {
                    // Certain RATIONAL properties (GPSLatitude, GPSLongitude, GPSDestLatitude, GPSDestLongitude, GPSTimeStamp)
                    // are in fact stored as 3 RATIONAL values (and therefore their length is 3 * 2 * 4 = 24 bits).
                    var values = new UnsignedRationalTrio(property.Value).ToDoubles();
                    if (tagType == ExifTagType.GPSLatitude || tagType == ExifTagType.GPSLatitudeRef || tagType == ExifTagType.GPSLongitude || tagType == ExifTagType.GPSLongitudeRef)
                    {
                        var degrees = values[0];
                        var minutes = values[1];
                        var seconds = values[2];
                        return degrees + (minutes / 60) + (seconds / (60 * 60));
                    }
                    else if (tagType == ExifTagType.GPSTimeStamp)
                    {
                        var hours = values[0];
                        var minutes = values[1];
                        var seconds = values[2];
                        return TimeSpan.FromHours(hours).Add(TimeSpan.FromMinutes(minutes)).Add(TimeSpan.FromSeconds(seconds));
                    }
                    else
                    {
                        return values;
                    }
                }
                else
                {
                    return new UnsignedRational(property.Value).ToDouble();
                }
            }
            else if (property.Type == 0x7) // UNDEFINED (8-bit)
            {
                return property.Value[0];
            }
            else if (property.Type == 0x9) // SLONG (32-bit int)
            {
                return BitConverter.ToInt32(property.Value, 0);
            }
            else if (property.Type == 0xA) // SRATIONAL (Two SLONGs, signed)
            {
                return new SignedRational(property.Value).ToDouble();
            }
            return null;
        }

        private static string GetString(byte[] bytes)
        {
            var value = Encoding.Default.GetString(bytes);
            value = value.Trim('\0');
            value = value.Replace("\r\n", "\n");
            value = value.Replace("\n", Environment.NewLine);
            return value;
        }

        private static string GetComponentsConfiguration(byte[] bytes)
        {
            var value = new StringBuilder();
            foreach (var b in bytes)
            {
                value.Append(ComponentsConfigurationValues[b]);
            }
            return value.ToString();
        }

        private static string GetVersionInterpretation(string prefix, string versionNumber)
        {
            if (versionNumber != null && versionNumber.Length == 4)
            {
                var majorString = versionNumber.Substring(0, 2);
                var minorString = versionNumber.Substring(2, 2);
                if (minorString.EndsWith("0"))
                {
                    minorString = minorString.Substring(0, 1);
                }
                int major;
                if (int.TryParse(majorString, out major))
                {
                    int minor;
                    if (int.TryParse(minorString, out minor))
                    {
                        return string.Concat(prefix, major, ".", minor);
                    }
                }
            }
            return versionNumber;
        }

        private static string GetVersionInterpretation(string prefix, byte[] versionBytes)
        {
            if (versionBytes != null && versionBytes.Length > 0)
            {
                var versionBuilder = new StringBuilder();
                foreach (var versionByte in versionBytes)
                {
                    if (versionBuilder.Length > 0)
                    {
                        versionBuilder.Append('.');
                    }
                    versionBuilder.Append(versionByte);
                }
                return prefix + versionBuilder.ToString();
            }
            return null;
        }

        private static string GetExposureBiasInterpretation(double exposureBias)
        {
            string baseValue;
            if ((exposureBias * 1) % 1 == 0)
            {
                baseValue = exposureBias.ToString();
            }
            else if ((exposureBias * 2) % 1 == 0)
            {
                baseValue = exposureBias * 2 + "/2";
            }
            else if ((exposureBias * 3) % 1 == 0)
            {
                baseValue = exposureBias * 3 + "/3";
            }
            else if ((exposureBias * 4) % 1 == 0)
            {
                baseValue = exposureBias * 4 + "/4";
            }
            else
            {
                baseValue = exposureBias.ToString();
            }
            if (exposureBias > 0)
            {
                baseValue = "+" + baseValue;
            }
            return baseValue + " eV";
        }

        #endregion

        #region Private Helper Classes

        private sealed class SignedRational
        {
            private Int32 _num;
            private Int32 _denom;

            public SignedRational(byte[] bytes)
            {
                byte[] n = new byte[4];
                byte[] d = new byte[4];
                Array.Copy(bytes, 0, n, 0, 4);
                Array.Copy(bytes, 4, d, 0, 4);
                _num = BitConverter.ToInt32(n, 0);
                _denom = BitConverter.ToInt32(d, 0);
            }

            public double ToDouble()
            {
                return Convert.ToDouble(_num) / Convert.ToDouble(_denom);
            }
        }

        private sealed class UnsignedRational
        {
            private UInt32 _num;
            private UInt32 _denom;

            public UnsignedRational(byte[] bytes)
            {
                byte[] n = new byte[4];
                byte[] d = new byte[4];
                Array.Copy(bytes, 0, n, 0, 4);
                Array.Copy(bytes, 4, d, 0, 4);
                _num = BitConverter.ToUInt32(n, 0);
                _denom = BitConverter.ToUInt32(d, 0);
            }

            public double ToDouble()
            {
                return Convert.ToDouble(_num) / Convert.ToDouble(_denom);
            }
        }

        private sealed class UnsignedRationalTrio
        {
            private UnsignedRational _first;
            private UnsignedRational _second;
            private UnsignedRational _third;

            public UnsignedRationalTrio(byte[] bytes)
            {
                byte[] first = new byte[8]; byte[] second = new byte[8]; byte[] third = new byte[8];

                Array.Copy(bytes, 0, first, 0, 8); Array.Copy(bytes, 8, second, 0, 8); Array.Copy(bytes, 16, third, 0, 8);

                _first = new UnsignedRational(first);
                _second = new UnsignedRational(second);
                _third = new UnsignedRational(third);
            }

            public double[] ToDoubles()
            {
                return new double[] { _first.ToDouble(), _second.ToDouble(), _third.ToDouble() };
            }
        }

        #endregion
    }
}