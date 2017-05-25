using System;
using System.ComponentModel.Composition;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Specifies that the <see cref="IEventPlugin"/> with which this attribute is associated can handle one or more batch events.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SupportedBatchEventAttribute : Attribute
    {
        /// <summary>
        /// Gets the supported batch event.
        /// </summary>
        public BatchEventType SupportedBatchEvent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedBatchEventAttribute"/> class.
        /// </summary>
        /// <param name="supportedBatchEvent">The supported batch event.</param>
        public SupportedBatchEventAttribute(BatchEventType supportedBatchEvent)
        {
            this.SupportedBatchEvent = supportedBatchEvent;
        }
    }
}