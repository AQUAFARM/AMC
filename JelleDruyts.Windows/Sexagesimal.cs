using System;
using System.Globalization;

namespace JelleDruyts.Windows
{
    /// <summary>
    /// Provides functionality around sexagesimal numbers.
    /// </summary>
    public static class Sexagesimal
    {
        /// <summary>
        /// Attempts to parse a sexagesimal value, typically in the form "hours,minutes,seconds[N|E|S|W]".
        /// </summary>
        /// <param name="value">The sexagesimal value to parse.</param>
        /// <param name="result">The parsed value.</param>
        /// <returns><see langword="true"/> if the value was parsed successfully, <see langword="false"/> otherwise.</returns>
        public static bool TryParse(string value, out double result)
        {
            result = 0;
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();

                var inverse = false;
                if (value.EndsWith("S", StringComparison.InvariantCultureIgnoreCase) || value.EndsWith("W", StringComparison.InvariantCultureIgnoreCase))
                {
                    inverse = true;
                }
                while (!char.IsDigit(value[value.Length - 1]))
                {
                    value = value.Substring(0, value.Length - 1);
                }
                var order = 0;
                foreach (var component in value.Split(','))
                {
                    double componentValue;
                    if (!double.TryParse(component, NumberStyles.Any, CultureInfo.InvariantCulture, out componentValue))
                    {
                        break;
                    }
                    result += componentValue / Math.Pow(60, order);
                    order++;
                }

                if (inverse)
                {
                    result = -result;
                }
                return true;
            }
            return false;
        }

    }
}