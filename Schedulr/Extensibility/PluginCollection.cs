using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using JelleDruyts.Windows;
using JelleDruyts.Windows.Text;
using Schedulr.Infrastructure;
using Schedulr.Messages;
using Schedulr.Models;
using Schedulr.Providers;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Represents a collection of plugin instances.
    /// </summary>
    public class PluginCollection
    {
        #region Properties

        /// <summary>
        /// Gets a unique identifier of this collection.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the scope of the collection.
        /// </summary>
        public PluginScope Scope { get; private set; }

        /// <summary>
        /// Gets the category of the collection.
        /// </summary>
        public PluginCategory Category { get; private set; }

        /// <summary>
        /// Gets the description of the collection.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the available plugin types of which instances can be added this collection.
        /// </summary>
        public ObservableCollection<PluginType> AvailablePluginTypes { get; private set; }

        /// <summary>
        /// Gets the plugin instances in this collection.
        /// </summary>
        public ObservableCollection<PluginInstance> PluginInstances { get; private set; }

        /// <summary>
        /// Gets the types for which text template tokens are available in this collection.
        /// </summary>
        public IEnumerable<Type> TemplateTokenTypes { get; private set; }

        /// <summary>
        /// Gets or sets the settings of the collection.
        /// </summary>
        public PluginCollectionSettings Settings { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginCollection"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of this collection.</param>
        /// <param name="scope">The scope of the collection.</param>
        /// <param name="category">The category of the collection.</param>
        /// <param name="description">The description of the event.</param>
        /// <param name="availablePluginTypes">The available plugin types of which instances can be added this collection.</param>
        /// <param name="templateTokenTypes">The types for which text template tokens are available in this collection.</param>
        public PluginCollection(string id, PluginScope scope, PluginCategory category, string description, ObservableCollection<PluginType> availablePluginTypes, IEnumerable<Type> templateTokenTypes)
        {
            this.Id = id;
            this.Scope = scope;
            this.Category = category;
            this.Description = description;
            this.AvailablePluginTypes = availablePluginTypes;
            this.PluginInstances = new ObservableCollection<PluginInstance>();
            this.TemplateTokenTypes = templateTokenTypes;
        }

        #endregion

        #region Call Plugin Instances

        /// <summary>
        /// Executes an action on each plugin instance of a specified type that is enabled.
        /// </summary>
        /// <param name="action">The action to perform on the plugin.</param>
        public void ForEachEnabledPluginInstance<TPluginInstance>(Action<TPluginInstance> action) where TPluginInstance : class, IPlugin
        {
            ForEachEnabledPluginInstance<TPluginInstance>(action, null);
        }

        /// <summary>
        /// Executes an action on each plugin instance of a specified type that is enabled.
        /// </summary>
        /// <typeparam name="TPluginInstance">The type of the plugin instance.</typeparam>
        /// <param name="action">The action to perform on the plugin.</param>
        /// <param name="selector">The plugin instance selector.</param>
        public void ForEachEnabledPluginInstance<TPluginInstance>(Action<TPluginInstance> action, Predicate<PluginInstance> selector) where TPluginInstance : class, IPlugin
        {
            Logger.Log(string.Format(CultureInfo.CurrentCulture, "Calling plugin instances for collection \"{0}\"", this.Id), TraceEventType.Verbose);
            foreach (var plugin in this.PluginInstances.Where(p => p.IsEnabled && p.PluginIsOfType<TPluginInstance>() && (selector == null || selector(p))))
            {
                try
                {
                    Logger.Log(string.Format(CultureInfo.CurrentCulture, "Calling plugin instance \"{0}\" for plugin collection \"{1}\"", plugin.Id, this.Id), TraceEventType.Verbose);
                    plugin.Execute<TPluginInstance>(action);
                }
                catch (Exception exc)
                {
                    Logger.Log(string.Format(CultureInfo.CurrentCulture, "An exception occurred while calling plugin instance \"{0}\" for plugin collection \"{1}\"", plugin.Id, this.Id), exc, TraceEventType.Warning);
                    Messenger.Send<StatusMessage>(new PluginStatusMessage(plugin, exc.Message, exc, TraceEventType.Error));
                }
            }
        }

        #endregion

        #region Add Plugin Instance

        /// <summary>
        /// Determines whether a new instance of the specified plugin type can be added to this collection.
        /// </summary>
        /// <param name="pluginType">The plugin type.</param>
        /// <returns><see langword="true"/> if a new instance of the specified plugin type can be added to this collection; otherwise, <see langword="false"/>.</returns>
        public bool CanAddPluginInstance(PluginType pluginType)
        {
            if (pluginType == null)
            {
                return false;
            }
            else if (pluginType.InstantiationPolicy == PluginInstantiationPolicy.MultipleInstancesPerScope)
            {
                return true;
            }
            else if (pluginType.InstantiationPolicy == PluginInstantiationPolicy.SingleInstancePerScope)
            {
                return !HasPluginInstance(pluginType);
            }
            else if (pluginType.InstantiationPolicy == PluginInstantiationPolicy.SingleInstancePerApplication)
            {
                return !PluginManager.HasPluginInstance(pluginType);
            }
            return false;
        }

        /// <summary>
        /// Adds an instance of the specified plugin type to this collection.
        /// </summary>
        /// <param name="pluginType">The plugin type.</param>
        public void AddPluginInstance(PluginType pluginType)
        {
            AddPluginInstance(null, pluginType, null, true);
        }

        /// <summary>
        /// Adds an instance of the specified plugin type to this collection.
        /// </summary>
        /// <param name="id">The ID of the plugin instance.</param>
        /// <param name="pluginType">The plugin type.</param>
        /// <param name="serializedSettings">The serialized settings object for the plugin.</param>
        /// <param name="isEnabled">A value that determines if this plugin instance is enabled.</param>
        public void AddPluginInstance(string id, PluginType pluginType, string serializedSettings, bool isEnabled)
        {
            if (!CanAddPluginInstance(pluginType))
            {
                throw new InvalidOperationException("A new instance of the requested plugin cannot be added because there is already an instance and the plugin type doesn't support multiple instances.");
            }
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    id = PluginManager.GetNewPluginInstanceId();
                }
                this.PluginInstances.Add(new PluginInstance(id, this, pluginType, serializedSettings, isEnabled));
            }
            catch (Exception exc)
            {
                Logger.Log("A new instance of the requested plugin could not be added", exc);
                throw;
            }
        }

        #endregion

        #region Remove Plugin Instance

        /// <summary>
        /// Removes the specified plugin instance from this collection.
        /// </summary>
        /// <param name="pluginInstance">The plugin instance to remove.</param>
        public void RemovePlugin(PluginInstance pluginInstance)
        {
            if (!this.PluginInstances.Contains(pluginInstance))
            {
                throw new InvalidOperationException("The requested plugin cannot be removed from this collection since it is not currently part of it.");
            }
            try
            {
                pluginInstance.Dispose();
            }
            catch (Exception exc)
            {
                Logger.Log("An exception occurred while disposing the plugin", exc);
            }
            this.PluginInstances.Remove(pluginInstance);
        }

        #endregion

        #region Move Plugin Instance Up/Down

        /// <summary>
        /// Determines whether the specified plugin instance can be moved up.
        /// </summary>
        /// <param name="pluginInstance">The plugin instance.</param>
        /// <returns><see langword="true"/> if the plugin instanhce can be moved up; otherwise, <see langword="false"/>.</returns>
        public bool CanMovePluginInstanceUp(PluginInstance pluginInstance)
        {
            return (pluginInstance != null && this.PluginInstances.Contains(pluginInstance) && this.PluginInstances.IndexOf(pluginInstance) > 0);
        }

        /// <summary>
        /// Moves the specified plugin instance up.
        /// </summary>
        /// <param name="pluginInstance">The plugin instance.</param>
        public void MovePluginInstanceUp(PluginInstance pluginInstance)
        {
            if (!CanMovePluginInstanceUp(pluginInstance))
            {
                throw new InvalidOperationException("The requested plugin cannot be moved up.");
            }
            var index = this.PluginInstances.IndexOf(pluginInstance);
            if (index > 0)
            {
                this.PluginInstances.Move(index, index - 1);
            }
        }

        /// <summary>
        /// Determines whether the specified plugin instance can be moved down.
        /// </summary>
        /// <param name="pluginInstance">The plugin instance.</param>
        /// <returns><see langword="true"/> if the plugin instanhce can be moved down; otherwise, <see langword="false"/>.</returns>
        public bool CanMovePluginInstanceDown(PluginInstance pluginInstance)
        {
            return (pluginInstance != null && this.PluginInstances.Contains(pluginInstance) && this.PluginInstances.IndexOf(pluginInstance) < this.PluginInstances.Count - 1);
        }

        /// <summary>
        /// Moves the specified plugin instance down.
        /// </summary>
        /// <param name="pluginInstance">The plugin instance.</param>
        public void MovePluginInstanceDown(PluginInstance pluginInstance)
        {
            if (!CanMovePluginInstanceDown(pluginInstance))
            {
                throw new InvalidOperationException("The requested plugin cannot be moved down.");
            }
            var index = this.PluginInstances.IndexOf(pluginInstance);
            if (index < this.PluginInstances.Count - 1)
            {
                this.PluginInstances.Move(index, index + 1);
            }
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Determines whether an instance of the specified plugin type is in this collection.
        /// </summary>
        /// <param name="pluginType">The plugin type.</param>
        /// <returns><see langword="true"/> if an instance of the specified plugin type is in this collection; otherwise, <see langword="false"/>.</returns>
        public bool HasPluginInstance(PluginType pluginType)
        {
            return this.PluginInstances.Any(p => p.Type == pluginType);
        }

        /// <summary>
        /// Gets the text template tokens that are available in this collection.
        /// </summary>
        /// <returns>The text template tokens that are available in this collection.</returns>
        public IList<TemplateTokenInfo> GetTemplateTokens()
        {
            return TemplateTokenProvider.ProvideTemplateTokens(this.TemplateTokenTypes);
        }

        #endregion
    }
}