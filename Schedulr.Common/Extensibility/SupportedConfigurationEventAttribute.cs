using System;
using System.ComponentModel.Composition;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Specifies that the <see cref="IEventPlugin"/> with which this attribute is associated can handle one or more configuration events.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SupportedConfigurationEventAttribute : Attribute
    {
        /// <summary>
        /// Gets the supported configuration event.
        /// </summary>
        public ConfigurationEventType SupportedConfigurationEvent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedConfigurationEventAttribute"/> class.
        /// </summary>
        /// <param name="supportedConfigurationEvent">The supported configuration event.</param>
        public SupportedConfigurationEventAttribute(ConfigurationEventType supportedConfigurationEvent)
        {
            this.SupportedConfigurationEvent = supportedConfigurationEvent;
        }
    }
}