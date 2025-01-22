namespace Application.DTOs.ConversationDTOs
{
    public class EditMessageDTO
    {
        public int messageId { get; set; }

        public Guid convoId { get; set; }

        public string content { get; set; }
    }
}
