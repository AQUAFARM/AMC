using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using JelleDruyts.Windows.Text;
using Schedulr.Models;

namespace Schedulr.Providers
{
    /// <summary>
    /// Provides text template tokens.
    /// </summary>
    public static class TemplateTokenProvider
    {
        #region Fields

        private static IDictionary<Type, object> sampleValues = new Dictionary<Type, object>();

        #endregion

        #region Get & Set Sample Value

        /// <summary>
        /// Gets the currently registered sample value of a certain type.
        /// </summary>
        /// <typeparam name="T">The type of the sample value.</typeparam>
        /// <returns>The sample value.</returns>
        public static T GetSampleValue<T>()
        {
            if (sampleValues.ContainsKey(typeof(T)))
            {
                return (T)sampleValues[typeof(T)];
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Registers a sample value for a certain type.
        /// </summary>
        /// <typeparam name="T">The type of the sample value.</typeparam>
        /// <param name="sampleValue">The sample value.</param>
        public static void SetSampleValue<T>(T sampleValue)
        {
            sampleValues[typeof(T)] = sampleValue;
        }

        #endregion

        #region ProvideTemplateTokenValues

        /// <summary>
        /// Provides token values for a number of source objects.
        /// </summary>
        /// <param name="tokenValues">The dictionary to which the token values must be added.</param>
        /// <param name="sourceValues">The source objects for which to provide token values.</param>
        public static void ProvideTemplateTokenValues(IDictionary<string, object> tokenValues, params object[] sourceValues)
        {
            var tokens = ProvideTemplateTokens(sourceValues).ToDictionary(i => i.Name, i => i.SampleValue);
            foreach (var token in tokens)
            {
                tokenValues.Add(token);
            }
        }

        #endregion

        #region ProvideTemplateTokens

        /// <summary>
        /// Provides token information for a source type.
        /// </summary>
        /// <param name="sourceType">The source type for which to provide token information based on its current sample value.</param>
        /// <returns>The available tokens for the given source type.</returns>
        public static IList<TemplateTokenInfo> ProvideTemplateTokens(Type sourceType)
        {
            return ProvideTemplateTokens(new Type[] { sourceType });
        }

        /// <summary>
        /// Provides token information for a number of source types.
        /// </summary>
        /// <param name="sourceTypes">The source types for which to provide token information based on their current sample values.</param>
        /// <returns>The available tokens for the given source types.</returns>
        public static IList<TemplateTokenInfo> ProvideTemplateTokens(IEnumerable<Type> sourceTypes)
        {
            return ProvideTemplateTokens(sourceTypes.Where(t => t != null && sampleValues.ContainsKey(t)).Select(t => sampleValues[t]));
        }

        /// <summary>
        /// Provides token information for a source object.
        /// </summary>
        /// <param name="sourceValue">The source object for which to provide token information.</param>
        /// <returns>The available tokens for the given source object.</returns>
        public static IList<TemplateTokenInfo> ProvideTemplateTokens(object sourceValue)
        {
            return ProvideTemplateTokens(new object[] { sourceValue });
        }

        /// <summary>
        /// Provides token information for a number of source objects.
        /// </summary>
        /// <param name="sourceValues">The source objects for which to provide token information.</param>
        /// <returns>The available tokens for the given source objects.</returns>
        public static IList<TemplateTokenInfo> ProvideTemplateTokens(IEnumerable<object> sourceValues)
        {
            var tokens = new List<TemplateTokenInfo>();
            if (sourceValues != null)
            {
                foreach (var value in sourceValues)
                {
                    if (value != null)
                    {
                        var sampleType = value.GetType();
                        // Skip if [Browsable(false)].
                        if (!sampleType.GetCustomAttributes(typeof(BrowsableAttribute), true).Cast<BrowsableAttribute>().Any(b => !b.Browsable))
                        {
                            foreach (var property in sampleType.GetProperties().OrderBy(p => p.Name))
                            {
                                // Skip if [Browsable(false)].
                                if (!property.GetCustomAttributes(typeof(BrowsableAttribute), true).Cast<BrowsableAttribute>().Any(b => !b.Browsable))
                                {
                                    // Prefix the token with the type name.
                                    var prefix = sampleType.Name;

                                    // Unless for a few cases that should use a modified prefix to make it cleaner.
                                    if (sampleType == typeof(ApplicationInfo))
                                    {
                                        prefix = "Application";
                                    }
                                    else if (sampleType == typeof(SchedulrConfiguration))
                                    {
                                        prefix = "Configuration";
                                    }
                                    tokens.Add(new TemplateTokenInfo(string.Format(CultureInfo.CurrentCulture, "{0}{1}", prefix, property.Name), property.GetValue(value, null)));
                                }
                            }
                        }
                    }
                }
            }
            return tokens;
        }

        #endregion
    }
}