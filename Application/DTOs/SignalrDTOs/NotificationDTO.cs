using Domain.Enums;

namespace Application.DTOs.SignalrDTOs
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public NotificationEnum Type { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }

        public string SenderUserId { get; set; }
        public string SenderUsername { get; set; }
    }
}
