using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Common.CustomAttributes
{
    public class RequiredByFieldValueAttribute : ValidationAttribute
    {
        private string _field { get; set; }
        private object _fieldValue { get; set; }
        private int[] itemInt { get; set; }
        private string[] itemString { get; set; }
        private string[] _validateItems { get; set; }

        public RequiredByFieldValueAttribute(string field, object fieldValue)
            : base("The {0} field is required by " + field)
        {
            _field = field;
            _fieldValue = fieldValue;
        }

        public RequiredByFieldValueAttribute(string field, object fieldValue, int[] items)
            : base("The {0} field is required by " + field +
                   " and value is not in " + string.Join(",", items))
        {
            _field = field;
            _fieldValue = fieldValue;
            itemInt = items;
        }

        public RequiredByFieldValueAttribute(string field, object fieldValue, string[] items)
            : base("The {0} field is required by " + field +
                   " and value is not in " + string.Join(",", items))
        {
            _field = field;
            _fieldValue = fieldValue;
            itemString = items;
        }

        public RequiredByFieldValueAttribute(string field, string[] validateItems)
        {
            _field = field;
            _validateItems = validateItems;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_field);
            if (property == null)
            {
                return new ValidationResult(
                    string.Format("Unknown property: {0}", _field)
                );
            }
            var fieldValue = property.GetValue(validationContext.ObjectInstance, null);

            // If value null -> return error
            //if (value == null)
            //    return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
            //                                , new string[] { validationContext.MemberName });

            if (_validateItems != null && _validateItems.Length > 0)
            {
                // If value null -> return error
                if (value == null)
                    return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                                                , new string[] { validationContext.MemberName });

                var selectedItem = _validateItems.FirstOrDefault(q => q.Split('|')[0].Equals(fieldValue));
                if (selectedItem != null && !selectedItem.Split('|')[1].Contains(value.ToString()))
                    return new ValidationResult($"The {validationContext.MemberName} field is required by {_field}" +
                        $" and value is not in {selectedItem.Split('|')[1]}");

                return null;
            }
            else
            {
                // If field value not match -> return
                if (!fieldValue.Equals(_fieldValue))
                    return null;

                //// If value null -> return error
                if (value == null)
                    return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                                                , new string[] { validationContext.MemberName });

                // If value not in array -> return error
                if (itemInt != null && itemInt.Length > 0)
                {
                    int val = (int)value;
                    if (!itemInt.Contains(val))
                        return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                                                    , new string[] { validationContext.MemberName });

                    return null;
                }
                else if (itemString != null && itemString.Length > 0)
                {
                    string val = (string)value;
                    if (!itemString.Contains(val))
                        return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                                                    , new string[] { validationContext.MemberName });

                    return null;
                }
                // Value is valid
                else
                    return null;

            }
        }
    }
}
