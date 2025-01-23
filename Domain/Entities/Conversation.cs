namespace Domain.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<ConversationMember> Members { get; set; } = new List<ConversationMember>();
    }
}