using System;
using System.Windows.Controls;

namespace Schedulr.Common
{
    /// <summary>
    /// Integer validation rule for min andmax values
    /// </summary>
    public class ShortValidationRule : ValidationRule
    {
        private short _min = short.MinValue;
        private short _max = short.MaxValue;
        private string _fieldName = "Field";
        private string _customMessage = String.Empty;

        /// <summary>
        /// Minimum value
        /// </summary>
        public short Min
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        /// Maximum value
        /// </summary>
        public short Max
        {
            get { return _max; }
            set { _max = value; }
        }

        /// <summary>
        /// Field name
        /// </summary>
        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        /// <summary>
        /// Custom message
        /// </summary>
        public string CustomMessage
        {
            get { return _customMessage; }
            set { _customMessage = value; }
        }

        /// <summary>
        /// The validation result
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            short num = 0;

            if (!short.TryParse(value.ToString(), out num))
                return new ValidationResult(false, String.Format("{0} must contain an integer value.", FieldName));

            if (num < Min || num > Max)
            {
                if (!String.IsNullOrEmpty(CustomMessage))
                    return new ValidationResult(false, CustomMessage);


                return new ValidationResult(false, String.Format("{0} must be between {1} and {2}.",
                                           FieldName, Min, Max));
            }

            return new ValidationResult(true, null);
        }
    }
}
