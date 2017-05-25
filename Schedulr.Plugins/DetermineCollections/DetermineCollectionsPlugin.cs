using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Schedulr.Extensibility;
using Schedulr.Models;

namespace Schedulr.Plugins.DetermineCollections
{
    [Plugin("Determine Sets & Groups", "Determine sets and groups for new pictures and videos based on the folder they are in", "Determines the sets and groups for new pictures and videos based on the folder they're in.", InstantiationPolicy = PluginInstantiationPolicy.MultipleInstancesPerScope)]
    [SupportedPictureEvent(PictureEventType.Adding)]
    public class DetermineCollectionsPlugin : EventPlugin<DetermineCollectionsPluginSettings, DetermineCollectionsPluginSettingsControl>
    {
        protected override DetermineCollectionsPluginSettingsControl GetSettingsControl(DetermineCollectionsPluginSettings settings)
        {
            return new DetermineCollectionsPluginSettingsControl(settings);
        }

        public override void OnPictureEvent(PictureEventArgs args)
        {
            var recursive = this.Settings.DetermineRecursively;
            if (this.Settings.DetermineSets)
            {
                this.Host.Logger.Log(string.Format(CultureInfo.CurrentCulture, "Determining Sets {0}for file \"{1}\"", (recursive ? "recursively " : string.Empty), args.Picture.FileName), TraceEventType.Information);
                DetermineCollections(args.Picture.FileName, args.Picture.SetIds, args.Account.UserInfo.Sets, recursive);
            }
            if (this.Settings.DetermineGroups)
            {
                this.Host.Logger.Log(string.Format(CultureInfo.CurrentCulture, "Determining Groups {0}for file \"{1}\"", (recursive ? "recursively " : string.Empty), args.Picture.FileName), TraceEventType.Information);
                DetermineCollections(args.Picture.FileName, args.Picture.GroupIds, args.Account.UserInfo.Groups, recursive);
            }
        }

        private static void DetermineCollections(string fileName, ObservableCollection<string> collectionIds, ObservableCollection<PictureCollection> collections, bool recursive)
        {
            var directory = new DirectoryInfo(Path.GetDirectoryName(fileName));
            while (directory != null)
            {
                foreach (var collection in collections)
                {
                    if (string.Equals(collection.Id, directory.Name, StringComparison.InvariantCultureIgnoreCase) || string.Equals(collection.Name, directory.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!collectionIds.Contains(collection.Id))
                        {
                            collectionIds.Add(collection.Id);
                        }
                    }
                }
                if (recursive)
                {
                    directory = directory.Parent;
                }
                else
                {
                    directory = null;
                }
            }
        }
    }
}