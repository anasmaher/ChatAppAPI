using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.ViewModels.UserVMs
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        [MaxLength(100)]
        public string Email { get; set; }
    }
}
