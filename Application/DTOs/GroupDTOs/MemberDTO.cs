namespace Application.DTOs.GroupDTOs
{
    public class MemberDTO
    {
        public string UserId { get; set; }

        public Guid ConversationId { get; set; }

        public bool IsAdmin { get; set; }
    }
}
