using System;
using System.Xml.Serialization;

namespace Schedulr.Models.Legacy
{
    /// <summary>
    /// Represents a <see cref="V1Picture"/> that has been uploaded.
    /// </summary>
    [Serializable]
    [XmlType(TypeName = "UploadedPicture")]
    public class V1UploadedPicture : V1Picture
    {
        #region Properties

        private DateTime uploaded;

        /// <summary>
        /// Gets or sets the date and time the picture was uploaded.
        /// </summary>
        [XmlAttribute]
        public DateTime Uploaded
        {
            get { return this.uploaded; }
            set { this.uploaded = value; }
        }

        #endregion
    }
}