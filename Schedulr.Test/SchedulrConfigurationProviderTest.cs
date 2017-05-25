using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schedulr.Models;
using Schedulr.Providers;

namespace Schedulr.Test
{
    [TestClass]
    public class SchedulrConfigurationProviderTest
    {
        private const string TestConfigurationFile = "TestSchedulrConfiguration.xml";
        private const string TestConfigurationFileFromV10 = "Configuration-v1.4.xml";
        private const string TestConfigurationFileFromV20 = "Configuration-v2.0.xml";
        private const string TestConfigurationFileFromV24 = "Configuration-v2.4.xml";

        [TestMethod]
        public void SaveWritesAndLoadsConfigurationFile()
        {
            var writeConfig = GetTestConfiguration();

            // Save the configuration.
            Assert.IsFalse(File.Exists(TestConfigurationFile));
            SchedulrConfigurationProvider.Save(writeConfig);
            Assert.IsTrue(File.Exists(TestConfigurationFile));

            // Read the saved configuration and compare.
            var readConfig = SchedulrConfigurationProvider.Load();
            Verifier.VerifySchedulrConfiguration(writeConfig, readConfig);

            // Clean up.
            File.Delete(TestConfigurationFile);
        }

        [TestMethod]
        public void LoadOrCreateLoadsOrCreatesConfiguration()
        {
            // If the configuration file does not exist, creates a new one.
            Assert.IsFalse(File.Exists(TestConfigurationFile));
            var createdConfig = SchedulrConfigurationProvider.LoadOrCreate();
            Assert.IsNotNull(createdConfig);
            Assert.AreEqual<int>(0, createdConfig.Accounts.Count);

            // Save the test configuration to force a file with multiple accounts to be present.
            var testConfig = GetTestConfiguration();
            SchedulrConfigurationProvider.Save(testConfig);

            // Now the configuration file exists so it should be loaded.
            Assert.IsTrue(File.Exists(TestConfigurationFile));
            var loadedConfig = SchedulrConfigurationProvider.LoadOrCreate();
            Assert.IsNotNull(loadedConfig);
            Assert.AreEqual<int>(testConfig.Accounts.Count, loadedConfig.Accounts.Count);

            // Now make the file unreadable as a configuration file.
            File.WriteAllText(TestConfigurationFile, "Dummy");
            var fallbackConfig = SchedulrConfigurationProvider.LoadOrCreate();
            Assert.IsNotNull(fallbackConfig);
            Assert.AreEqual<int>(0, fallbackConfig.Accounts.Count);

            // Clean up.
            File.Delete(TestConfigurationFile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadThrowsOnNullFileName()
        {
            SchedulrConfigurationProvider.Load(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadThrowsOnEmptyFileName()
        {
            SchedulrConfigurationProvider.Load(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadThrowsOnNonExistentFileName()
        {
            SchedulrConfigurationProvider.Load(Util.GetDummyFileName());
        }

        [TestMethod]
        [DeploymentItem(TestConfigurationFileFromV10)]
        public void LoadImportsConfigurationFromV1()
        {
            var account = new Account();
            var count = SchedulrConfigurationProvider.ImportFromLegacyConfiguration(account, TestConfigurationFileFromV10);
            var equivalentBatches = GetEquivalentBatchesFromV1();
            Assert.AreEqual<int>(equivalentBatches.SelectMany(b => b.Pictures).Count(), count);
            Assert.AreEqual<int>(0, account.UploadedBatches.Count); // Uploaded pictures should NOT be imported.
#pragma warning disable 618 // Disable warning about obsolete members
            Assert.AreEqual<int>(0, account.UploadedPictures.Count); // Legacy picture collections should always be empty.
            Assert.AreEqual<int>(0, account.QueuedPictures.Count); // Legacy picture collections should always be empty.
#pragma warning restore 618 // Restore warning about obsolete members
            Verifier.VerifyBatches(equivalentBatches, account.QueuedBatches);
        }

        [TestMethod]
        [DeploymentItem(TestConfigurationFileFromV20)]
        public void LoadImportsConfigurationFromV2()
        {
            // Verify static config file with in-memory config (making sure that file format stays compatible over time).
            var testConfig = GetTestConfiguration();
            var readConfig = SchedulrConfigurationProvider.Load(TestConfigurationFileFromV20);
            Verifier.VerifySchedulrConfiguration(testConfig, readConfig);
        }

        [TestMethod]
        [DeploymentItem(TestConfigurationFileFromV24)]
        public void LoadReplacesObsoleteSettingsWithPlugins()
        {
#pragma warning disable 618 // Disable warning about obsolete members
            var readConfig = SchedulrConfigurationProvider.Load(TestConfigurationFileFromV24);
            var account = readConfig.Accounts.First();
            Assert.AreEqual<int>(3, account.Plugins.Count);
            Assert.AreEqual<int>(0, account.Settings.FoldersToMonitor.Count);
            Assert.AreEqual<bool>(false, account.Settings.DeleteFileAfterUpload);
            Assert.AreEqual<bool>(false, account.Settings.RetrieveMetadata);
            var monitorFoldersPluginConfiguration = account.Plugins.FirstOrDefault(p => p.PluginTypeId.IndexOf("MonitorFoldersPlugin", StringComparison.OrdinalIgnoreCase) > 0);
            Assert.IsNotNull(monitorFoldersPluginConfiguration);
            Assert.IsTrue(monitorFoldersPluginConfiguration.IsEnabled);
            Assert.IsTrue(monitorFoldersPluginConfiguration.Settings.IndexOf(@"c:\foldertomonitor1", StringComparison.OrdinalIgnoreCase) > 0);
            Assert.IsTrue(monitorFoldersPluginConfiguration.Settings.IndexOf(@"c:\foldertomonitor2", StringComparison.OrdinalIgnoreCase) > 0);
            var deleteFilePluginConfiguration = account.Plugins.FirstOrDefault(p => p.PluginTypeId.IndexOf("DeleteFilePlugin", StringComparison.OrdinalIgnoreCase) > 0);
            Assert.IsNotNull(deleteFilePluginConfiguration);
            Assert.IsTrue(deleteFilePluginConfiguration.IsEnabled);
            var importMetadataPluginConfiguration = account.Plugins.FirstOrDefault(p => p.PluginTypeId.IndexOf("ImportMetadataPlugin", StringComparison.OrdinalIgnoreCase) > 0);
            Assert.IsNotNull(importMetadataPluginConfiguration);
            Assert.IsTrue(deleteFilePluginConfiguration.IsEnabled);
#pragma warning restore 618 // Restore warning about obsolete members
        }

        private static SchedulrConfiguration GetTestConfiguration()
        {
            var firstQueuedPicture = new Picture
            {
                Title = "title-1",
                Description = "description-1. This description spans\r\nmultiple lines\r\n\r\nwhich should be supported!",
                Tags = "tags-1",
                VisibilityIsFamily = true,
                VisibilityIsFriend = true,
                VisibilityIsPublic = true,
                Safety = Safety.Safe,
                ContentType = ContentType.Photo,
                License = License.AttributionCC,
                GroupIds = { "group-id-2", "group-id-3" },
                SetIds = { "set-id-1" }
            };
            var firstUploadedPicture = new Picture
            {
                Title = "title-3",
                Description = "description-3. This description spans\r\nmultiple lines\r\n\r\nwhich should be supported!",
                Tags = "tags-3",
                VisibilityIsFamily = true,
                VisibilityIsFriend = false,
                VisibilityIsPublic = true,
                Safety = Safety.None,
                ContentType = ContentType.Screenshot,
                License = License.AttributionNoncommercialCC,
                GroupIds = { "group-id-1", "group-id-3" },
                SetIds = { "set-id-2" },
                DateUploaded = new DateTimeOffset(2010, 06, 28, 20, 51, 23, TimeSpan.FromHours(2)).DateTime
            };
            return new SchedulrConfiguration
            {
                Accounts =
                {
                    new Account
                    {
                        Id = "John-ID",
                        Name = "John",
                        IsDefaultAccount = true,
                        AuthenticationToken = "token-john",
                        QueuedBatches = new ObservableCollection<Batch>
                        {
                            new Batch
                            {
                                IsExpanded = true,
                                Photoset = new Photoset
                                {
                                    PrimaryPictureId = firstQueuedPicture.FileName
                                },
                                CreatePhotosetForBatch = false,
                                Pictures = new BulkObservableCollection<Picture>
                                {
                                    firstQueuedPicture,
                                    new Picture
                                    {
                                        Title = "title-2",
                                        Description = "description-2. This description spans\r\nmultiple lines\r\n\r\nwhich should be supported!",
                                        Tags = "tags-2",
                                        VisibilityIsFamily = false,
                                        VisibilityIsFriend = false,
                                        VisibilityIsPublic = false,
                                        Safety = Safety.Restricted,
                                        ContentType = ContentType.None,
                                        License = License.AttributionNoDerivativesCC,
                                        SetIds = { "set-id-1", "set-id-2" }
                                    }
                                }
                            }
                        },
                        UploadedBatches = new ObservableCollection<Batch>
                        {
                            new Batch
                            {
                                IsExpanded = true,
                                Photoset = new Photoset
                                {
                                    PrimaryPictureId = firstUploadedPicture.FileName
                                },
                                CreatePhotosetForBatch = false,
                                Pictures = new BulkObservableCollection<Picture>
                                {
                                    firstUploadedPicture
                                }
                            }
                        },
                        UserInfo = new UserInfo
                        {
                            UserName = "John",
                            PicturesUrl = "http://www.flickr.com/photos/john",
                            BuddyIconUrl = "http://www.flickr.com/photos/john/buddy-icon",
                            IsProUser = true,
                            UploadLimit = 10 * 1024 * 1024,
                            Groups = 
                            {
                                new PictureCollection { Id = "group-id-1", Name = "group-name-1", ImageUrl = "group-image-1" },
                                new PictureCollection { Id = "group-id-2", Name = "group-name-2", ImageUrl = "group-image-2" },
                                new PictureCollection { Id = "group-id-3", Name = "group-name-3", ImageUrl = "group-image-3" }
                            },
                            Sets = 
                            {
                                new PictureCollection { Id = "set-id-1", Name = "set-name-1", ImageUrl = "set-image-1" },
                                new PictureCollection { Id = "set-id-2", Name = "set-name-2", ImageUrl = "set-image-2" }
                            }
                        },
                        Settings = new AccountSettings
                        {
#pragma warning disable 618 // Disable warning about obsolete members
                            DeleteFileAfterUpload = false,
                            RetrieveMetadata = true,
#pragma warning restore 618 // Restore warning about obsolete members
                            PictureDefaults = new Picture
                            {
                                Title = "default-title",
                                Description = "default-description. This description spans\r\nmultiple lines\r\n\r\nwhich should be supported!",
                                Tags = "default-tags",
                                VisibilityIsFamily = true,
                                VisibilityIsFriend = false,
                                VisibilityIsPublic = true,
                                Safety = Safety.Safe,
                                ContentType = ContentType.Photo,
                                License = License.AttributionNoncommercialNoDerivativesCC,
                                GroupIds = { "group-id-2", "group-id-3" },
                                SetIds = { "set-id-1" }
                            }
                        },
                        UploadSchedule = new UploadSchedule
                        {
                            Type = ScheduleType.Daily,
                            StartTime = new DateTimeOffset(2010, 06, 30, 19, 00, 00, TimeSpan.FromHours(2)).DateTime,
                            OnlyIfLoggedOn = false,
                            WakeComputer = true,
                            OnlyIfNotOnBatteryPower = true,
                            HourlyInterval = 2,
                            HourlyIntervalDuration = 4,
                            DailyInterval = 3,
                            WeeklyInterval = 4,
                            WeeklyOnMonday = false,
                            WeeklyOnTuesday = true,
                            WeeklyOnWednesday = false,
                            WeeklyOnThursday = true,
                            WeeklyOnFriday = false,
                            WeeklyOnSaturday = true,
                            WeeklyOnSunday = false
                        }
                    },
                    new Account
                    {
                        Id = "Mary-ID",
                        Name = "Mary",
                        IsDefaultAccount = false,
                        AuthenticationToken = "token-mary",
                        UserInfo = new UserInfo
                        {
                            UserName = "Mary",
                            PicturesUrl = "http://www.flickr.com/photos/mary",
                            BuddyIconUrl = "http://www.flickr.com/photos/mary/buddy-icon",
                            IsProUser = false,
                            UploadLimit = long.MaxValue,
                            Groups = 
                            {
                                new PictureCollection { Id = "group-id-2", Name = "group-name-2", ImageUrl = "group-image-2" },
                                new PictureCollection { Id = "group-id-4", Name = "group-name-4", ImageUrl = "group-image-4" }
                            },
                        },
                        Settings = new AccountSettings
                        {
#pragma warning disable 618 // Disable warning about obsolete members
                            DeleteFileAfterUpload = true,
                            RetrieveMetadata = false,
#pragma warning restore 618 // Restore warning about obsolete members
                            PictureDefaults = new Picture
                            {
                                Title = "default-title-2",
                                Description = "default-description-2. This description spans\r\nmultiple lines\r\n\r\nwhich should be supported!",
                                Tags = "default-tags-2",
                                VisibilityIsFamily = false,
                                VisibilityIsFriend = true,
                                VisibilityIsPublic = false,
                                Safety = Safety.Restricted,
                                ContentType = ContentType.Other,
                                License = License.AllRightsReserved,
                                GroupIds = { "group-id-4" }
                            }
                        },
                        UploadSchedule = new UploadSchedule
                        {
                            Type = ScheduleType.Weekly,
                            StartTime = new DateTimeOffset(2010, 07, 01, 03, 00, 00, TimeSpan.FromHours(2)).DateTime,
                            OnlyIfLoggedOn = true,
                            WakeComputer = false,
                            OnlyIfNotOnBatteryPower = false,
                            HourlyInterval = 6,
                            HourlyIntervalDuration = 8,
                            DailyInterval = 7,
                            WeeklyInterval = 8,
                            WeeklyOnMonday = true,
                            WeeklyOnTuesday = false,
                            WeeklyOnWednesday = true,
                            WeeklyOnThursday = false,
                            WeeklyOnFriday = true,
                            WeeklyOnSaturday = false,
                            WeeklyOnSunday = true
                        }
                    }
                }
            };
        }

        private static ObservableCollection<Batch> GetEquivalentBatchesFromV1()
        {
            var b1p1 = new Picture
            {
                FileName = @"C:\Users\Schedulr\Pictures\Upload\First Day.jpg",
                Tags = "\"Day 1\" Travel Train",
                VisibilityIsPublic = true,
                VisibilityIsFamily = false,
                VisibilityIsFriend = false,
                SetIds = { "set-1" },
                GroupIds = { "group-1" },
                Title = "Arrival",
                Description = @"This was a great first day.

Really!",
#pragma warning disable 618 // Disable warning about obsolete members
                BatchId = null
#pragma warning restore 618 // Restore warning about obsolete members
            };
            var b2p1 = new Picture
            {
                FileName = @"C:\Users\Schedulr\Pictures\Upload\Second Day 1.jpg",
                Tags = "\"Day 2\" Travel Pool",
                VisibilityIsPublic = false,
                VisibilityIsFamily = true,
                VisibilityIsFriend = false,
                SetIds = { "set-2" },
                GroupIds = { "group-2" },
                Title = "Swimming Pool",
                Description = "Chilling in the pool!",
#pragma warning disable 618 // Disable warning about obsolete members
                BatchId = null
#pragma warning restore 618 // Restore warning about obsolete members
            };
            var b3p1 = new Picture
            {
                FileName = @"C:\Users\Schedulr\Pictures\Upload\Third Day 1.jpg",
                Tags = "\"Day 3\" Travel City",
                VisibilityIsPublic = false,
                VisibilityIsFamily = false,
                VisibilityIsFriend = true,
                SetIds = { "set-3" },
                GroupIds = { "group-3" },
                Title = "City Tripping",
                Description = "Checking out the city.",
#pragma warning disable 618 // Disable warning about obsolete members
                BatchId = null
#pragma warning restore 618 // Restore warning about obsolete members
            };
            var b4p1 = new Picture
            {
                FileName = @"C:\Users\Schedulr\Pictures\Upload\Fourth Day.jpg",
                Tags = "\"Day 4\" Travel Plane",
                VisibilityIsPublic = true,
                VisibilityIsFamily = true,
                VisibilityIsFriend = true,
                SetIds = { "set-1", "set-2", "set-3", "set-4" },
                GroupIds = { "group-1", "group-2", "group-3", "group-4" },
                Title = "Departure",
                Description = string.Empty,
#pragma warning disable 618 // Disable warning about obsolete members
                BatchId = null
#pragma warning restore 618 // Restore warning about obsolete members
            };
            // Create the equivalent of the batches in the legacy v1 config file.
            return new ObservableCollection<Batch>
            {
                new Batch
                {
                    IsExpanded = true,
                    Photoset = new Photoset
                    {
                        PrimaryPictureId = b1p1.FileName
                    },
                    Pictures = new BulkObservableCollection<Picture>
                    {
                        b1p1
                    }
                },
                new Batch
                {
                    IsExpanded = true,
                    Photoset = new Photoset
                    {
                        PrimaryPictureId = b2p1.FileName
                    },
                    Pictures = new BulkObservableCollection<Picture>
                    {
                        b2p1,
                        new Picture
                        {
                            FileName = @"C:\Users\Schedulr\Pictures\Upload\Second Day 2.jpg",
                            Tags = "\"Day 2\" Travel Beach",
                            VisibilityIsPublic = false,
                            VisibilityIsFamily = true,
                            VisibilityIsFriend = false,
                            SetIds = { "set-2" },
                            GroupIds = { "group-2" },
                            Title = "Beach",
                            Description = "Sunbathing on the beach...",
#pragma warning disable 618 // Disable warning about obsolete members
                            BatchId = null
#pragma warning restore 618 // Restore warning about obsolete members
                        }
                    }
                },
                new Batch
                {
                    IsExpanded = true,
                    Photoset = new Photoset
                    {
                        PrimaryPictureId = b3p1.FileName
                    },
                    Pictures = new BulkObservableCollection<Picture>
                    {
                        b3p1,
                        new Picture
                        {
                            FileName = @"C:\Users\Schedulr\Pictures\Upload\Third Day 2.jpg",
                            Tags = "\"Day 3\" Travel Museum",
                            VisibilityIsPublic = false,
                            VisibilityIsFamily = false,
                            VisibilityIsFriend = true,
                            SetIds = { "set-3" },
                            GroupIds = { "group-3" },
                            Title = "Museum",
                            Description = "A bit of culture.",
#pragma warning disable 618 // Disable warning about obsolete members
                            BatchId = null
#pragma warning restore 618 // Restore warning about obsolete members
                        }
                    }
                },
                new Batch
                {
                    IsExpanded = true,
                    Photoset = new Photoset
                    {
                        PrimaryPictureId = b4p1.FileName
                    },
                    Pictures = new BulkObservableCollection<Picture>
                    {
                        b4p1
                    }
                }
            };
        }
    }
}