using System;
using System.ComponentModel.Composition;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Specifies that the <see cref="IEventPlugin"/> with which this attribute is associated can handle one or more scheduled task events.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SupportedScheduledTaskEventAttribute : Attribute
    {
        /// <summary>
        /// Gets the supported scheduled task event.
        /// </summary>
        public ScheduledTaskEventType SupportedScheduledTaskEvent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedScheduledTaskEventAttribute"/> class.
        /// </summary>
        /// <param name="supportedScheduledTaskEvent">The supported scheduled task event.</param>
        public SupportedScheduledTaskEventAttribute(ScheduledTaskEventType supportedScheduledTaskEvent)
        {
            this.SupportedScheduledTaskEvent = supportedScheduledTaskEvent;
        }
    }
}