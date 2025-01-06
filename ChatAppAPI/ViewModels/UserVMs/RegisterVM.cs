using Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.ViewModels.UserVMs
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "First name is required.")]
        [Length(2, 20, ErrorMessage = "First name must be between 2 and 20 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Length(2, 20, ErrorMessage = "Last name must be between 2 and 20 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage ="Email address is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match.")]
        public string ConfirmedPassword { get; set; }

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile? Photo { get; set; }
    }
}
