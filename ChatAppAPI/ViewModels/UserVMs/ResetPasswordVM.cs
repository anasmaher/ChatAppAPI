using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.ViewModels.UserVMs
{
    public class ResetPasswordVM
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MaxLength(100)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }
    }
}
