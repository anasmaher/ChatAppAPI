using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.ViewModels.ForAdminVMs
{
    public class ChangeRoleVM
    {
        [Required(ErrorMessage = "Role name is required")]
        public string RoleName { get; set; }
    }
}
