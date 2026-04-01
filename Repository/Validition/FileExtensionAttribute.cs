using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace MYMVCAPP.Repository.Validation
{
    
    public class FileExtensionAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public FileExtensionAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLower();

                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult($"Hãy chọn file có đuôi: {string.Join(", ", _extensions)}");
                }
            }

            // Nếu không có file, hoặc hợp lệ
            return ValidationResult.Success;
        }
    }
}
