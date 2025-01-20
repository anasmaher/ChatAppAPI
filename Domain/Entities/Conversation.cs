namespace Domain.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public bool IsGroup { get; set; } = false;

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual List<Message> Messages { get; set; }
        public virtual List<ConversationMember> Members { get; set; }
    }
}