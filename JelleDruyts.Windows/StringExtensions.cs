﻿using System;
using System.Globalization;

namespace JelleDruyts.Windows
{
    /// <summary>
    /// Provides extension methods for the <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions
    {
        #region ToDisplayString

        /// <summary>
        /// Gets the number of bytes as a string.
        /// </summary>
        /// <param name="bytes">The number of bytes.</param>
        /// <returns>The size in kilobytes or megabytes of the given number of bytes, with 2 decimal places.</returns>
        public static string ToDisplayString(this long bytes)
        {
            double kiloBytes = (bytes / ((double)1024));
            if (kiloBytes > 1024)
            {
                return string.Format(CultureInfo.CurrentCulture, "{0} MB", (kiloBytes / ((double)1024)).ToString("f2", CultureInfo.CurrentCulture));
            }
            else
            {
                return string.Format(CultureInfo.CurrentCulture, "{0} KB", kiloBytes.ToString("f2", CultureInfo.CurrentCulture));
            }
        }

        #endregion

        #region ToPercentageString

        /// <summary>
        /// Gets the value formatted as a percentage.
        /// </summary>
        /// <param name="percentage">The percentage to format.</param>
        /// <returns>The value formatted as a percentage (where 1 is 100%), with no decimal places.</returns>
        public static string ToPercentageString(this double percentage)
        {
            return percentage.ToString("p0", CultureInfo.CurrentCulture);
        }

        #endregion

        #region Pluralize

        /// <summary>
        /// Pluralizes the specified string if needed, i.e. if the specified count is not equal to 1.
        /// </summary>
        /// <param name="singular">The singular version of the string.</param>
        /// <param name="count">The number of items.</param>
        /// <returns>The singular version of the string if count is equal to 1, the plural version otherwise.</returns>
        public static string Pluralize(this string singular, int count)
        {
            if (singular == null)
            {
                throw new ArgumentNullException("singular");
            }
            if (count < 0)
            {
                throw new ArgumentException("The count cannot be negative.");
            }
            if (count == 1)
            {
                return singular;
            }
            else
            {
                if (singular.EndsWith("y"))
                {
                    return singular.Substring(0, singular.Length - 1) + "ies";
                }
                else
                {
                    return singular + "s";
                }
            }
        }

        #endregion

        #region ToCountString

        /// <summary>
        /// Gets the count formatted as a string.
        /// </summary>
        /// <param name="count">The number of items.</param>
        /// <param name="singular">The singular version of the string describing an item.</param>
        /// <returns>A string that consists of the count followed by the optionally pluralized description.</returns>
        public static string ToCountString(this int count, string singular)
        {
            return count.ToCountString(singular, null, null);
        }

        /// <summary>
        /// Gets the count formatted as a string.
        /// </summary>
        /// <param name="count">The number of items.</param>
        /// <param name="singular">The singular version of the string describing an item.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>A string that consists of the prefix, then the count followed by the optionally pluralized description.</returns>
        public static string ToCountString(this int count, string singular, string prefix)
        {
            return count.ToCountString(singular, prefix, null);
        }

        /// <summary>
        /// Gets the count formatted as a string.
        /// </summary>
        /// <param name="count">The number of items.</param>
        /// <param name="singular">The singular version of the string describing an item.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="postfix">The postfix.</param>
        /// <returns>A string that consists of the prefix, then the count followed by the optionally pluralized description, and then the postfix.</returns>
        public static string ToCountString(this int count, string singular, string prefix, string postfix)
        {
            return prefix + count + " " + singular.Pluralize(count) + postfix;
        }

        #endregion
    }
}