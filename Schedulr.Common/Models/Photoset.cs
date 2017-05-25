using System.Runtime.Serialization;
using JelleDruyts.Windows.ObjectModel;

namespace Schedulr.Models
{
    /// <summary>
    /// A photoset.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class Photoset : PictureCollection
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the ID of the primary picture for the photoset.
        /// </summary>
        [DataMember]
        public string PrimaryPictureId
        {
            get { return this.GetValue(PrimaryPictureIdProperty); }
            set { this.SetValue(PrimaryPictureIdProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="PrimaryPictureId"/> observable property.
        /// </summary>
        public static ObservableProperty<string> PrimaryPictureIdProperty = new ObservableProperty<string, Photoset>(o => o.PrimaryPictureId);

        /// <summary>
        /// Gets or sets the description of the photoset.
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return this.GetValue(DescriptionProperty); }
            set { this.SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// The definition for the <see cref="Description"/> observable property.
        /// </summary>
        public static ObservableProperty<string> DescriptionProperty = new ObservableProperty<string, Photoset>(o => o.Description);

        #endregion
    }
}