using Domain.Enums;

namespace Domain.Entities
{
    public class UserRelationship
    {
        public int Id { get; set; }

        public string User1Id { get; set; }
        public virtual AppUser User1 { get; set; }

        public string User2Id { get; set; }
        public virtual AppUser User2 { get; set; }

        public RelationshipStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ActionUserId { get; set; }
        public virtual AppUser ActionUser { get; set; }
    }
}
