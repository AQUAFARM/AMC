using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schedulr.Models;

namespace Schedulr.Test
{
    internal static class Verifier
    {
        public static void VerifySchedulrConfiguration(SchedulrConfiguration expected, SchedulrConfiguration actual)
        {
            // Verify configuration.
            Assert.IsNotNull(actual);

            // Verify Accounts.
            Assert.IsNotNull(actual.Accounts);
            Assert.AreEqual<int>(expected.Accounts.Count, actual.Accounts.Count);
            for (int i = 0; i < expected.Accounts.Count; i++)
            {
                // Verify Account.
                var expectedAccount = expected.Accounts[i];
                var actualAccount = actual.Accounts[i];
                VerifyAccount(expectedAccount, actualAccount);
            }
        }

        public static void VerifyAccount(Account expected, Account actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual<string>(expected.Id, actual.Id);
            Assert.AreEqual<string>(expected.Name, actual.Name);
            Assert.AreEqual<bool>(expected.IsDefaultAccount, actual.IsDefaultAccount);
            Assert.AreEqual<string>(expected.AuthenticationToken, actual.AuthenticationToken);
            VerifyUserInfo(expected.UserInfo, actual.UserInfo);
#pragma warning disable 618 // Disable warning about obsolete members
            VerifyPictures(expected.QueuedPictures, actual.QueuedPictures);
            VerifyPictures(expected.UploadedPictures, actual.UploadedPictures);
#pragma warning restore 618 // Restore warning about obsolete members
            VerifyBatches(expected.QueuedBatches, actual.QueuedBatches);
            VerifyBatches(expected.UploadedBatches, actual.UploadedBatches);
            VerifyAccountSettings(expected.Settings, actual.Settings);
            VerifyUploadSchedule(expected.UploadSchedule, actual.UploadSchedule);
        }

        public static void VerifyPictures(ObservableCollection<Picture> expected, ObservableCollection<Picture> actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual<int>(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                // Verify item.
                VerifyPicture(expected[i], actual[i]);
            }
        }

        public static void VerifyBatches(ObservableCollection<Batch> expected, ObservableCollection<Batch> actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual<int>(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                // Verify item.
                VerifyBatch(expected[i], actual[i]);
            }
        }

        public static void VerifyBatch(Batch expected, Batch actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual<bool>(expected.CreatePhotosetForBatch, actual.CreatePhotosetForBatch);
            Assert.AreEqual<bool>(expected.IsExpanded, actual.IsExpanded);
            VerifyPhotoset(expected.Photoset, actual.Photoset);
            VerifyPictures(expected.Pictures, actual.Pictures);
        }

        private static void VerifyPhotoset(Photoset expected, Photoset actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual<string>(expected.Description, actual.Description);
            Assert.AreEqual<string>(expected.Id, actual.Id);
            Assert.AreEqual<string>(expected.ImageUrl, actual.ImageUrl);
            Assert.AreEqual<string>(expected.Name, actual.Name);
            Assert.AreEqual<string>(expected.PrimaryPictureId, actual.PrimaryPictureId);
        }

        public static void VerifyAccountSettings(AccountSettings expected, AccountSettings actual)
        {
            Assert.IsNotNull(actual);
#pragma warning disable 618 // Disable warning about obsolete members
            Assert.AreEqual<bool>(false, actual.DeleteFileAfterUpload); // Obsolete properties are always reset.
            Assert.AreEqual<bool>(false, actual.RetrieveMetadata); // Obsolete properties are always reset.
#pragma warning restore 618 // Restore warning about obsolete members
            Assert.IsNotNull(actual.PictureDefaults);
            VerifyPicture(expected.PictureDefaults, actual.PictureDefaults);
        }

        public static void VerifyPicture(Picture expected, Picture actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual<string>(expected.Description, actual.Description);
            Assert.AreEqual<string>(expected.FileName, actual.FileName);
            Assert.AreEqual<long>(expected.FileSize, actual.FileSize);
#pragma warning disable 618 // Disable warning about obsolete members
            Assert.AreEqual<string>(expected.BatchId, actual.BatchId);
#pragma warning restore 618 // Restore warning about obsolete members
            Assert.AreEqual<string>(expected.ShortFileName, actual.ShortFileName);
            Assert.AreEqual<string>(expected.Tags, actual.Tags);
            Assert.AreEqual<string>(expected.Title, actual.Title);
            Assert.AreEqual<bool?>(expected.VisibilityIsFamily, actual.VisibilityIsFamily);
            Assert.AreEqual<bool?>(expected.VisibilityIsFriend, actual.VisibilityIsFriend);
            Assert.AreEqual<bool?>(expected.VisibilityIsPublic, actual.VisibilityIsPublic);
            Assert.AreEqual<Safety?>(expected.Safety, actual.Safety);
            Assert.AreEqual<ContentType?>(expected.ContentType, actual.ContentType);
            Assert.AreEqual<License?>(expected.License, actual.License);
            Assert.AreEqual<DateTimeOffset?>(expected.DateUploaded, actual.DateUploaded);
            VerifyList<string>(expected.SetIds, actual.SetIds);
            VerifyList<string>(expected.GroupIds, actual.GroupIds);
        }

        public static void VerifyUserInfo(UserInfo expected, UserInfo actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual<string>(expected.UserName, actual.UserName);
            Assert.AreEqual<string>(expected.PicturesUrl, actual.PicturesUrl);
            Assert.AreEqual<string>(expected.BuddyIconUrl, actual.BuddyIconUrl);
            Assert.AreEqual<bool>(expected.IsProUser, actual.IsProUser);
            Assert.AreEqual<long>(expected.UploadLimit, actual.UploadLimit);

            // Verify collections.
            VerifyFlickrCollections(expected.Groups, actual.Groups);
            VerifyFlickrCollections(expected.Sets, actual.Sets);
        }

        private static void VerifyUploadSchedule(UploadSchedule expected, UploadSchedule actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual<ScheduleType>(expected.Type, actual.Type);
            Assert.AreEqual<DateTime>(expected.StartTime, actual.StartTime);
            Assert.AreEqual<bool>(expected.WakeComputer, actual.WakeComputer);
            Assert.AreEqual<bool>(expected.OnlyIfLoggedOn, actual.OnlyIfLoggedOn);
            Assert.AreEqual<bool>(expected.OnlyIfNotOnBatteryPower, actual.OnlyIfNotOnBatteryPower);

            Assert.AreEqual<int>(expected.HourlyInterval, actual.HourlyInterval);
            Assert.AreEqual<int>(expected.HourlyIntervalDuration, actual.HourlyIntervalDuration);

            Assert.AreEqual<int>(expected.DailyInterval, actual.DailyInterval);

            Assert.AreEqual<int>(expected.WeeklyInterval, actual.WeeklyInterval);
            Assert.AreEqual<bool>(expected.WeeklyOnMonday, actual.WeeklyOnMonday);
            Assert.AreEqual<bool>(expected.WeeklyOnTuesday, actual.WeeklyOnTuesday);
            Assert.AreEqual<bool>(expected.WeeklyOnWednesday, actual.WeeklyOnWednesday);
            Assert.AreEqual<bool>(expected.WeeklyOnThursday, actual.WeeklyOnThursday);
            Assert.AreEqual<bool>(expected.WeeklyOnFriday, actual.WeeklyOnFriday);
            Assert.AreEqual<bool>(expected.WeeklyOnSaturday, actual.WeeklyOnSaturday);
            Assert.AreEqual<bool>(expected.WeeklyOnSunday, actual.WeeklyOnSunday);
        }

        public static void VerifyFlickrCollections(ObservableCollection<PictureCollection> expected, ObservableCollection<PictureCollection> actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual<int>(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                // Verify item.
                var expectedItem = expected[i];
                var actualItem = actual[i];

                Assert.IsNotNull(actualItem);
                Assert.AreEqual<string>(expectedItem.Id, actualItem.Id);
                Assert.AreEqual<string>(expectedItem.ImageUrl, actualItem.ImageUrl);
                Assert.AreEqual<string>(expectedItem.Name, actualItem.Name);
            }
        }

        public static void VerifyList<T>(IList<T> expected, IList<T> actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual<int>(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                // Verify item.
                var expectedItem = expected[i];
                var actualItem = actual[i];

                Assert.IsNotNull(actualItem);
                Assert.AreEqual(expectedItem, actualItem);
            }
        }
    }
}