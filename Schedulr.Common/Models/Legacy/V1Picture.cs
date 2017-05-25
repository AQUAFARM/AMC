using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Schedulr.Models.Legacy
{
    /// <summary>
    /// Represents a picture.
    /// </summary>
    [Serializable]
    [XmlType(TypeName = "Picture")]
    public class V1Picture
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        [XmlAttribute]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        [XmlAttribute]
        public string Tags { get; set; }

        /// <summary>
        /// [Obsolete: Use SetIds property instead.] Gets or sets the photoset.
        /// </summary>
        [XmlAttribute]
        public string Photoset { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this picture is public.
        /// </summary>
        [XmlAttribute]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this picture is visible to family.
        /// </summary>
        [XmlAttribute]
        public bool IsFamily { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this picture is visible to friends.
        /// </summary>
        [XmlAttribute]
        public bool IsFriend { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [XmlElement]
        public string Title { get; set; }

        private string description;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [XmlElement]
        public string Description
        {
            get { return this.description; }
            set
            {
                this.description = value;

                // Fix up multiline strings (these are normalized by the XmlSerializer).
                if (this.description != null && !this.description.Contains("\r\n"))
                {
                    this.description = this.description.Replace("\n", "\r\n");
                }
            }
        }

        private string[] setIds = new string[0];

        /// <summary>
        /// Gets or sets the id's of the sets to associate with this picture.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), XmlAttribute]
        public string[] SetIds
        {
            get { return this.setIds; }
            set
            {
                // The XML serializer has some strange behavior regarding arrays where elements can be empty strings.
                // Loop over the deserialized entries and remove empty strings to fix that.
                var trimmedIds = new List<string>();
                if (value != null)
                {
                    foreach (var id in value)
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            trimmedIds.Add(id);
                        }
                    }
                }
                this.setIds = trimmedIds.ToArray();
            }
        }

        private string[] groupIds = new string[0];

        /// <summary>
        /// Gets or sets the id's of the groups to associate with this picture.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays"), XmlAttribute]
        public string[] GroupIds
        {
            get { return this.groupIds; }
            set
            {
                // The XML serializer has some strange behavior regarding arrays where elements can be empty strings.
                // Loop over the deserialized entries and remove empty strings to fix that.
                var trimmedIds = new List<string>();
                if (value != null)
                {
                    foreach (var id in value)
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            trimmedIds.Add(id);
                        }
                    }
                }
                this.groupIds = trimmedIds.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this picture should be uploaded together with the next picture in the queue.
        /// </summary>
        [XmlAttribute]
        public bool UploadWithNext { get; set; }

        #endregion
    }
}