using Domain.Enums;

namespace Application.DTOs.NotificationDTOs
{
    public class NotificationDTO
    {
        public int Id { get; set; }

        public NotificationEnum Type { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedDate { get; set; }

        public string SenderUserId { get; set; }

        public string SenderFirstName { get; set; }

        public string SenderLastName { get; set; }
    }
}
