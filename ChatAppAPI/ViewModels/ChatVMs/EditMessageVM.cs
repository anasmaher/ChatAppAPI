using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.ViewModels.ChatVMs
{
    public class EditMessageVM
    {
        [Required]
        [DataType(DataType.MultilineText)]
        [MaxLength(2000)]
        public string content { get; set; }
    }
}
