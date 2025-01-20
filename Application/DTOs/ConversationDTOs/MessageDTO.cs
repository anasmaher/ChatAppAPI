namespace Application.DTOs.ConversationDTOs
{
    public class MessageDTO
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; }

        public string SenderId { get; set; }

        public string SenderFirstName { get; set; }

        public string SenderLastName { get; set; }
    }
}
