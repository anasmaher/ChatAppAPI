namespace Domain.Entities
{
    public class Message
    {
        public int id { get; set; }

        public string content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public string SenderId { get; set; }
        public int ConversationId { get; set; }

        public virtual AppUser Sender { get; set; }
        public virtual Conversation Conversation { get; set; }
    }
}
