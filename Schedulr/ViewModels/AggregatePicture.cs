using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using JelleDruyts.Windows;
using JelleDruyts.Windows.ObjectModel;
using Schedulr.Models;

namespace Schedulr.ViewModels
{
    /// <summary>
    /// An aggregated picture that gets and sets property values from a collection of actual <see cref="Picture"/> instances.
    /// </summary>
    public class AggregatePicture : Picture
    {
        #region Properties

        /// <summary>
        /// Gets the aggregated pictures.
        /// </summary>
        public IList<Picture> AggregatedPictures { get; private set; }

        /// <summary>
        /// Gets or sets the first aggregated picture.
        /// </summary>
        private Picture FirstPicture { get; set; }

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets the name of the files (each enclosed by double quotes and separated by a space).
        /// </summary>
        public override string FileName
        {
            get
            {
                var fileNames = new StringBuilder();
                foreach (var picture in this.AggregatedPictures)
                {
                    if (fileNames.Length > 0)
                    {
                        fileNames.Append(" ");
                    }
                    fileNames.AppendFormat(CultureInfo.CurrentCulture, "\"{0}\"", picture.FileName);
                }
                return fileNames.ToString();
            }
            set { throw new InvalidOperationException("This property should not be called on an AggregatePicture."); }
        }

        /// <summary>
        /// Gets or sets the tags for the aggregated pictures (the getter always returns <see langword="null"/>, the setter adds the given value to all pictures' tags).
        /// </summary>
        public override string Tags
        {
            get
            {
                // Always leave the tags null since they will be added (not just edited) when dealing with multiple pictures.
                return null;
            }
            set
            {
                // Do not simply set the value but add it to the Tags property of all pictures.
                if (!string.IsNullOrWhiteSpace(value))
                {
                    foreach (var picture in this.AggregatedPictures)
                    {
                        var currentTags = picture.Tags;
                        if (currentTags != null)
                        {
                            currentTags = currentTags.Trim();
                        }
                        picture.Tags = (currentTags + " " + value.Trim()).Trim();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pictures are public.
        /// </summary>
        public override bool? VisibilityIsPublic
        {
            get
            {
                return GetAll(VisibilityIsPublicProperty);
            }
            set
            {
                SetAll(VisibilityIsPublicProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pictures are visible to family.
        /// </summary>
        public override bool? VisibilityIsFamily
        {
            get
            {
                return GetAll(VisibilityIsFamilyProperty);
            }
            set
            {
                SetAll(VisibilityIsFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pictures are visible to friends.
        /// </summary>
        public override bool? VisibilityIsFriend
        {
            get
            {
                return GetAll(VisibilityIsFriendProperty);
            }
            set
            {
                SetAll(VisibilityIsFriendProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the title of all aggregated pictures.
        /// </summary>
        public override string Title
        {
            get
            {
                return GetAll(TitleProperty);
            }
            set
            {
                SetAll(TitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the description of all aggregated pictures.
        /// </summary>
        public override string Description
        {
            get
            {
                return GetAll(DescriptionProperty);
            }
            set
            {
                SetAll(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the date and time the pictures were uploaded, or <see langword="null"/> if the pictures haven't been uploaded yet.
        /// </summary>
        public override DateTime? DateUploaded
        {
            get
            {
                return GetAll(DateUploadedProperty);
            }
            set
            {
                SetAll(DateUploadedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the safety rating of this picture.
        /// </summary>
        public override Safety? Safety
        {
            get
            {
                return GetAll(SafetyProperty);
            }
            set
            {
                SetAll(SafetyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content type of this picture.
        /// </summary>
        public override ContentType? ContentType
        {
            get
            {
                return GetAll(ContentTypeProperty);
            }
            set
            {
                SetAll(ContentTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the license of this picture.
        /// </summary>
        public override License? License
        {
            get
            {
                return GetAll(LicenseProperty);
            }
            set
            {
                SetAll(LicenseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the geographic location where the picture was taken.
        /// </summary>
        public override GeoLocation Location
        {
            get
            {
                return GetAll(LocationProperty);
            }
            set
            {
                SetAll(LocationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the picture in the global search results.
        /// </summary>
        public override SearchVisibility? SearchVisibility
        {
            get
            {
                return GetAll(SearchVisibilityProperty);
            }
            set
            {
                SetAll(SearchVisibilityProperty, value);
            }
        }

        #endregion

        #region Obsolete Properties

#pragma warning disable 618 // Disable warning about obsolete members

        /// <summary>
        /// Gets or sets the ID of the upload batch the aggregated pictures belong to.
        /// </summary>
        /// <value></value>
        [System.ComponentModel.Browsable(false)]
        [Obsolete("This property is obsolete and has been replaced by the Batch class.")]
        public override string BatchId
        {
            get
            {
                return GetAll(BatchIdProperty);
            }
            set
            {
                SetAll(BatchIdProperty, value);
            }
        }

#pragma warning restore 618 // Restore warning about obsolete members

        #endregion

        #region Derived Properties

        /// <summary>
        /// Gets the short name of the file.
        /// </summary>
        public override string ShortFileName
        {
            get { throw new InvalidOperationException("This property should not be called on an AggregatePicture."); }
        }

        /// <summary>
        /// Gets the size of the file in bytes.
        /// </summary>
        public override long FileSize
        {
            get
            {
                return this.AggregatedPictures.Sum(p => p.FileSize);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatePicture"/> class.
        /// </summary>
        /// <param name="pictures">The pictures to aggregate.</param>
        public AggregatePicture(IList<Picture> pictures)
        {
            if (pictures == null)
            {
                throw new ArgumentNullException("pictures");
            }
            if (pictures.Count < 2)
            {
                throw new ArgumentException("There must be more than one picture to aggregate.");
            }
            this.AggregatedPictures = pictures;
            this.FirstPicture = this.AggregatedPictures[0];

            // Listen for changes in the source collections to update the aggregate.
            foreach (var picture in this.AggregatedPictures)
            {
                picture.GroupIds.CollectionChanged += (sender, e) => UpdateAggregatedSetsAndGroups();
                picture.SetIds.CollectionChanged += (sender, e) => UpdateAggregatedSetsAndGroups();
            }

            // Aggregate the collections now.
            UpdateAggregatedSetsAndGroups();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets a value that determines if all aggregated pictures have the specified Set ID.
        /// </summary>
        /// <param name="setId">The Set ID.</param>
        /// <returns><see langword="true"/> if all aggregated pictures have the specified Set ID, <see langword="false"/> if none have it, or <see langword="null"/> if some have it.</returns>
        public bool? SetIdsContains(string setId)
        {
            if (this.AggregatedPictures.All(p => p.SetIds.Contains(setId)))
            {
                return true;
            }
            else if (this.AggregatedPictures.All(p => !p.SetIds.Contains(setId)))
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets a value that determines if all aggregated pictures have the specified Group ID.
        /// </summary>
        /// <param name="groupId">The Group ID.</param>
        /// <returns><see langword="true"/> if all aggregated pictures have the specified Group ID, <see langword="false"/> if none have it, or <see langword="null"/> if some have it.</returns>
        public bool? GroupIdsContains(string groupId)
        {
            if (this.AggregatedPictures.All(p => p.GroupIds.Contains(groupId)))
            {
                return true;
            }
            else if (this.AggregatedPictures.All(p => !p.GroupIds.Contains(groupId)))
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called whenever the <see cref="Picture.GroupIds"/> collection changed on this aggregate picture.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void GroupIdsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                AddToPictureCollections(p => p.GroupIds, e.NewItems);
                RemoveFromPictureCollections(p => p.GroupIds, e.OldItems);
            }
            UpdateAggregatedSetsAndGroups();
        }

        /// <summary>
        /// Called whenever the <see cref="Picture.SetIds"/> collection changed on this aggregate picture.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void SetIdsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                AddToPictureCollections(p => p.SetIds, e.NewItems);
                RemoveFromPictureCollections(p => p.SetIds, e.OldItems);
            }
            UpdateAggregatedSetsAndGroups();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the aggregated value from all pictures.
        /// </summary>
        /// <typeparam name="T">The type of the aggregate value.</typeparam>
        /// <param name="property">The property to aggregate.</param>
        /// <returns>The value if all pictures have the same value, or the property type's default value if they are not all the same.</returns>
        private T GetAll<T>(ObservableProperty<T> property)
        {
            var allSame = this.AggregatedPictures.All(p => object.Equals(p.GetValue<T>(property), this.FirstPicture.GetValue<T>(property)));
            return (allSame ? this.FirstPicture.GetValue<T>(property) : default(T));
        }

        /// <summary>
        /// Sets a property value on all aggregated pictures.
        /// </summary>
        /// <typeparam name="T">The type of the aggregate value.</typeparam>
        /// <param name="property">The property to aggregate.</param>
        /// <param name="value">The value to set on all aggregated pictures.</param>
        private void SetAll<T>(ObservableProperty<T> property, T value)
        {
            var anyChanged = false;
            foreach (var picture in this.AggregatedPictures)
            {
                var currentChanged = picture.SetValue(property, value);
                if (currentChanged)
                {
                    anyChanged = true;
                }
            }
            if (anyChanged)
            {
                OnPropertyChanged(property.Name);
            }
        }

        /// <summary>
        /// Updates the aggregated sets and groups.
        /// </summary>
        private void UpdateAggregatedSetsAndGroups()
        {
            // Stop listening for changes in the aggregate collections (since they are about to be modified).
            this.GroupIds.CollectionChanged -= new NotifyCollectionChangedEventHandler(GroupIdsChanged);
            this.SetIds.CollectionChanged -= new NotifyCollectionChangedEventHandler(SetIdsChanged);

            // Aggregate all common Group and Set ID's.
            IEnumerable<string> groupIds = this.FirstPicture.GroupIds;
            IEnumerable<string> setIds = this.FirstPicture.SetIds;
            foreach (var picture in this.AggregatedPictures)
            {
                groupIds = groupIds.Union(picture.GroupIds);
                setIds = setIds.Union(picture.SetIds);
            }

            // Update the collections with the aggregate values.
            this.GroupIds.ReplaceItems(groupIds);
            this.SetIds.ReplaceItems(setIds);

            // Listen for changes in the aggregate collections to update the source pictures.
            this.GroupIds.CollectionChanged += new NotifyCollectionChangedEventHandler(GroupIdsChanged);
            this.SetIds.CollectionChanged += new NotifyCollectionChangedEventHandler(SetIdsChanged);
        }

        /// <summary>
        /// Adds a collection of ID's to picture collections.
        /// </summary>
        /// <param name="collectionSelector">The collection selector.</param>
        /// <param name="ids">The ID's to add.</param>
        private void AddToPictureCollections(Func<Picture, ObservableCollection<string>> collectionSelector, IList ids)
        {
            if (ids != null && ids.Count > 0)
            {
                foreach (var picture in this.AggregatedPictures)
                {
                    var collection = collectionSelector(picture);
                    foreach (string id in ids.Cast<string>())
                    {
                        if (!collection.Contains(id))
                        {
                            collection.Add(id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes a collection of ID's from picture collections.
        /// </summary>
        /// <param name="collectionSelector">The collection selector.</param>
        /// <param name="ids">The ID's to remove.</param>
        private void RemoveFromPictureCollections(Func<Picture, ObservableCollection<string>> collectionSelector, IList ids)
        {
            if (ids != null && ids.Count > 0)
            {
                foreach (var picture in this.AggregatedPictures)
                {
                    var collection = collectionSelector(picture);
                    foreach (string id in ids.Cast<string>())
                    {
                        if (collection.Contains(id))
                        {
                            collection.Remove(id);
                        }
                    }
                }
            }
        }

        #endregion
    }
}