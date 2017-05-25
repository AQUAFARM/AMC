using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Schedulr.Infrastructure;
using Schedulr.Models;
using Schedulr.Providers;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Manages plugins.
    /// </summary>
    public static class PluginManager
    {
        #region Constants & Fields

        private const string RenderingCollectionId = "Rendering";
        private static readonly List<PluginCollection> pluginCollections;

        #endregion

        #region Static Constructor

        /// <summary>
        /// Initializes the <see cref="PluginManager"/> class.
        /// </summary>
        static PluginManager()
        {
            pluginCollections = new List<PluginCollection>();

            try
            {
                // Discover all plugin types.
                var catalog = new DirectoryCatalog(Program.ApplicationDirectory);
                Logger.Log("Plugin path: " + catalog.FullPath, TraceEventType.Verbose);
                Logger.Log("Potential plugin files: " + string.Join(";", catalog.LoadedFiles.ToArray()), TraceEventType.Verbose);
                var container = new CompositionContainer(catalog);
                var availablePluginTypes = new List<PluginType>();
                foreach (var export in container.GetExports<IPlugin, IDictionary<string, object>>())
                {
                    try
                    {
                        Logger.Log("Discovered plugin: " + export.Value.GetType(), TraceEventType.Verbose);
                        availablePluginTypes.Add(new PluginType(export.Value.GetType(), export.Metadata));
                    }
                    catch (Exception exc)
                    {
                        Logger.Log("An exception occurred while discovering a plugin type", exc);
                        continue;
                    }
                }

                // Initialize rendering plugins.
                pluginCollections.Add(new PluginCollection(RenderingCollectionId, PluginScope.Account, PluginCategory.Rendering, "A picture or video is rendered before it is uploaded", new ObservableCollection<PluginType>(availablePluginTypes.Where(t => typeof(IRenderingPlugin).IsAssignableFrom(t.PluginInstanceType) && t.SupportedRenderingTypes.Any())), new Type[] { typeof(ApplicationInfo), typeof(Picture) }));

                // Initialize all events with their matching plugin types.
                pluginCollections.AddRange(GetPluginEvents<ApplicationEventType>(PluginScope.Application, PluginCategory.ApplicationEvents, availablePluginTypes, new Type[] { typeof(ApplicationInfo) }, (t, e) => typeof(IEventPlugin).IsAssignableFrom(t.PluginInstanceType) && t.SupportedApplicationEvents.Contains(e)));
                pluginCollections.AddRange(GetPluginEvents<ConfigurationEventType>(PluginScope.Application, PluginCategory.ApplicationEvents, availablePluginTypes, new Type[] { typeof(ApplicationInfo), typeof(SchedulrConfiguration) }, (t, e) => typeof(IEventPlugin).IsAssignableFrom(t.PluginInstanceType) && t.SupportedConfigurationEvents.Contains(e)));
                pluginCollections.AddRange(GetPluginEvents<GeneralAccountEventType>(PluginScope.Application, PluginCategory.ApplicationEvents, availablePluginTypes, new Type[] { typeof(ApplicationInfo), typeof(Account) }, (t, e) => typeof(IEventPlugin).IsAssignableFrom(t.PluginInstanceType) && t.SupportedGeneralAccountEvents.Contains(e)));
                pluginCollections.AddRange(GetPluginEvents<AccountEventType>(PluginScope.Account, PluginCategory.AccountEvents, availablePluginTypes, new Type[] { typeof(ApplicationInfo), typeof(Account) }, (t, e) => typeof(IEventPlugin).IsAssignableFrom(t.PluginInstanceType) && t.SupportedAccountEvents.Contains(e)));
                pluginCollections.AddRange(GetPluginEvents<BatchEventType>(PluginScope.Account, PluginCategory.AccountEvents, availablePluginTypes, new Type[] { typeof(ApplicationInfo), typeof(Batch) }, (t, e) => typeof(IEventPlugin).IsAssignableFrom(t.PluginInstanceType) && t.SupportedBatchEvents.Contains(e)));
                pluginCollections.AddRange(GetPluginEvents<PictureEventType>(PluginScope.Account, PluginCategory.AccountEvents, availablePluginTypes, new Type[] { typeof(ApplicationInfo), typeof(Picture) }, (t, e) => typeof(IEventPlugin).IsAssignableFrom(t.PluginInstanceType) && t.SupportedPictureEvents.Contains(e)));
                pluginCollections.AddRange(GetPluginEvents<ScheduledTaskEventType>(PluginScope.Account, PluginCategory.AccountEvents, availablePluginTypes, new Type[] { typeof(ApplicationInfo), typeof(UploadSchedule) }, (t, e) => typeof(IEventPlugin).IsAssignableFrom(t.PluginInstanceType) && t.SupportedScheduledTaskEvents.Contains(e)));

                Logger.Log(string.Format(CultureInfo.CurrentCulture, "Finished discovering plugins: {0} plugin types found in {1} collections", availablePluginTypes.Count, pluginCollections.Count), TraceEventType.Verbose);

                // Set the available collections on the plugin types.
                foreach (var pluginType in availablePluginTypes)
                {
                    foreach (var collection in pluginCollections.Where(e => e.AvailablePluginTypes.Contains(pluginType)))
                    {
                        pluginType.AvailablePluginCollections.Add(collection);
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.Log("An exception occurred while discovering plugins", exc);
            }
        }

        #endregion

        #region Manage Plugin Instances

        /// <summary>
        /// Loads the specified plugins.
        /// </summary>
        /// <param name="plugins">The plugins to load.</param>
        public static void LoadPlugins(IEnumerable<PluginConfiguration> plugins)
        {
            // Load all requested plugins.
            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    LoadPlugin(plugin);
                }
            }
        }

        /// <summary>
        /// Unloads the specified plugins.
        /// </summary>
        /// <param name="scope">The scope for which to unload all plugins.</param>
        /// <returns>The configurations of all unloaded plugins.</returns>
        public static IEnumerable<PluginConfiguration> UnloadPlugins(PluginScope scope)
        {
            return GetLoadedPlugins(scope, true);
        }

        /// <summary>
        /// Gets the loaded plugins.
        /// </summary>
        /// <param name="scope">The scope for which to retrieve all plugins.</param>
        /// <returns>The configurations of all loaded plugins.</returns>
        public static IEnumerable<PluginConfiguration> GetLoadedPlugins(PluginScope scope)
        {
            return GetLoadedPlugins(scope, false);
        }

        /// <summary>
        /// Determines whether an instance of the specified plugin type is loaded.
        /// </summary>
        /// <param name="pluginType">The plugin type.</param>
        /// <returns><see langword="true"/> if an instance of the specified plugin type is loaded; otherwise, <see langword="false"/>.</returns>
        public static bool HasPluginInstance(PluginType pluginType)
        {
            return pluginCollections.Any(e => e.HasPluginInstance(pluginType));
        }

        #endregion

        #region Manage Plugin Types

        /// <summary>
        /// Gets the available plugin types for a specified category.
        /// </summary>
        /// <param name="category">The category for which to retrieve all plugin types.</param>
        /// <returns>The plugin types for the specified category.</returns>
        public static IEnumerable<PluginType> GetPluginTypes(PluginCategory category)
        {
            return GetPluginCollections(category).SelectMany(e => e.AvailablePluginTypes).Distinct();
        }

        #endregion

        #region Manage Plugin Collections

        /// <summary>
        /// Sets the settings for the plugin collections.
        /// </summary>
        /// <param name="pluginCollectionSettings">The plugin collection settings.</param>
        public static void SetPluginCollectionSettings(ObservableCollection<PluginCollectionSettings> pluginCollectionSettings)
        {
            if (pluginCollectionSettings == null)
            {
                throw new ArgumentNullException("pluginCollectionSettings");
            }
            foreach (var pluginCollection in pluginCollections)
            {
                var settings = pluginCollectionSettings.Where(s => s.PluginCollectionId == pluginCollection.Id).FirstOrDefault();
                if (settings == null)
                {
                    settings = new PluginCollectionSettings { PluginCollectionId = pluginCollection.Id };
                    pluginCollectionSettings.Add(settings);
                }
                pluginCollection.Settings = settings;
            }
        }

        /// <summary>
        /// Gets the available plugin collections for a specified category.
        /// </summary>
        /// <param name="category">The category for which to retrieve all plugin collections.</param>
        /// <returns>The collections for the specified category.</returns>
        public static IEnumerable<PluginCollection> GetPluginCollections(PluginCategory category)
        {
            return pluginCollections.Where(e => e.Category == category);
        }

        /// <summary>
        /// Gets the available plugin collections for a specified scope.
        /// </summary>
        /// <param name="scope">The scope for which to retrieve all plugin collections.</param>
        /// <returns>The collections for the specified scope.</returns>
        public static IEnumerable<PluginCollection> GetPluginCollections(PluginScope scope)
        {
            return pluginCollections.Where(e => e.Scope == scope);
        }

        #endregion

        #region Determine IDs

        public static string GetNewPluginInstanceId()
        {
            return Guid.NewGuid().ToString();
        }

        public static string GetEventId<TEventType>(TEventType @event) where TEventType : struct
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", typeof(TEventType).Name, @event.ToString());
        }

        public static string GetPluginTypeId(Type pluginInstanceType)
        {
            return pluginInstanceType.FullName;
        }

        #endregion

        #region Helper Methods

        private static bool LoadPlugin(PluginConfiguration plugin)
        {
            foreach (var collection in pluginCollections)
            {
                if (string.Equals(collection.Id, plugin.CollectionId, StringComparison.Ordinal))
                {
                    foreach (var pluginType in collection.AvailablePluginTypes)
                    {
                        if (string.Equals(pluginType.Id, plugin.PluginTypeId, StringComparison.Ordinal))
                        {
                            try
                            {
                                collection.AddPluginInstance(plugin.Id, pluginType, plugin.Settings, plugin.IsEnabled);
                                return true;
                            }
                            catch (Exception exc)
                            {
                                Logger.Log(string.Format(CultureInfo.CurrentCulture, "The configured plugin of type \"{0}\" for collection \"{1}\" could not be loaded", plugin.PluginTypeId, plugin.CollectionId), exc);
                                return false;
                            }
                        }
                    }
                }
            }
            Logger.Log(string.Format(CultureInfo.CurrentCulture, "The configured plugin of type \"{0}\" for collection \"{1}\" could not be loaded; either the plugin type or the collection doesn't exist anymore.", plugin.PluginTypeId, plugin.CollectionId), TraceEventType.Error);
            return false;
        }

        private static IEnumerable<PluginConfiguration> GetLoadedPlugins(PluginScope scope, bool unload)
        {
            var plugins = new List<PluginConfiguration>();
            foreach (var collection in GetPluginCollections(scope))
            {
                foreach (var plugin in collection.PluginInstances.ToArray())
                {
                    try
                    {
                        plugins.Add(new PluginConfiguration(plugin.Id, plugin.IsEnabled, plugin.GetSerializedSettings(), collection.Id, plugin.Type.Id));
                    }
                    catch (Exception exc)
                    {
                        Logger.Log("An exception occurred while retrieving a plugin's settings", exc);
                    }
                    if (unload)
                    {
                        collection.RemovePlugin(plugin);
                    }
                }
            }
            return plugins;
        }

        private static ICollection<PluginEvent> GetPluginEvents<TEventType>(PluginScope scope, PluginCategory category, IList<PluginType> availablePluginTypes, IList<Type> templateTokenTypes, Func<PluginType, TEventType, bool> selector) where TEventType : struct
        {
            var events = new List<PluginEvent>();
            foreach (TEventType @event in Enum.GetValues(typeof(TEventType)))
            {
                var description = (DescriptionAttribute)Attribute.GetCustomAttribute(typeof(TEventType).GetField((@event.ToString())), typeof(DescriptionAttribute));
                events.Add(new PluginEvent<TEventType>(@event, scope, category, description.Description, new ObservableCollection<PluginType>(availablePluginTypes.Where(f => selector(f, @event))), templateTokenTypes));
            }
            return events;
        }

        #endregion

        #region Calling Plugin Instances

        /// <summary>
        /// Raises a specific account plugin event.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public static void OnAccountEvent(AccountEventArgs args)
        {
            TemplateTokenProvider.ProvideTemplateTokenValues(args.TemplateTokenValues, args.ApplicationInfo, args.Account);
            pluginCollections.OfType<PluginEvent<AccountEventType>>().Where(e => e.Event == args.Event).First().ForEachEnabledPluginInstance<IEventPlugin>(p => p.OnAccountEvent(args));
        }

        /// <summary>
        /// Raises an application plugin event.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public static void OnApplicationEvent(ApplicationEventArgs args)
        {
            TemplateTokenProvider.ProvideTemplateTokenValues(args.TemplateTokenValues, args.ApplicationInfo);
            pluginCollections.OfType<PluginEvent<ApplicationEventType>>().Where(e => e.Event == args.Event).First().ForEachEnabledPluginInstance<IEventPlugin>(p => p.OnApplicationEvent(args));
        }

        /// <summary>
        /// Raises a specific batch plugin event.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public static void OnBatchEvent(BatchEventArgs args)
        {
            TemplateTokenProvider.ProvideTemplateTokenValues(args.TemplateTokenValues, args.ApplicationInfo, args.Batch);
            pluginCollections.OfType<PluginEvent<BatchEventType>>().Where(e => e.Event == args.Event).First().ForEachEnabledPluginInstance<IEventPlugin>(p => p.OnBatchEvent(args));
        }

        /// <summary>
        /// Raises a configuration plugin event.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public static void OnConfigurationEvent(ConfigurationEventArgs args)
        {
            TemplateTokenProvider.ProvideTemplateTokenValues(args.TemplateTokenValues, args.ApplicationInfo, args.Configuration);
            pluginCollections.OfType<PluginEvent<ConfigurationEventType>>().Where(e => e.Event == args.Event).First().ForEachEnabledPluginInstance<IEventPlugin>(p => p.OnConfigurationEvent(args));
        }

        /// <summary>
        /// Raises a general account plugin event.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public static void OnGeneralAccountEvent(GeneralAccountEventArgs args)
        {
            TemplateTokenProvider.ProvideTemplateTokenValues(args.TemplateTokenValues, args.ApplicationInfo, args.Account);
            pluginCollections.OfType<PluginEvent<GeneralAccountEventType>>().Where(e => e.Event == args.Event).First().ForEachEnabledPluginInstance<IEventPlugin>(p => p.OnGeneralAccountEvent(args));
        }

        /// <summary>
        /// Raises a picture plugin event.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public static void OnPictureEvent(PictureEventArgs args)
        {
            TemplateTokenProvider.ProvideTemplateTokenValues(args.TemplateTokenValues, args.ApplicationInfo, args.Picture);
            pluginCollections.OfType<PluginEvent<PictureEventType>>().Where(e => e.Event == args.Event).First().ForEachEnabledPluginInstance<IEventPlugin>(p => p.OnPictureEvent(args));
        }

        /// <summary>
        /// Raises a scheduled task plugin event.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public static void OnScheduledTaskEvent(ScheduledTaskEventArgs args)
        {
            TemplateTokenProvider.ProvideTemplateTokenValues(args.TemplateTokenValues, args.ApplicationInfo, args.UploadSchedule);
            pluginCollections.OfType<PluginEvent<ScheduledTaskEventType>>().Where(e => e.Event == args.Event).First().ForEachEnabledPluginInstance<IEventPlugin>(p => p.OnScheduledTaskEvent(args));
        }

        /// <summary>
        /// Calls plugins to render a file.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        /// <param name="fileToRender">The file to render.</param>
        /// <returns>The rendered file.</returns>
        public static Stream OnRenderingFile(RenderingEventArgs args, Stream fileToRender)
        {
            TemplateTokenProvider.ProvideTemplateTokenValues(args.TemplateTokenValues, args.ApplicationInfo, args.Picture);
            pluginCollections.Where(c => string.Equals(c.Id, RenderingCollectionId, StringComparison.Ordinal)).First().ForEachEnabledPluginInstance<IRenderingPlugin>(p => fileToRender = p.OnRenderingFile(args, fileToRender), i => i.Type.SupportedRenderingTypes.Contains(args.IsVideo ? RenderingType.Video : RenderingType.Picture));
            return fileToRender;
        }

        /// <summary>
        /// Calls plugins to notify that file rendering is complete.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        public static void OnRenderingFileCompleted(RenderingEventArgs args)
        {
            TemplateTokenProvider.ProvideTemplateTokenValues(args.TemplateTokenValues, args.ApplicationInfo, args.Picture);
            pluginCollections.Where(c => string.Equals(c.Id, RenderingCollectionId, StringComparison.Ordinal)).First().ForEachEnabledPluginInstance<IRenderingPlugin>(p => p.OnRenderingFileCompleted(args), i => i.Type.SupportedRenderingTypes.Contains(args.IsVideo ? RenderingType.Video : RenderingType.Picture));
        }

        #endregion
    }
}