using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Boilerplate.DAL.Validation
{
    public class ValidGuidAttribute: ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' does is not valid guid";

        public ValidGuidAttribute() : base(DefaultErrorMessage)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = Convert.ToString(value, CultureInfo.CurrentCulture);

            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (!Guid.TryParse(input, out var _))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}
