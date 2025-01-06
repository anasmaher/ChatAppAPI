using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxFileSizeInBytes;

        public MaxFileSizeAttribute(long maxFileSizeInBytes)
        {
            _maxFileSizeInBytes = maxFileSizeInBytes;
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSizeInBytes)
                {
                    return new ValidationResult($"Maximum allowed file size is {_maxFileSizeInBytes / (1024 * 1024)} MB.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
