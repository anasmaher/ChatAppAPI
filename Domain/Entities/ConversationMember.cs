namespace Domain.Entities
{
    public class ConversationMember
    {
        public string UserId { get; set; }
        public Guid ConversationId { get; set; }

        public virtual AppUser User { get; set; }
        public virtual Conversation Conversation { get; set; }

        public bool IsAdmin { get; set; }
    }
}
