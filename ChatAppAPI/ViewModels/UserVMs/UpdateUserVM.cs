using Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.ViewModels.UserVMs
{
    public class UpdateUserVM
    {
        [Length(2, 20, ErrorMessage = "First name must be between 2 and 20 characters.")]
        public string? FirstName { get; set; }

        [Length(2, 20, ErrorMessage = "First name must be between 2 and 20 characters.")]
        public string? LastName { get; set; }

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile? Photo { get; set; }
    }
}
