using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Schedulr.Models.Legacy
{
    /// <summary>
    /// Represents the configuration for Schedulr.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = "http://schemas.jelle.druyts.net/Schedulr", ElementName = "SchedulrConfiguration")]
    public class V1SchedulrConfiguration
    {
        #region Properties

        private BindingList<V1Picture> queuedPictures = new BindingList<V1Picture>();

        /// <summary>
        /// Gets or sets the queued pictures.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public BindingList<V1Picture> QueuedPictures
        {
            get { return this.queuedPictures; }
        }

        private BindingList<V1UploadedPicture> uploadedPictures = new BindingList<V1UploadedPicture>();

        /// <summary>
        /// Gets or sets the uploaded pictures.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public BindingList<V1UploadedPicture> UploadedPictures
        {
            get { return this.uploadedPictures; }
        }

        #endregion

        #region Load

        /// <summary>
        /// Loads the configuration from file.
        /// </summary>
        /// <param name="fileName">The name of the file that contains the configuration.</param>
        public static V1SchedulrConfiguration Load(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(V1SchedulrConfiguration));
            using (FileStream stream = File.OpenRead(fileName))
            {
                V1SchedulrConfiguration configuration = (V1SchedulrConfiguration)serializer.Deserialize(stream);
                return configuration;
            }
        }

        #endregion
    }
}