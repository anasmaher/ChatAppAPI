namespace Application.DTOs.RelationshipDTOs
{
    public class FriendRequestDTO
    {
        public int Id { get; set; }

        public string SenderId { get; set; }

        public string SenderFirstName { get; set; }

        public string SenderLastName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
