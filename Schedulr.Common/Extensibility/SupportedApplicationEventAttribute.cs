using System;
using System.ComponentModel.Composition;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Specifies that the <see cref="IEventPlugin"/> with which this attribute is associated can handle one or more application events.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SupportedApplicationEventAttribute : Attribute
    {
        /// <summary>
        /// Gets the supported application event.
        /// </summary>
        public ApplicationEventType SupportedApplicationEvent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedApplicationEventAttribute"/> class.
        /// </summary>
        /// <param name="supportedApplicationEvent">The supported application event.</param>
        public SupportedApplicationEventAttribute(ApplicationEventType supportedApplicationEvent)
        {
            this.SupportedApplicationEvent = supportedApplicationEvent;
        }
    }
}