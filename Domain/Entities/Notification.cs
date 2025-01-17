using Domain.Enums;

namespace Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public virtual AppUser User { get; set; }

        public string SenderUserId { get; set; }
        public virtual AppUser SenderUser { get; set; }

        public NotificationEnum Type { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsRead { get; set; }
    }
}
