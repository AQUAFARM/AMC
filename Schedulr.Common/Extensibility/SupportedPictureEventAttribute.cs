using System;
using System.ComponentModel.Composition;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Specifies that the <see cref="IEventPlugin"/> with which this attribute is associated can handle one or more picture events.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SupportedPictureEventAttribute : Attribute
    {
        /// <summary>
        /// Gets the supported picture event.
        /// </summary>
        public PictureEventType SupportedPictureEvent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedPictureEventAttribute"/> class.
        /// </summary>
        /// <param name="supportedPictureEvent">The supported picture event.</param>
        public SupportedPictureEventAttribute(PictureEventType supportedPictureEvent)
        {
            this.SupportedPictureEvent = supportedPictureEvent;
        }
    }
}