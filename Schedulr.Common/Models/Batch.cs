using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// A batch of pictures that can be uploaded to Flickr.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class Batch : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets a value that determines if the batch is expanded in the UI.
        /// </summary>
        [DataMember]
        [DefaultValue(true)]
        [Browsable(false)]
        public bool IsExpanded
        {
            get { return this.GetValue(IsExpandedProperty); }
            set { this.SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="IsExpanded"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> IsExpandedProperty = new ObservableProperty<bool, Batch>(o => o.IsExpanded, true);

        /// <summary>
        /// Gets or sets a value that determines if a new photoset should be created for this batch.
        /// </summary>
        [DataMember]
        public bool CreatePhotosetForBatch
        {
            get { return this.GetValue(CreatePhotosetForBatchProperty); }
            set { this.SetValue(CreatePhotosetForBatchProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="CreatePhotosetForBatch"/> observable property.
        /// </summary>
        public static ObservableProperty<bool> CreatePhotosetForBatchProperty = new ObservableProperty<bool, Batch>(o => o.CreatePhotosetForBatch);

        /// <summary>
        /// Gets or sets the photoset to create for this batch.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public Photoset Photoset
        {
            get { return this.GetValue(PhotosetProperty); }
            set { this.SetValue(PhotosetProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Photoset"/> observable property.
        /// </summary>
        public static ObservableProperty<Photoset> PhotosetProperty = new ObservableProperty<Photoset, Batch>(o => o.Photoset);

        /// <summary>
        /// Gets or sets the pictures in this batch.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public  BulkObservableCollection<Picture> Pictures
        {
            get { return this.GetValue(PicturesProperty); }
            set { this.SetValue(PicturesProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Pictures"/> observable property.
        /// </summary>
        public static ObservableProperty<BulkObservableCollection<Picture>> PicturesProperty = new ObservableProperty<BulkObservableCollection<Picture>, Batch>(o => o.Pictures);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Batch"/> class.
        /// </summary>
        public Batch()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Batch"/> class.
        /// </summary>
        /// <param name="primaryPictureForPhotoset">The primary picture for the photoset.</param>
        public Batch(Picture primaryPictureForPhotoset)
        {
            this.Pictures = new BulkObservableCollection<Picture>();
            this.Photoset = new Photoset();
            this.Photoset.PrimaryPictureId = (primaryPictureForPhotoset == null ? null : primaryPictureForPhotoset.FileName);
        }

        #endregion
    }
}