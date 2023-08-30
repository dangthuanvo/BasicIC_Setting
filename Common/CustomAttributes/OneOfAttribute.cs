using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Common.CustomAttributes
{
    public class OneOfAttribute : ValidationAttribute
    {
        private int[] ItemInt { get; set; }
        private string[] ItemString { get; set; }

        public OneOfAttribute(params int[] items) : base("{0} value is not in " + String.Join(",", items))
        {
            this.ItemInt = items;
        }

        public OneOfAttribute(params string[] items) : base("{0} value is not in " + String.Join(",", items))
        {
            this.ItemString = items;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (ItemInt != null && ItemInt.Length > 0)
            {
                int val = (int)value;
                if (!ItemInt.Contains(val))
                {
                    return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                                                , new string[] { validationContext.MemberName });
                }
                return null;
            }
            else
            {
                string val = (string)value;
                if (!ItemString.Contains(val))
                {
                    return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                                                , new string[] { validationContext.MemberName });
                }
                return null;
            }
        }
    }
}
