using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JelleDruyts.Windows;
using Schedulr.Extensibility;
using Schedulr.Infrastructure;
using Schedulr.Models;
using Schedulr.Models.Legacy;
using Schedulr.Plugins.DeleteFile;
using Schedulr.Plugins.ImportMetadata;
using Schedulr.Plugins.MonitorFolders;

namespace Schedulr.Providers
{
    /// <summary>
    /// Provides the configuration for Schedulr.
    /// </summary>
    public static class SchedulrConfigurationProvider
    {
        #region Load

        /// <summary>
        /// Loads the configuration from the default file location, or creates a new configuration instance if the file does not exist or could not be loaded.
        /// </summary>
        /// <returns>The configuration that was loaded or created.</returns>
        public static SchedulrConfiguration LoadOrCreate()
        {
            var isNew = false;
            return LoadOrCreate(out isNew);
        }

        /// <summary>
        /// Loads the configuration from the default file location, or creates a new configuration instance if the file does not exist or could not be loaded.
        /// </summary>
        /// <param name="isNew">A value that determines if the configuration is new (i.e. created) or not (i.e. loaded from file).</param>
        /// <returns>The configuration that was loaded or created.</returns>
        public static SchedulrConfiguration LoadOrCreate(out bool isNew)
        {
            SchedulrConfiguration configuration = null;
            isNew = false;
            var configurationFileName = PathProvider.ConfigurationFilePath;
            if (File.Exists(configurationFileName))
            {
                try
                {
                    configuration = Load(PathProvider.ConfigurationFilePath);
                }
                catch (Exception exc)
                {
                    Logger.Log("An error occurred while loading the configuration", exc);
                }
            }
            if (configuration == null)
            {
                configuration = new SchedulrConfiguration();
                isNew = true;
            }
            return configuration;
        }

        /// <summary>
        /// Loads the configuration from the default file location.
        /// </summary>
        public static SchedulrConfiguration Load()
        {
            return Load(PathProvider.ConfigurationFilePath);
        }

        /// <summary>
        /// Loads the configuration from the specified file location.
        /// </summary>
        /// <param name="fileName">The name of the file that contains the configuration.</param>
        public static SchedulrConfiguration Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new ArgumentException("The specified configuration file does not exist: " + fileName);
            }

            try
            {
                Logger.Log("Loading configuration from file: " + fileName, TraceEventType.Information);
                var configuration = SerializationProvider.Read<SchedulrConfiguration>(fileName);
                Logger.Log("Loading configuration - done", TraceEventType.Verbose);

                // Instantiate certain objects that were added in newer versions of the application if they don't exist yet.
                if (configuration.UISettings == null)
                {
                    configuration.UISettings = new UISettings();
                }
                if (configuration.Plugins == null)
                {
                    configuration.Plugins = new ObservableCollection<PluginConfiguration>();
                }
                if (configuration.PluginCollectionSettings == null)
                {
                    configuration.PluginCollectionSettings = new ObservableCollection<PluginCollectionSettings>();
                }
                foreach (var account in configuration.Accounts)
                {
#pragma warning disable 618 // Disable warning about obsolete members
                    if (account.Settings.FoldersToMonitor == null)
                    {
                        account.Settings.FoldersToMonitor = new ObservableCollection<string>();
                    }
#pragma warning restore 618 // Restore warning about obsolete members
                    if (account.Settings.QueuedPictureDetailsUISettings == null)
                    {
                        account.Settings.QueuedPictureDetailsUISettings = new PictureDetailsUISettings();
                    }
                    if (account.Settings.UploadedPictureDetailsUISettings == null)
                    {
                        account.Settings.UploadedPictureDetailsUISettings = new PictureDetailsUISettings();
                    }
                    if (account.Settings.PictureDefaultsDetailsUISettings == null)
                    {
                        account.Settings.PictureDefaultsDetailsUISettings = new PictureDetailsUISettings();
                    }
                    if (account.Plugins == null)
                    {
                        account.Plugins = new ObservableCollection<PluginConfiguration>();
                    }
                    if (account.QueuedBatches == null)
                    {
                        account.QueuedBatches = new ObservableCollection<Batch>();
                    }
                    if (account.UploadedBatches == null)
                    {
                        account.UploadedBatches = new ObservableCollection<Batch>();
                    }

#pragma warning disable 618 // Disable warning about obsolete members
                    account.Settings.PictureDefaults.BatchId = null;
                    MigratePicturesToBatches(account.QueuedPictures, account.QueuedBatches);
                    MigratePicturesToBatches(account.UploadedPictures, account.UploadedBatches);
#pragma warning restore 618 // Restore warning about obsolete members
                }

                // Migrate obsolete settings to plugins if needed.
#pragma warning disable 618 // Disable warning about obsolete members
                foreach (var account in configuration.Accounts)
                {
                    if (account.Plugins.Count == 0)
                    {
                        if (account.Settings.FoldersToMonitor.Count > 0)
                        {
                            var pluginSettings = new MonitorFoldersPluginSettings();
                            foreach (var folderToMonitor in account.Settings.FoldersToMonitor)
                            {
                                pluginSettings.FoldersToMonitor.Add(folderToMonitor);
                            }
                            var eventId = PluginManager.GetEventId<AccountEventType>(AccountEventType.Activated);
                            var pluginTypeId = PluginManager.GetPluginTypeId(typeof(MonitorFoldersPlugin));
                            var pluginSettingsString = SerializationProvider.WriteToString<MonitorFoldersPluginSettings>(pluginSettings, false);
                            account.Plugins.Add(new PluginConfiguration(PluginManager.GetNewPluginInstanceId(), true, pluginSettingsString, eventId, pluginTypeId));
                            account.Settings.FoldersToMonitor.Clear();
                        }
                        if (account.Settings.DeleteFileAfterUpload)
                        {
                            var eventId = PluginManager.GetEventId<PictureEventType>(PictureEventType.Uploaded);
                            var pluginTypeId = PluginManager.GetPluginTypeId(typeof(DeleteFilePlugin));
                            string pluginSettingsString = null;
                            account.Plugins.Add(new PluginConfiguration(PluginManager.GetNewPluginInstanceId(), true, pluginSettingsString, eventId, pluginTypeId));
                            account.Settings.DeleteFileAfterUpload = false;
                        }
                        if (account.Settings.RetrieveMetadata)
                        {
                            AddPluginToRetrieveMetadata(account);
                        }
                    }
                }
#pragma warning restore 618 // Restore warning about obsolete members

                return configuration;
            }
            catch (Exception exc)
            {
                Logger.Log("Error while loading configuration", exc);
                throw;
            }
        }

        internal static void AddPluginToRetrieveMetadata(Account account)
        {
            var eventId = PluginManager.GetEventId<PictureEventType>(PictureEventType.Adding);
            var pluginTypeId = PluginManager.GetPluginTypeId(typeof(ImportMetadataPlugin));
            string pluginSettingsString = null;
            account.Plugins.Add(new PluginConfiguration(PluginManager.GetNewPluginInstanceId(), true, pluginSettingsString, eventId, pluginTypeId));
#pragma warning disable 618 // Disable warning about obsolete members
            account.Settings.RetrieveMetadata = false;
#pragma warning restore 618 // Restore warning about obsolete members
        }

        private static void MigratePicturesToBatches(ObservableCollection<Picture> pictures, ObservableCollection<Batch> batches)
        {
#pragma warning disable 618 // Disable warning about obsolete members
            foreach (var pictureBatch in pictures.GroupBy(p => p.BatchId))
            {
                var batch = new Batch(pictureBatch.First());
                foreach (var picture in pictureBatch)
                {
                    picture.BatchId = null;
                    batch.Pictures.Add(picture);
                }
                batches.Add(batch);
            }
#pragma warning restore 618 // Restore warning about obsolete members
            pictures.Clear();
        }

        #endregion

        #region Save

        /// <summary>
        /// Saves the configuration to the default file location.
        /// </summary>
        /// <param name="configuration">The configuration to save.</param>
        public static void Save(SchedulrConfiguration configuration)
        {
            Save(configuration, PathProvider.ConfigurationFilePath);
        }

        /// <summary>
        /// Saves the configuration to the specified file location.
        /// </summary>
        /// <param name="configuration">The configuration to save.</param>
        /// <param name="fileName">The name of the file to which to save the configuration.</param>
        public static void Save(SchedulrConfiguration configuration, string fileName)
        {
            try
            {
                Logger.Log("Saving configuration to file: " + fileName, TraceEventType.Information);
                SerializationProvider.Write<SchedulrConfiguration>(configuration, fileName);
            }
            catch (Exception exc)
            {
                Logger.Log("Error while saving configuration", exc);
                throw;
            }
        }

        #endregion

        #region ImportFromLegacyConfiguration

        /// <summary>
        /// Imports queued pictures from a legacy configuration file.
        /// </summary>
        /// <param name="account">The account into which to import the queued pictures.</param>
        /// <param name="fileName">The name of the legacy configuration file.</param>
        /// <returns>The number of queued pictures that were imported.</returns>
        public static int ImportFromLegacyConfiguration(Account account, string fileName)
        {
            try
            {
                Logger.Log("Loading legacy configuration from file: " + fileName, TraceEventType.Information);
                var legacyConfiguration = V1SchedulrConfiguration.Load(fileName);
                var pictureDefaults = account.Settings.PictureDefaults;
                var pictureCount = 0;
                Batch batch = null;
                var shouldCreateBatch = true;
                foreach (var legacyQueuedPicture in legacyConfiguration.QueuedPictures)
                {
                    pictureCount++;
                    var picture = new Picture
                    {
                        // Existing properties are taken from the legacy picture.
                        Description = legacyQueuedPicture.Description,
                        FileName = legacyQueuedPicture.FileName,
                        Tags = legacyQueuedPicture.Tags,
                        Title = legacyQueuedPicture.Title,
                        VisibilityIsFamily = legacyQueuedPicture.IsFamily,
                        VisibilityIsFriend = legacyQueuedPicture.IsFriend,
                        VisibilityIsPublic = legacyQueuedPicture.IsPublic,

                        // New properties are taken from the picture defaults.
                        ContentType = pictureDefaults.ContentType,
                        Safety = pictureDefaults.Safety,
                        License = pictureDefaults.License,
                        Location = pictureDefaults.Location,
                        SearchVisibility = pictureDefaults.SearchVisibility
                    };

                    // Properties that are only valid for uploaded pictures are set to null.
                    PictureProvider.InitializePicture(picture);

                    foreach (var legacyGroupId in legacyQueuedPicture.GroupIds)
                    {
                        picture.GroupIds.Add(legacyGroupId);
                    }
                    foreach (var legacySetId in legacyQueuedPicture.SetIds)
                    {
                        picture.SetIds.Add(legacySetId);
                    }

                    if (shouldCreateBatch)
                    {
                        batch = new Batch(picture);
                        account.QueuedBatches.Add(batch);
                    }
                    batch.Pictures.Add(picture);

                    // Create a new batch if the current picture should not be uploaded with the next.
                    shouldCreateBatch = !legacyQueuedPicture.UploadWithNext;
                }
                return pictureCount;
            }
            catch (Exception exc)
            {
                Logger.Log("Error while importing legacy configuration file", exc);
                throw;
            }
        }

        #endregion
    }
}