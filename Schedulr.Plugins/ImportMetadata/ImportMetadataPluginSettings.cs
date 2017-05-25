using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Plugins.ImportMetadata
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class ImportMetadataPluginSettings : ObservableObject
    {
        #region Constants

        private const string DefaultCustomPictureDescription = @"$(PictureMetadataDescription)

Captured: $(PictureMetadataCaptureTime)
Camera: $(PictureMetadataModel) ($(PictureMetadataMake))
Lens: $(PictureMetadataLens)
Focal Length: $(PictureMetadataFocalLength) mm
ISO Speed: $(PictureMetadataIsoSpeed)
Aperture: f/$(PictureMetadataFNumber)
Shutter Speed: $(PictureMetadataExposureTimeInterpretation)
Exposure Bias: $(PictureMetadataExposureBiasInterpretation)
Exposure Program: $(PictureMetadataExposureProgramInterpretation)
Flash Fired: $(PictureMetadataFlashFiredInterpretation)
Metering Mode: $(PictureMetadataMeteringModeInterpretation)";

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets a value that determines if a custom format should be used for the picture description.
        /// </summary>
        [DataMember]
        public bool UseCustomPictureDescription
        {
            get { return this.GetValue(UseCustomPictureDescriptionProperty); }
            set { this.SetValue(UseCustomPictureDescriptionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="UseCustomPictureDescription"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> UseCustomPictureDescriptionProperty = new ObservableProperty<bool, ImportMetadataPluginSettings>(o => o.UseCustomPictureDescription);

        /// <summary>
        /// Gets or sets the text template to use for the picture description.
        /// </summary>
        [DataMember]
        public string CustomPictureDescription
        {
            get { return this.GetValue(CustomPictureDescriptionProperty); }
            set { this.SetValue(CustomPictureDescriptionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="CustomPictureDescription"/> observable property.
        /// </summary>
        public static ObservableProperty<string> CustomPictureDescriptionProperty = new ObservableProperty<string, ImportMetadataPluginSettings>(o => o.CustomPictureDescription, DefaultCustomPictureDescription);

        #endregion
    }
}