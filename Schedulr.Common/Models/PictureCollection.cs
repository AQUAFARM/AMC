using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// A collection of pictures on Flickr, i.e. a Set or a Group.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    [KnownType(typeof(Photoset))]
    public class PictureCollection : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the ID of the collection.
        /// </summary>
        [DataMember]
        public string Id
        {
            get { return this.GetValue(IdProperty); }
            set { this.SetValue(IdProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Id"/> observable property.
        /// </summary>
        public static ObservableProperty<string> IdProperty = new ObservableProperty<string, PictureCollection>(o => o.Id);

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return this.GetValue(NameProperty); }
            set { this.SetValue(NameProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Name"/> observable property.
        /// </summary>
        public static ObservableProperty<string> NameProperty = new ObservableProperty<string, PictureCollection>(o => o.Name);

        /// <summary>
        /// Gets or sets the URL for the image that represents this collection.
        /// </summary>
        [DataMember]
        public string ImageUrl
        {
            get { return this.GetValue(ImageUrlProperty); }
            set { this.SetValue(ImageUrlProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="ImageUrl"/> observable property.
        /// </summary>
        public static ObservableProperty<string> ImageUrlProperty = new ObservableProperty<string, PictureCollection>(o => o.ImageUrl);

        /// <summary>
        /// Gets or sets the number of items in the collection.
        /// </summary>
        [DataMember]
        public long Size
        {
            get { return this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Size"/> observable property.
        /// </summary>
        public static ObservableProperty<long> SizeProperty = new ObservableProperty<long, PictureCollection>(o => o.Size);

        #endregion
    }
}