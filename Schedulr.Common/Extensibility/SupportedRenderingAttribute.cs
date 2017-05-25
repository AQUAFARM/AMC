using System;
using System.ComponentModel.Composition;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Specifies that the <see cref="IRenderingPlugin"/> with which this attribute is associated can render a certain content type.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SupportedRenderingAttribute : Attribute
    {
        /// <summary>
        /// Gets the supported rendering.
        /// </summary>
        public RenderingType SupportedRendering { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedRenderingAttribute"/> class.
        /// </summary>
        /// <param name="supportedRendering">The supported rendering.</param>
        public SupportedRenderingAttribute(RenderingType supportedRendering)
        {
            this.SupportedRendering = supportedRendering;
        }
    }
}