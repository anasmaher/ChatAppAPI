using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.ViewModels.UserVMs
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Old Password is required.")]
        [DataType(DataType.Password)]
        [MaxLength(100)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        [DataType(DataType.Password)]
        [MaxLength(100)]
        public string NewPassword { get; set; }
    }
}
