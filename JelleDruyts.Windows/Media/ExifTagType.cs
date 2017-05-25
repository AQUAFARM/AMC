
namespace JelleDruyts.Windows.Media
{
    /// <summary>
    /// Defines the supported types of EXIF tags.
    /// </summary>
    public enum ExifTagType
    {
        /// <summary>
        /// The custom (non-EXIF) Flash Fired tag.
        /// </summary>
        FlashFired = -1,

        /// <summary>
        /// The GPSVersionID tag.
        /// </summary>
        GPSVersionID = 0x0,

        /// <summary>
        /// The GPSLatitudeRef tag.
        /// </summary>
        GPSLatitudeRef = 0x1,

        /// <summary>
        /// The GPSLatitude tag.
        /// </summary>
        GPSLatitude = 0x2,

        /// <summary>
        /// The GPSLongitudeRef tag.
        /// </summary>
        GPSLongitudeRef = 0x3,

        /// <summary>
        /// The GPSLongitude tag.
        /// </summary>
        GPSLongitude = 0x4,

        /// <summary>
        /// The GPSAltitudeRef tag.
        /// </summary>
        GPSAltitudeRef = 0x5,

        /// <summary>
        /// The GPSAltitude tag.
        /// </summary>
        GPSAltitude = 0x6,

        /// <summary>
        /// The GPSTimeStamp tag.
        /// </summary>
        GPSTimeStamp = 0x7,

        /// <summary>
        /// The GPSSatellites tag.
        /// </summary>
        GPSSatellites = 0x8,

        /// <summary>
        /// The GPSStatus tag.
        /// </summary>
        GPSStatus = 0x9,

        /// <summary>
        /// The GPSMeasureMode tag.
        /// </summary>
        GPSMeasureMode = 0xa,

        /// <summary>
        /// The GPSDOP tag.
        /// </summary>
        GPSDOP = 0xb,

        /// <summary>
        /// The GPSSpeedRef tag.
        /// </summary>
        GPSSpeedRef = 0xc,

        /// <summary>
        /// The GPSSpeed tag.
        /// </summary>
        GPSSpeed = 0xd,

        /// <summary>
        /// The GPSTrackRef tag.
        /// </summary>
        GPSTrackRef = 0xe,

        /// <summary>
        /// The GPSTrack tag.
        /// </summary>
        GPSTrack = 0xf,

        /// <summary>
        /// The GPSImgDirectionRef tag.
        /// </summary>
        GPSImgDirectionRef = 0x10,

        /// <summary>
        /// The GPSImgDirection tag.
        /// </summary>
        GPSImgDirection = 0x11,

        /// <summary>
        /// The GPSMapDatum tag.
        /// </summary>
        GPSMapDatum = 0x12,

        /// <summary>
        /// The GPSDestLatitudeRef tag.
        /// </summary>
        GPSDestLatitudeRef = 0x13,

        /// <summary>
        /// The GPSDestLatitude tag.
        /// </summary>
        GPSDestLatitude = 0x14,

        /// <summary>
        /// The GPSDestLongitudeRef tag.
        /// </summary>
        GPSDestLongitudeRef = 0x15,

        /// <summary>
        /// The GPSDestLongitude tag.
        /// </summary>
        GPSDestLongitude = 0x16,

        /// <summary>
        /// The GPSDestBearingRef tag.
        /// </summary>
        GPSDestBearingRef = 0x17,

        /// <summary>
        /// The GPSDestBearing tag.
        /// </summary>
        GPSDestBearing = 0x18,

        /// <summary>
        /// The GPSDestDistanceRef tag.
        /// </summary>
        GPSDestDistanceRef = 0x19,

        /// <summary>
        /// The GPSDestDistance tag.
        /// </summary>
        GPSDestDistance = 0x1a,

        /// <summary>
        /// The GPSProcessingMethod tag.
        /// </summary>
        GPSProcessingMethod = 0x1b,

        /// <summary>
        /// The GPSAreaInformation tag.
        /// </summary>
        GPSAreaInformation = 0x1c,

        /// <summary>
        /// The GPSDateStamp tag.
        /// </summary>
        GPSDateStamp = 0x1d,

        /// <summary>
        /// The GPSDifferential tag.
        /// </summary>
        GPSDifferential = 0x1e,

        /// <summary>
        /// The ImageWidth tag.
        /// </summary>
        ImageWidth = 0x100,

        /// <summary>
        /// The ImageHeight tag.
        /// </summary>
        ImageHeight = 0x101,

        /// <summary>
        /// The BitsPerSample tag.
        /// </summary>
        BitsPerSample = 0x102,

        /// <summary>
        /// The Compression tag.
        /// </summary>
        Compression = 0x103,

        /// <summary>
        /// The PhotometricInterpretation tag.
        /// </summary>
        PhotometricInterpretation = 0x106,

        /// <summary>
        /// The ImageDescription tag.
        /// </summary>
        ImageDescription = 0x10e,

        /// <summary>
        /// The Make tag.
        /// </summary>
        Make = 0x10f,

        /// <summary>
        /// The Model tag.
        /// </summary>
        Model = 0x110,

        /// <summary>
        /// The StripOffsets tag.
        /// </summary>
        StripOffsets = 0x111,

        /// <summary>
        /// The Orientation tag.
        /// </summary>
        Orientation = 0x112,

        /// <summary>
        /// The SamplesPerPixel tag.
        /// </summary>
        SamplesPerPixel = 0x115,

        /// <summary>
        /// The RowsPerStrip tag.
        /// </summary>
        RowsPerStrip = 0x116,

        /// <summary>
        /// The StripByteCounts tag.
        /// </summary>
        StripByteCounts = 0x117,

        /// <summary>
        /// The XResolution tag.
        /// </summary>
        XResolution = 0x11a,

        /// <summary>
        /// The YResolution tag.
        /// </summary>
        YResolution = 0x11b,

        /// <summary>
        /// The PlanarConfiguration tag.
        /// </summary>
        PlanarConfiguration = 0x11c,

        /// <summary>
        /// The ResolutionUnit tag.
        /// </summary>
        ResolutionUnit = 0x128,

        /// <summary>
        /// The TransferFunction tag.
        /// </summary>
        TransferFunction = 0x12d,

        /// <summary>
        /// The Software tag.
        /// </summary>
        Software = 0x131,

        /// <summary>
        /// The DateTime tag.
        /// </summary>
        DateTime = 0x132,

        /// <summary>
        /// The Artist tag.
        /// </summary>
        Artist = 0x13b,

        /// <summary>
        /// The WhitePoint tag.
        /// </summary>
        WhitePoint = 0x13e,

        /// <summary>
        /// The PrimaryChromaticities tag.
        /// </summary>
        PrimaryChromaticities = 0x13f,

        /// <summary>
        /// The JPEGInterchangeFormat tag.
        /// </summary>
        JPEGInterchangeFormat = 0x201,

        /// <summary>
        /// The JPEGInterchangeFormatLength tag.
        /// </summary>
        JPEGInterchangeFormatLength = 0x202,

        /// <summary>
        /// The YCbCrCoefficients tag.
        /// </summary>
        YCbCrCoefficients = 0x211,

        /// <summary>
        /// The YCbCrSubSampling tag.
        /// </summary>
        YCbCrSubSampling = 0x212,

        /// <summary>
        /// The YCbCrPositioning tag.
        /// </summary>
        YCbCrPositioning = 0x213,

        /// <summary>
        /// The ReferenceBlackWhite tag.
        /// </summary>
        ReferenceBlackWhite = 0x214,

        /// <summary>
        /// The Copyright tag.
        /// </summary>
        Copyright = 0x8298,

        /// <summary>
        /// The ExposureTime tag.
        /// </summary>
        ExposureTime = 0x829a,

        /// <summary>
        /// The FNumber tag.
        /// </summary>
        FNumber = 0x829d,

        /// <summary>
        /// The ExposureProgram tag.
        /// </summary>
        ExposureProgram = 0x8822,

        /// <summary>
        /// The SpectralSensitivity tag.
        /// </summary>
        SpectralSensitivity = 0x8824,

        /// <summary>
        /// The ISOSpeedRatings tag.
        /// </summary>
        ISOSpeedRatings = 0x8827,

        /// <summary>
        /// The OECF tag.
        /// </summary>
        OECF = 0x8828,

        /// <summary>
        /// The ExifVersion tag.
        /// </summary>
        ExifVersion = 0x9000,

        /// <summary>
        /// The DateTimeOriginal tag.
        /// </summary>
        DateTimeOriginal = 0x9003,

        /// <summary>
        /// The DateTimeDigitized tag.
        /// </summary>
        DateTimeDigitized = 0x9004,

        /// <summary>
        /// The ComponentsConfiguration tag.
        /// </summary>
        ComponentsConfiguration = 0x9101,

        /// <summary>
        /// The CompressedBitsPerPixel tag.
        /// </summary>
        CompressedBitsPerPixel = 0x9102,

        /// <summary>
        /// The ShutterSpeedValue tag.
        /// </summary>
        ShutterSpeedValue = 0x9201,

        /// <summary>
        /// The ApertureValue tag.
        /// </summary>
        ApertureValue = 0x9202,

        /// <summary>
        /// The BrightnessValue tag.
        /// </summary>
        BrightnessValue = 0x9203,

        /// <summary>
        /// The ExposureBiasValue tag.
        /// </summary>
        ExposureBiasValue = 0x9204,

        /// <summary>
        /// The MaxApertureValue tag.
        /// </summary>
        MaxApertureValue = 0x9205,

        /// <summary>
        /// The SubjectDistance tag.
        /// </summary>
        SubjectDistance = 0x9206,

        /// <summary>
        /// The MeteringMode tag.
        /// </summary>
        MeteringMode = 0x9207,

        /// <summary>
        /// The LightSource tag.
        /// </summary>
        LightSource = 0x9208,

        /// <summary>
        /// The Flash tag.
        /// </summary>
        Flash = 0x9209,

        /// <summary>
        /// The FocalLength tag.
        /// </summary>
        FocalLength = 0x920a,

        /// <summary>
        /// The SubjectArea tag.
        /// </summary>
        SubjectArea = 0x9214,

        /// <summary>
        /// The MakerNote tag.
        /// </summary>
        MakerNote = 0x927c,

        /// <summary>
        /// The UserComment tag.
        /// </summary>
        UserComment = 0x9286,

        /// <summary>
        /// The SubSecTime tag.
        /// </summary>
        SubSecTime = 0x9290,

        /// <summary>
        /// The SubSecTimeOriginal tag.
        /// </summary>
        SubSecTimeOriginal = 0x9291,

        /// <summary>
        /// The SubSecTimeDigitized tag.
        /// </summary>
        SubSecTimeDigitized = 0x9292,

        /// <summary>
        /// The FlashpixVersion tag.
        /// </summary>
        FlashpixVersion = 0xa000,

        /// <summary>
        /// The ColorSpace tag.
        /// </summary>
        ColorSpace = 0xa001,

        /// <summary>
        /// The PixelXDimension tag.
        /// </summary>
        PixelXDimension = 0xa002,

        /// <summary>
        /// The PixelYDimension tag.
        /// </summary>
        PixelYDimension = 0xa003,

        /// <summary>
        /// The RelatedSoundFile tag.
        /// </summary>
        RelatedSoundFile = 0xa004,

        /// <summary>
        /// The FlashEnergy tag.
        /// </summary>
        FlashEnergy = 0xa20b,

        /// <summary>
        /// The SpatialFrequencyResponse tag.
        /// </summary>
        SpatialFrequencyResponse = 0xa20c,

        /// <summary>
        /// The FocalPlaneXResolution tag.
        /// </summary>
        FocalPlaneXResolution = 0xa20e,

        /// <summary>
        /// The FocalPlaneYResolution tag.
        /// </summary>
        FocalPlaneYResolution = 0xa20f,

        /// <summary>
        /// The FocalPlaneResolutionUnit tag.
        /// </summary>
        FocalPlaneResolutionUnit = 0xa210,

        /// <summary>
        /// The SubjectLocation tag.
        /// </summary>
        SubjectLocation = 0xa214,

        /// <summary>
        /// The ExposureIndex tag.
        /// </summary>
        ExposureIndex = 0xa215,

        /// <summary>
        /// The SensingMethod tag.
        /// </summary>
        SensingMethod = 0xa217,

        /// <summary>
        /// The FileSource tag.
        /// </summary>
        FileSource = 0xa300,

        /// <summary>
        /// The SceneType tag.
        /// </summary>
        SceneType = 0xa301,

        /// <summary>
        /// The CFAPattern tag.
        /// </summary>
        CFAPattern = 0xa302,

        /// <summary>
        /// The CustomRendered tag.
        /// </summary>
        CustomRendered = 0xa401,

        /// <summary>
        /// The ExposureMode tag.
        /// </summary>
        ExposureMode = 0xa402,

        /// <summary>
        /// The WhiteBalance tag.
        /// </summary>
        WhiteBalance = 0xa403,

        /// <summary>
        /// The DigitalZoomRatio tag.
        /// </summary>
        DigitalZoomRatio = 0xa404,

        /// <summary>
        /// The FocalLengthIn35mmFilm tag.
        /// </summary>
        FocalLengthIn35mmFilm = 0xa405,

        /// <summary>
        /// The SceneCaptureType tag.
        /// </summary>
        SceneCaptureType = 0xa406,

        /// <summary>
        /// The GainControl tag.
        /// </summary>
        GainControl = 0xa407,

        /// <summary>
        /// The Contrast tag.
        /// </summary>
        Contrast = 0xa408,

        /// <summary>
        /// The Saturation tag.
        /// </summary>
        Saturation = 0xa409,

        /// <summary>
        /// The Sharpness tag.
        /// </summary>
        Sharpness = 0xa40a,

        /// <summary>
        /// The DeviceSettingDescription tag.
        /// </summary>
        DeviceSettingDescription = 0xa40b,

        /// <summary>
        /// The SubjectDistanceRange tag.
        /// </summary>
        SubjectDistanceRange = 0xa40c,

        /// <summary>
        /// The ImageUniqueID tag.
        /// </summary>
        ImageUniqueID = 0xa420
    }
}