namespace Application.DTOs.ConversationDTOs
{
    public class DeleteMessageDTO
    {
        public int messageId { get; set; }

        public Guid convoId { get; set; }
    }
}
