using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Represents an event that can be handled by a plugin.
    /// </summary>
    /// <typeparam name="TEventType">The type of the event handled by the plugin.</typeparam>
    public class PluginEvent<TEventType> : PluginEvent where TEventType : struct
    {
        #region Properties

        /// <summary>
        /// Gets the event that can be handled by a plugin.
        /// </summary>
        public TEventType Event { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginEvent&lt;TEventType&gt;"/> class.
        /// </summary>
        /// <param name="event">The event that can be handled by a plugin.</param>
        /// <param name="scope">The scope of the event.</param>
        /// <param name="category">The category of the collection.</param>
        /// <param name="description">The description of the event.</param>
        /// <param name="availablePluginTypes">The available plugin types that support this event.</param>
        /// <param name="templateTokenTypes">The types for which text template tokens are available in this collection.</param>
        public PluginEvent(TEventType @event, PluginScope scope, PluginCategory category, string description, ObservableCollection<PluginType> availablePluginTypes, IList<Type> templateTokenTypes)
            : base(PluginManager.GetEventId<TEventType>(@event), scope, category, description, availablePluginTypes, templateTokenTypes)
        {
            this.Event = @event;
        }

        #endregion
    }

    /// <summary>
    /// Represents an event that can be handled by a plugin.
    /// </summary>
    public abstract class PluginEvent : PluginCollection
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginEvent"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of this collection.</param>
        /// <param name="scope">The scope of the event.</param>
        /// <param name="category">The category of the collection.</param>
        /// <param name="description">The description of the event.</param>
        /// <param name="availablePluginTypes">The available plugin types that support this event.</param>
        /// <param name="templateTokenTypes">The types for which text template tokens are available in this collection.</param>
        public PluginEvent(string id, PluginScope scope, PluginCategory category, string description, ObservableCollection<PluginType> availablePluginTypes, IList<Type> templateTokenTypes)
            : base(id, scope, category, description, availablePluginTypes, templateTokenTypes)
        {
        }

        #endregion
    }
}