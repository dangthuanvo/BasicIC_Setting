using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EmailValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                MailAddress addr = new MailAddress(value.ToString());

                if (addr.Address == value.ToString())
                    return null;
                else
                    return new ValidationResult("The Email address is invalid");
            }
            catch
            {
                return new ValidationResult("The Email address is invalid");
            }
        }
    }
}
