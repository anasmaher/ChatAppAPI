using System.ComponentModel.DataAnnotations;

namespace ChatAppAPI.ViewModels.ChatVMs
{
    public class SendMesaageVM
    {
        [DataType(DataType.MultilineText)]
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }
    }
}
