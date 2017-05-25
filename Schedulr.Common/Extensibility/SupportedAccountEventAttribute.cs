using System;
using System.ComponentModel.Composition;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Specifies that the <see cref="IEventPlugin"/> with which this attribute is associated can handle one or more specific account events.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SupportedAccountEventAttribute : Attribute
    {
        /// <summary>
        /// Gets the supported specific account event.
        /// </summary>
        public AccountEventType SupportedAccountEvent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedAccountEventAttribute"/> class.
        /// </summary>
        /// <param name="supportedAccountEvent">The supported specific account event.</param>
        public SupportedAccountEventAttribute(AccountEventType supportedAccountEvent)
        {
            this.SupportedAccountEvent = supportedAccountEvent;
        }
    }
}