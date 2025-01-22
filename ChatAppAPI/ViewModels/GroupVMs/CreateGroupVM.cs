using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.ViewModels.GroupVMs
{
    public class CreateGroupVM
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public List<string> MemberIds { get; set; }
    }
}
