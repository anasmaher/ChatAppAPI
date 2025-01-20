namespace Application.DTOs.ConversationDTOs
{
    public class SendMessageDTO
    {
        public Guid ConversationId { get; set; }

        public string Content { get; set; }
    }
}
