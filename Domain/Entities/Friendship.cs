using Domain.Enums;

namespace Domain.Entities
{
    public class Friendship
    {
        public int Id { get; set; }

        public string User1Id { get; set; }
        public virtual AppUser User1 { get; set; }

        public string User2Id { get; set; }
        public virtual AppUser User2 { get; set; }

        public FriendshipStatusEnum Status { get; set; }
        public DateOnly CreatedDate { get; set; }

        public string ActionUserId { get; set; }
        public virtual AppUser ActionUser { get; set; }
    }
}
