using System;
using System.ComponentModel.Composition;

namespace Schedulr.Extensibility
{
    /// <summary>
    /// Specifies that a type is a plugin.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class PluginAttribute : ExportAttribute
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the plugin.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the purpose of the plugin (i.e. the short description).
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// Gets or sets the description of the plugin (i.e. the full explanation of what it does).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the optional author of the plugin.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the optional version of the plugin.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the optional URL of the plugin.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the instantiation policy of the plugin. By default, this is <see cref="PluginInstantiationPolicy.MultipleInstancesPerScope"/>.
        /// </summary>
        public PluginInstantiationPolicy InstantiationPolicy { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginAttribute"/> class with a <see cref="PluginInstantiationPolicy.MultipleInstancesPerScope"/> instantiation policy.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="purpose">The purpose of the plugin (i.e. the short description).</param>
        /// <param name="description">The description of the plugin (i.e. the full explanation of what it does).</param>
        public PluginAttribute(string name, string purpose, string description)
            : this(name, purpose, description, null, null, null, PluginInstantiationPolicy.MultipleInstancesPerScope)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="purpose">The purpose of the plugin (i.e. the short description).</param>
        /// <param name="description">The description of the plugin (i.e. the full explanation of what it does).</param>
        /// <param name="author">The optional author of the plugin.</param>
        /// <param name="version">The optional version of the plugin.</param>
        /// <param name="url">The optional URL of the plugin.</param>
        /// <param name="instantiationPolicy">The instantiation policy of the plugin.</param>
        public PluginAttribute(string name, string purpose, string description, string author, string version, string url, PluginInstantiationPolicy instantiationPolicy)
            : base(typeof(IPlugin))
        {
            this.Name = name;
            this.Purpose = purpose;
            this.Description = (string.IsNullOrEmpty(description) ? this.Purpose : description);
            this.Author = author;
            this.Version = version;
            this.Url = url;
            this.InstantiationPolicy = instantiationPolicy;
        }

        #endregion
    }
}