using System;
using System.ComponentModel.Composition;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Specifies that the <see cref="IEventPlugin"/> with which this attribute is associated can handle one or more general account events.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SupportedGeneralAccountEventAttribute : Attribute
    {
        /// <summary>
        /// Gets the supported general account event.
        /// </summary>
        public GeneralAccountEventType SupportedGeneralAccountEvent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedGeneralAccountEventAttribute"/> class.
        /// </summary>
        /// <param name="supportedGeneralAccountEvent">The supported general account event.</param>
        public SupportedGeneralAccountEventAttribute(GeneralAccountEventType supportedGeneralAccountEvent)
        {
            this.SupportedGeneralAccountEvent = supportedGeneralAccountEvent;
        }
    }
}